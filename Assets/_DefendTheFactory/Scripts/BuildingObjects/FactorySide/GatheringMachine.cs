using System;
using UnityEngine;

public class GatheringMachine : PlacedObject, IItemStorage {

    public event EventHandler OnItemStorageCountChanged;

    [SerializeField] ConveyorBelt outputBelt;
    [SerializeField] float resourceSearchRange;
    [SerializeField] Vector2Int ghostBeltPosition;
    [SerializeField] ResourcesEnum gatheredResource;
    [SerializeField] float gatheringTime;
    [SerializeField] int maxStoredItems;

    public GameObject prefab;

    public ItemSO producedItem;

    bool resourcesInRange;
    bool storageFull;
    int storedItemsCount;
    float timer;

    void Update() {
        if(!resourcesInRange || storageFull) return;

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

        Vector2Int centerPosition = placedObjectTypeSO.GetMachineCenterPosition(origin, placedObjectTypeSO.width, placedObjectTypeSO.height, dir);

        GridCell[,] gridArray = BuildingSystem.Instance.grid.gridArray;

        int top = (int)Mathf.Ceil(centerPosition.y - resourceSearchRange);
        int bottom = (int)Mathf.Floor(centerPosition.y + resourceSearchRange - 1);
        int left = (int)Mathf.Ceil(centerPosition.x - resourceSearchRange);
        int right = (int)Mathf.Floor(centerPosition.x + resourceSearchRange - 1);


        for(int y = top; y <= bottom; y++) {
            for(int x = left; x <= right; x++) {
                if(IsPositionValid(gridArray, new Vector2Int(x, y)) && inside_circle(centerPosition, new Vector2Int(x, y))) {
                    //Instantiate(prefab, new Vector3(x, 0, y), Quaternion.identity);
                    ResourceNode node = gridArray[x, y].placedObject as ResourceNode;
                    if(node != null && node.ResourceType == gatheredResource) {
                        resourcesInRange = true;
                        Debug.Log($"FOUND RESOURCE ON CELL: {x}, {y}! I CAN GATHER :D");
                    }
                }
            }
        }

        if(!resourcesInRange) {
            Debug.Log($"NO RESOURCES CLOSE TO ME :( I WILL NOT WORK");
        }
    }

    bool inside_circle(Vector2Int center, Vector2Int point) {
        int dx = center.x - point.x;
        int dy = center.y - point.y;
        return dx * dx + dy * dy <= resourceSearchRange * resourceSearchRange;
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
    }

    void SetupBelt() {
        Vector2Int beltPos = placedObjectTypeSO.GetMachineBeltPosition(origin, ghostBeltPosition, dir);
        outputBelt.SetupBuildingBelt(beltPos, dir);
    }

    void Gather() {
        storedItemsCount++;

        if(storedItemsCount == maxStoredItems) {
            storageFull = true;
            timer = 0f;
        }
    }

    void OnEarlyTick() {
        if(storedItemsCount == 0) return;

        TryPutItemOnBelt();
    }

    void TryPutItemOnBelt() {
        if(outputBelt.worldItem != null) return;

        WorldItem worldItem = WorldItem.Create(outputBelt.GetGridPosition(), producedItem);
        outputBelt.TrySetWorldItem(worldItem);
        storedItemsCount--;
        storageFull = false;
    }

    public ItemSO GetMiningResourceItem() {
        return new ItemSO();
    }

    public int GetItemStoredCount(ItemSO filterItemScriptableObject) {
        return storedItemsCount;
    }

    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO) {
        if(ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.any, filterItemSO) ||
            ItemSO.IsItemSOInFilter(new ItemSO(), filterItemSO)) {
            // If filter matches any or filter matches this itemType
            if(storedItemsCount > 0) {
                storedItemsCount--;
                itemSO = new ItemSO();
                OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
                TriggerGridObjectChanged();
                return true;
            } else {
                itemSO = null;
                return false;
            }
        } else {
            itemSO = null;
            return false;
        }
    }

    public ItemSO[] GetItemSOThatCanStore() {
        return new ItemSO[] { GameAssets.i.itemSO_Refs.none };
    }

    public bool TryStoreItem(ItemSO itemScriptableObject) {
        return false;
    }

}