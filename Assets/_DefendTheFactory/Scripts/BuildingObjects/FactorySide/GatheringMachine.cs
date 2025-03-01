using System.Collections.Generic;
using UnityEngine;

public class GatheringMachine : BaseDataPlacedObject<GatheringMachineSO> {

    ConveyorBelt outputBelt;
    List<ResourceNode> nodesInRange = new();
    ResourceNode currentNode;
    int storedItemsCount;
    int productionTicks;

    public override void Initialize(Vector2Int origin, BuildingDir dir, GatheringMachineSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    void OnDestroy() {
        Unsubscribe();
    }

    public override void GridSetupDone() {
        SetupBelt();
        Subscribe();
        SearchForResources();

        if(nodesInRange.Count > 0) {
            PickClosestNode();
        }
    }

    public override void DestroySelf() {
        outputBelt.DestroySelf();
        base.DestroySelf();
    }

    void SearchForResources() {
        Vector2 centerPosition = buildableDataSO.GetCenterPosition(origin, dir);
        GridCell[,] gridArray = BuildingSystem.Instance.grid.gridArray;

        float searchRange = buildableDataSO.resourceSearchRange;
        int bottom = (int)Mathf.Floor(centerPosition.y - searchRange);
        int top = (int)Mathf.Ceil(centerPosition.y + searchRange - 1);
        int left = (int)Mathf.Floor(centerPosition.x - searchRange);
        int right = (int)Mathf.Ceil(centerPosition.x + searchRange - 1);

        for(int y = bottom; y <= top; y++) {
            for(int x = left; x <= right; x++) {

                if(IsPositionValid(gridArray, new Vector2Int(x, y)) && IsInsideCircle(centerPosition, new Vector2Int(x, y))) {
                    ResourceNode node = gridArray[x, y].placedObject as ResourceNode;
                    if(node != null && node.buildableDataSO.resourceType == buildableDataSO.gatheredResource) {
                        nodesInRange.Add(node);
                        node.NodeGatheredCompletly += HandleNodeDestroyed;
                    }
                }
            }
        }
    }

    bool IsInsideCircle(Vector2 center, Vector2Int point) {
        float dx = center.x - (point.x + 0.5f);
        float dy = center.y - (point.y + 0.5f);
        return dx * dx + dy * dy <= buildableDataSO.resourceSearchRange * buildableDataSO.resourceSearchRange + 0.5f;
    }

    void PickClosestNode() {
        Vector2 machineCenterPosition = buildableDataSO.GetCenterPosition(origin, dir);
        float currentDistance = Mathf.Infinity;
        currentNode = null;

        foreach(ResourceNode node in nodesInRange) {
            Vector2 nodeCenterPosition = node.buildableDataSO.GetCenterPosition(node.origin, node.dir);
            float distanceToNode = (machineCenterPosition - nodeCenterPosition).sqrMagnitude;

            if(distanceToNode < currentDistance) {
                currentNode = node;
                currentDistance = distanceToNode;
            }
        }
    }

    bool ShouldSnapBack(GridCell[,] gridArray, Vector2Int position, out ConveyorBelt belt) {
        belt = null;

        if(!IsPositionValid(gridArray, position)) return false;

        belt = gridArray[position.x, position.y].placedObject as ConveyorBelt;
        return belt != null && belt.previousPosition == origin;
    }

    bool IsPositionValid(GridCell[,] gridArray, Vector2Int position) {
        return position.x >= 0 && position.x < gridArray.GetLength(0) && position.y >= 0 && position.y < gridArray.GetLength(1);
    }

    void Subscribe() {
        TimeTickSystem.Instance.OnProductionTick += OnProductionTick;
        TimeTickSystem.Instance.OnEarlyTick += OnEarlyTick;
    }

    void Unsubscribe() {
        TimeTickSystem.Instance.OnProductionTick -= OnProductionTick;
        TimeTickSystem.Instance.OnEarlyTick -= OnEarlyTick;
        int nodesAmount = nodesInRange.Count;

        for(int i = 0; i < nodesAmount; i++) {
            nodesInRange[i].NodeGatheredCompletly -= HandleNodeDestroyed;
        }
    }

    void SetupBelt() {
        outputBelt = gameObject.AddComponent<ConveyorBelt>();
        Vector2Int beltPos = buildableDataSO.GetMachineBeltPosition(origin, buildableDataSO.outputBeltPosition, dir);
        outputBelt.SetupBuildingBelt(beltPos, dir, this);
    }

    void OnProductionTick() {
        if(currentNode == null || storedItemsCount == buildableDataSO.maxStoredOutputItems) return;

        productionTicks++;

        if(productionTicks == buildableDataSO.ticksForGather) {
            productionTicks = 0;
            Gather();
        }
    }

    void Gather() {
        currentNode.MineRsource();
        storedItemsCount++;
    }

    void HandleNodeDestroyed(ResourceNode node) {
        node.NodeGatheredCompletly -= HandleNodeDestroyed;
        nodesInRange.Remove(node);

        if(node == currentNode) {
            PickClosestNode();
        }
    }

    void OnEarlyTick() {
        if(storedItemsCount == 0) return;

        TryPutItemOnBelt();
    }

    void TryPutItemOnBelt() {
        if(outputBelt.worldItem != null) return;

        WorldItem worldItem = WorldItem.Create(outputBelt.origin, buildableDataSO.producedItem);
        outputBelt.SetWorldItem(worldItem);
        storedItemsCount--;
    }

    public ItemSO GetMiningResourceItem() {
        return new ItemSO();
    }

    public int GetItemStoredCount(ItemSO filterItemScriptableObject) {
        return storedItemsCount;
    }
}