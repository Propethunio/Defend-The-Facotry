using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningMachine : PlacedObject, IItemStorage {

    public event EventHandler OnItemStorageCountChanged;

    [SerializeField] private Transform pfMiningDrone;


    private ItemSO miningResourceItem;

    private float miningTimer;
    private int storedItemCount;

    protected override void Setup() {
        //Debug.Log("MiningMachine.Setup()");
    }

    public override void GridSetupDone() {
        int resourceNodeSearchWidth = 2;
        int resourceNodeSearchHeight = 2;

        // Find resources within range
        for (int x = origin.x - resourceNodeSearchWidth; x < origin.x + resourceNodeSearchWidth + placedObjectTypeSO.width; x++) {
            for (int y = origin.y - resourceNodeSearchHeight; y < origin.y + resourceNodeSearchHeight + placedObjectTypeSO.height; y++) {
                Vector2Int gridPosition = new Vector2Int(x, y);
                if (BuildingSystem.Instance.IsValidGridPosition(gridPosition)) {
                    PlacedObject placedObject = BuildingSystem.Instance.GetGridObject(gridPosition).placedObject;
                    if (placedObject != null) {
                        if (placedObject is ResourceNode) {
                            ResourceNode resourceNode = placedObject as ResourceNode;
                            miningResourceItem = resourceNode.GetItemScriptableObject();
                        }
                    }
                }
            }
        }

        //Debug.Log("MiningMachine: " + miningResourceItem);

        //Transform droneTransform = Instantiate(pfMiningDrone, transform.position, Quaternion.identity);
    }

    public override string ToString() {
        if (miningResourceItem == null) {
            return "NO RESOURCES!";
        }
        return storedItemCount.ToString();
    }

    private void Update() {
        if (miningResourceItem == null) {
            // No resources in range!
            return;
        }

        miningTimer -= Time.deltaTime;
        if (miningTimer <= 0f) {
            miningTimer += miningResourceItem.miningTimer;

            storedItemCount += 1;
            OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
            TriggerGridObjectChanged();
        }
    }

    public ItemSO GetMiningResourceItem() {
        return miningResourceItem;
    }

    public int GetItemStoredCount(ItemSO filterItemScriptableObject) {
        return storedItemCount;
    }

    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO) {
        if (ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.any, filterItemSO) ||
            ItemSO.IsItemSOInFilter(miningResourceItem, filterItemSO)) {
            // If filter matches any or filter matches this itemType
            if (storedItemCount > 0) {
                storedItemCount--;
                itemSO = miningResourceItem;
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
