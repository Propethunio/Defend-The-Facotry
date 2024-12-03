using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheringMachine : PlacedMachine, IItemStorage {

    public event EventHandler OnItemStorageCountChanged;

    [SerializeField] ConveyorBelt outputBelt;
    [SerializeField] float resourceSearchRange;
    [SerializeField] Vector2Int ghostBeltPosition;
    [SerializeField] ResourcesEnum gatheredResource;
    [SerializeField] float gatheringTime;
    [SerializeField] int maxStoredItems;

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
        int resourceNodeSearchWidth = 2;
        int resourceNodeSearchHeight = 2;

        /* Find resources within range
        for(int x = origin.x - resourceNodeSearchWidth; x < origin.x + resourceNodeSearchWidth + placedObjectTypeSO.width; x++) {
            for(int y = origin.y - resourceNodeSearchHeight; y < origin.y + resourceNodeSearchHeight + placedObjectTypeSO.height; y++) {
                Vector2Int gridPosition = new Vector2Int(x, y);
                if(BuildingSystem.Instance.IsValidGridPosition(gridPosition)) {
                    PlacedObject placedObject = BuildingSystem.Instance.GetGridObject(gridPosition).placedObject;
                    if(placedObject != null) {
                        if(placedObject is ResourceNode) {
                            ResourceNode resourceNode = placedObject as ResourceNode;
                            miningResourceItem = resourceNode.GetItemScriptableObject();
                        }
                    }
                }
            }
        } */

        SetupBelt();
        Subscribe();
        resourcesInRange = true;
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