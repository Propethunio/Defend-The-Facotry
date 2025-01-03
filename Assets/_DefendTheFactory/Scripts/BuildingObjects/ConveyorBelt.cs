﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : PlacedObject, IWorldItemSlot {

    private Vector2Int previousPosition;
    private Vector2Int gridPosition;
    private Vector2Int nextPosition;

    private WorldItem worldItem;

    protected override void Setup() {
        gridPosition = origin;

        previousPosition = origin + PlacedObjectTypeSO.GetDirForwardVector(dir) * -1;
        nextPosition = origin + PlacedObjectTypeSO.GetDirForwardVector(dir);
    }

    public override void GridSetupDone() {
        BeltManager.Instance.AddBelt(this);
    }

    public void ItemResetHasAlreadyMoved() {
        if (!IsEmpty()) {
            GetWorldItem().ResetHasAlreadyMoved();
        }
    }

    public void TakeAction() {
        if (!IsEmpty() && GetWorldItem().CanMove()) {
            // This conveyour belt is not empty and the item can move
            // Try push item forward
            PlacedObject nextPlacedObject = BuildingSystem.Instance.GetGridObject(nextPosition).placedObject;
            if (nextPlacedObject != null) {
                // Has object next
                if (nextPlacedObject is IWorldItemSlot) {
                    IWorldItemSlot worldItemSlot = nextPlacedObject as IWorldItemSlot;
                    if (worldItemSlot.TrySetWorldItem(worldItem)) {
                        // Successfully moved item onto next slot
                        // Move World Item
                        worldItem.SetGridPosition(worldItemSlot.GetGridPosition());
                        worldItem.SetHasAlreadyMoved();
                        // Remove current world item
                        RemoveWorldItem();
                    }
                }
            }
        }
    }

    public Vector2Int GetPreviousGridPosition() {
        return previousPosition;
    }

    public Vector2Int GetNextGridPosition() {
        return nextPosition;
    }

    public WorldItem GetWorldItem() {
        return worldItem;
    }

    public bool IsEmpty() {
        return worldItem == null;
    }

    public bool TrySetWorldItem(WorldItem worldItem) {
        if (IsEmpty()) {
            this.worldItem = worldItem;
            return true;
        } else {
            return false;
        }
    }

    public void RemoveWorldItem() {
        worldItem = null;
    }

    public override void DestroySelf() {
        if (!IsEmpty()) {
            worldItem.DestroySelf();
        }

        BeltManager.Instance.RemoveBelt(this);
        base.DestroySelf();
    }

    public bool TryGetWorldItem(ItemSO[] filterItemSO, out WorldItem worldItem) {
        if (IsEmpty()) {
            // Nothing to grab
            worldItem = null;
            return false;
        } else {
            // Check if this WorldItem matches the filter OR there's no filter
            if (ItemSO.IsItemSOInFilter(GetWorldItem().GetItemSO(), filterItemSO) ||
                ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.any, filterItemSO)) {
                // Return this World Item and Remove it
                worldItem = GetWorldItem();
                RemoveWorldItem();
                return true;
            } else {
                // This itemType does not match the filter
                worldItem = null;
                return false;
            }
        }
    }

    public ItemSO[] GetItemSOThatCanStore() {
        return new ItemSO[] { GameAssets.i.itemSO_Refs.any };
    }

}
