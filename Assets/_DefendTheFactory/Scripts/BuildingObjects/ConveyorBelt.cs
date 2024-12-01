using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConveyorBelt : PlacedObject, IWorldItemSlot {

    public Vector2Int previousPosition { get; private set; }
    public Vector2Int nextPosition { get; private set; }
    WorldItem worldItem;
    bool isPartOfBuilding;

    protected override void Setup() {
        Vector2Int forwardVector = PlacedObjectTypeSO.GetDirForwardVector(dir);
        nextPosition = origin + forwardVector;

        GridCell[,] gridArray = BuildingSystem.Instance.grid.gridArray;
        Vector2Int backPosition = origin - forwardVector;

        if(ShouldSnap(gridArray, backPosition)) {
            previousPosition = backPosition;
            return;
        }

        Vector2Int rightVector = new Vector2Int(forwardVector.y, -forwardVector.x);
        Vector2Int rightPosition = origin + rightVector;
        Vector2Int leftPosition = origin - rightVector;

        bool snapRight = ShouldSnap(gridArray, rightPosition);
        bool snapLeft = ShouldSnap(gridArray, leftPosition);

        if(snapLeft && !snapRight) {
            previousPosition = leftPosition;
        } else if(snapRight && !snapLeft) {
            previousPosition = rightPosition;
        } else {
            previousPosition = backPosition;
        }
    }

    bool ShouldSnap(GridCell[,] gridArray, Vector2Int position) {
        if(!IsPositionValid(gridArray, position)) return false;

        ConveyorBelt belt = gridArray[position.x, position.y].placedObject as ConveyorBelt;
        return belt != null && belt.nextPosition == origin;
    }

    public bool IsPositionValid(GridCell[,] gridArray, Vector2Int position) {
        return position.x >= 0 && position.x < gridArray.GetLength(0) && position.y >= 0 && position.y < gridArray.GetLength(1);
    }

    public override void GridSetupDone() {
        BeltManager.Instance.AddBelt(this);
    }

    public void SetupBuildingBelt(Vector2Int origin, PlacedObjectTypeSO.Dir dir) {
        isPartOfBuilding = true;
        this.origin = origin;
        this.dir = dir;
        Setup();
        BuildingSystem.Instance.AddGhostBeltToGrid(origin, this);
        GridSetupDone();
    }

    public void ItemResetHasAlreadyMoved() {
        if(!IsEmpty()) {
            GetWorldItem().ResetHasAlreadyMoved();
        }
    }

    public void TakeAction() {
        if(IsEmpty() || !GetWorldItem().CanMove()) return;

        IWorldItemSlot worldItemSlot = BuildingSystem.Instance.GetGridObject(nextPosition).placedObject as IWorldItemSlot;
        if(worldItemSlot == null || !worldItemSlot.TrySetWorldItem(worldItem)) return;

        worldItem.MoveToGridPosition(worldItemSlot.GetGridPosition());
        worldItem.SetHasAlreadyMoved();
        RemoveWorldItem();
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
        if(IsEmpty()) {
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
        if(!IsEmpty()) {
            worldItem.DestroySelf();
        }

        BeltManager.Instance.RemoveBelt(this);
        base.DestroySelf();
    }

    public bool TryGetWorldItem(ItemSO[] filterItemSO, out WorldItem worldItem) {
        if(IsEmpty()) {
            // Nothing to grab
            worldItem = null;
            return false;
        } else {
            // Check if this WorldItem matches the filter OR there's no filter
            if(ItemSO.IsItemSOInFilter(GetWorldItem().GetItemSO(), filterItemSO) ||
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