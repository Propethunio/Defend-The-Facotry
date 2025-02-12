using System;
using System.Collections.Generic;
using UnityEngine;

public class GatheringMachine : PlacedObject {

    public event EventHandler OnItemStorageCountChanged;

    [SerializeField] ConveyorBelt outputBelt;
    [SerializeField] float resourceSearchRange;
    [SerializeField] Vector2Int ghostBeltPosition;
    [SerializeField] ResourcesEnum gatheredResource;
    [SerializeField] float gatheringTime;
    [SerializeField] int maxStoredItems;
    [SerializeField] ItemSO producedItem;

    List<ResourceNode> nodesInRange = new();
    ResourceNode currentNode;
    int storedItemsCount;
    float timer;

    void Update() {
        if(currentNode == null || storedItemsCount == maxStoredItems) return;

        timer += Time.deltaTime;
        if(timer >= gatheringTime) {
            timer -= gatheringTime;
            Gather();
        }
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

        int bottom = (int)Mathf.Floor(centerPosition.y - resourceSearchRange);
        int top = (int)Mathf.Ceil(centerPosition.y + resourceSearchRange - 1);
        int left = (int)Mathf.Floor(centerPosition.x - resourceSearchRange);
        int right = (int)Mathf.Ceil(centerPosition.x + resourceSearchRange - 1);

        for(int y = bottom; y <= top; y++) {
            for(int x = left; x <= right; x++) {

                if(IsPositionValid(gridArray, new Vector2Int(x, y)) && IsInsideCircle(centerPosition, new Vector2Int(x, y))) {
                    ResourceNode node = gridArray[x, y].placedObject as ResourceNode;
                    if(node != null && node.resourceType == gatheredResource) {
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
        return dx * dx + dy * dy <= resourceSearchRange * resourceSearchRange + 0.5f;
    }

    void PickClosestNode() {
        Vector2 machineCenterPosition = buildableDataSO.GetCenterPosition(origin, dir);
        float currentDistance = Mathf.Infinity;

        foreach(ResourceNode node in nodesInRange) {
            Vector2 nodeCenterPosition = node.buildableDataSO.GetCenterPosition(node.origin, node.dir);
            float distanceToNode = Vector2.Distance(machineCenterPosition, nodeCenterPosition);

            if(currentNode == null || distanceToNode < currentDistance) {
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
        TimeTickSystem.Instance.OnEarlyTick += OnEarlyTick;
    }

    void Unsubscribe() {
        TimeTickSystem.Instance.OnEarlyTick -= OnEarlyTick;
        foreach(ResourceNode node in nodesInRange) {
            node.NodeGatheredCompletly -= HandleNodeDestroyed;
        }
    }

    void SetupBelt() {
        Vector2Int beltPos = buildableDataSO.GetMachineBeltPosition(origin, ghostBeltPosition, dir);
        outputBelt.SetupBuildingBelt(beltPos, dir, this);
    }

    void Gather() {
        currentNode.MineRsource();
        storedItemsCount++;
    }

    void HandleNodeDestroyed(ResourceNode node) {
        node.NodeGatheredCompletly -= HandleNodeDestroyed;
        nodesInRange.Remove(node);

        if(node == currentNode) {
            currentNode = null;
            PickClosestNode();
        }
    }

    void OnEarlyTick() {
        if(storedItemsCount == 0) return;

        TryPutItemOnBelt();
    }

    void TryPutItemOnBelt() {
        if(outputBelt.worldItem != null) return;

        WorldItem worldItem = WorldItem.Create(outputBelt.origin, producedItem);
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