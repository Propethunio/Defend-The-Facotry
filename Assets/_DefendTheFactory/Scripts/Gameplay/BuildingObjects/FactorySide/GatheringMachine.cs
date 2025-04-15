using System;
using System.Collections.Generic;
using UnityEngine;

public class GatheringMachine : BaseDataPlacedObject<GatheringMachineSO> {
    public int storedItemsCount { get; private set; }

    private int productionTicks;
    private ConveyorBelt outputBelt;
    private List<ResourceNode> nodesInRange = new();
    private ResourceNode currentNode;

    public event Action<int> StoredItemsCountChanged, ResourcesInRangeChanged;
    public event Action<float> ProductionTicksChanged;

    protected override void Initialize(Vector2Int origin, BuildingDir dir, GatheringMachineSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    private void OnDestroy() {
        Unsubscribe();
    }

    public override void GridSetupDone() {
        SetupBelt();
        Subscribe();
        SearchForResources();

        if (nodesInRange.Count > 0) {
            PickClosestNode();
        }
    }

    public override void DestroySelf() {
        outputBelt.DestroySelf();
        base.DestroySelf();
    }

    public int GetResourcesInRangeAmount() {
        int amount = 0;
        int nodesCount = nodesInRange.Count;

        for (var i = 0; i < nodesCount; i++) {
            amount += nodesInRange[i].amountLeft;
        }

        return amount;
    }

    private void SearchForResources() {
        Vector2 centerPosition = buildableDataSO.GetCenterPosition(origin, dir);
        GridCell[,] gridArray = Injector.Resolve<BuildingSystem>().grid.gridArray;

        float searchRange = buildableDataSO.resourceSearchRange;
        int bottom = (int)Mathf.Floor(centerPosition.y - searchRange);
        int top = (int)Mathf.Ceil(centerPosition.y + searchRange - 1);
        int left = (int)Mathf.Floor(centerPosition.x - searchRange);
        int right = (int)Mathf.Ceil(centerPosition.x + searchRange - 1);

        for (int y = bottom; y <= top; y++) {
            for (int x = left; x <= right; x++) {
                if (!IsPositionValid(gridArray, new Vector2Int(x, y)) || !IsInsideCircle(centerPosition, new Vector2Int(x, y))) continue;

                ResourceNode node = gridArray[x, y].placedObject as ResourceNode;
                if (node == null || node.buildableDataSO.resourceType != buildableDataSO.gatheredResource || nodesInRange.Contains(node)) continue;

                nodesInRange.Add(node);
                node.ResourcesGathered += OnResourcesInRangeChanged;
                node.NodeGatheredCompletely += HandleNodeDestroyed;
            }
        }
    }

    private void OnResourcesInRangeChanged() {
        ResourcesInRangeChanged?.Invoke(GetResourcesInRangeAmount());
    }

    private bool IsInsideCircle(Vector2 center, Vector2Int point) {
        float dx = center.x - (point.x + 0.5f);
        float dy = center.y - (point.y + 0.5f);
        return dx * dx + dy * dy <= buildableDataSO.resourceSearchRange * buildableDataSO.resourceSearchRange + 0.5f;
    }

    private void PickClosestNode() {
        Vector2 machineCenterPosition = buildableDataSO.GetCenterPosition(origin, dir);
        float currentDistance = Mathf.Infinity;
        currentNode = null;
        int nodesCount = nodesInRange.Count;

        for (var i = 0; i < nodesCount; i++) {
            ResourceNode node = nodesInRange[i];
            Vector2 nodeCenterPosition = node.buildableDataSO.GetCenterPosition(node.origin, node.dir);
            float distanceToNode = (machineCenterPosition - nodeCenterPosition).sqrMagnitude;

            if (!(distanceToNode < currentDistance)) continue;

            currentNode = node;
            currentDistance = distanceToNode;
        }
    }

    private bool ShouldSnapBack(GridCell[,] gridArray, Vector2Int position, out ConveyorBelt belt) {
        belt = null;

        if (!IsPositionValid(gridArray, position)) return false;

        belt = gridArray[position.x, position.y].placedObject as ConveyorBelt;
        return belt != null && belt.previousPosition == origin;
    }

    private bool IsPositionValid(GridCell[,] gridArray, Vector2Int position) {
        return position.x >= 0 && position.x < gridArray.GetLength(0) && position.y >= 0 && position.y < gridArray.GetLength(1);
    }

    private void Subscribe() {
        TimeTickSystem timeTickSystem = Injector.Resolve<TimeTickSystem>();
        timeTickSystem.OnMicroTick += OnMicroTick;
        timeTickSystem.OnEarlyTick += OnEarlyTick;
    }

    private void Unsubscribe() {
        TimeTickSystem timeTickSystem = Injector.Resolve<TimeTickSystem>();
        timeTickSystem.OnMicroTick -= OnMicroTick;
        timeTickSystem.OnEarlyTick -= OnEarlyTick;
        int nodesAmount = nodesInRange.Count;

        for (int i = 0; i < nodesAmount; i++) {
            nodesInRange[i].NodeGatheredCompletely -= HandleNodeDestroyed;
            nodesInRange[i].ResourcesGathered -= OnResourcesInRangeChanged;
        }
    }

    private void SetupBelt() {
        outputBelt = gameObject.AddComponent<ConveyorBelt>();
        Vector2Int beltPos = buildableDataSO.GetMachineBeltPosition(origin, buildableDataSO.outputBeltPosition, dir);
        outputBelt.SetupBuildingBelt(beltPos, dir, this);
    }

    private void OnMicroTick() {
        if (currentNode == null || storedItemsCount == 2) return;

        productionTicks++;

        if (productionTicks == buildableDataSO.ticksForGather) {
            productionTicks = 0;
            Gather();
        }

        ProductionTicksChanged?.Invoke(GetTargetProgressNormalized());
    }

    public float GetTargetProgressNormalized() {
        return (float)(productionTicks + 1) / buildableDataSO.ticksForGather;
    }

    public float GetProgressNormalized() {
        return (float)productionTicks / buildableDataSO.ticksForGather;
    }

    private void Gather() {
        currentNode.MineResource();
        storedItemsCount++;
        StoredItemsCountChanged?.Invoke(storedItemsCount);
    }

    private void HandleNodeDestroyed(ResourceNode node) {
        node.NodeGatheredCompletely -= HandleNodeDestroyed;
        node.ResourcesGathered -= OnResourcesInRangeChanged;

        if (node.amountLeft > 0) {
            ResourcesInRangeChanged?.Invoke(GetResourcesInRangeAmount());
        }

        nodesInRange.Remove(node);

        if (node == currentNode) {
            PickClosestNode();
        }
    }

    private void OnEarlyTick() {
        if (storedItemsCount == 0) return;

        TryPutItemOnBelt();
    }

    private void TryPutItemOnBelt() {
        if (outputBelt.startItem != null) return;

        WorldItem worldItem = WorldItem.Create(outputBelt.origin, dir, buildableDataSO.producedItem);
        outputBelt.SetWorldItem(worldItem);
        storedItemsCount--;
        StoredItemsCountChanged?.Invoke(storedItemsCount);
    }
}