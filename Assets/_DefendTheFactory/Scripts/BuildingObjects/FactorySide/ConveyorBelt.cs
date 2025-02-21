using System;
using UnityEngine;

public class ConveyorBelt : BaseDataPlacedObject<BaseBuildableObjectSO> {

    [HideInInspector] public Vector2Int previousPosition;
    public Vector2Int nextPosition { get; private set; }
    public WorldItem worldItem { get; private set; }
    public BasePlacedObject parentBuilding { get; private set; }

    BuildingSystem buildingSystem;

    public event Action<Vector2Int, Vector2Int> OnVisualUpdate;

    public override void Initialize(Vector2Int origin, BuildingDir dir, BaseBuildableObjectSO placedObjectDataSO) {
        BaseDataSet(origin, dir, placedObjectDataSO);
    }

    protected override void Setup() {
        buildingSystem = BuildingSystem.Instance;
        Vector2Int forwardVector = buildingSystem.GetDirForwardVector(dir);
        nextPosition = origin + forwardVector;

        GridCell[,] gridArray = buildingSystem.grid.gridArray;
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

    bool IsPositionValid(GridCell[,] gridArray, Vector2Int position) {
        return position.x >= 0 && position.x < gridArray.GetLength(0) && position.y >= 0 && position.y < gridArray.GetLength(1);
    }

    public override void GridSetupDone() {
        BeltManager.Instance.AddBelt(this);

        if(parentBuilding == null) {
            OnVisualUpdate?.Invoke(origin, previousPosition);
        }
    }

    public void SetupBuildingBelt(Vector2Int origin, BuildingDir dir, BasePlacedObject parentBuilding) {
        this.origin = origin;
        this.dir = dir;
        this.parentBuilding = parentBuilding;
        Setup();
        buildingSystem.AddGhostBeltToGrid(origin, this);
        GridSetupDone();
    }

    public void ItemResetHasAlreadyMoved() {
        if(worldItem != null) {
            worldItem.ResetHasAlreadyMoved();
        }
    }

    public bool TakeAction() {
        if(worldItem == null || !worldItem.CanMove()) return false;
        ConveyorBelt nextBelt = buildingSystem.GetGridObject(nextPosition).placedObject as ConveyorBelt;
        if(nextBelt == null) return false;
        if(!nextBelt.TrySetWorldItem(worldItem)) return true;
        worldItem.MoveToGridPosition(nextBelt.origin);
        worldItem.SetHasAlreadyMoved();
        worldItem = null;
        return false;
    }

    public void ResetWorldItem() {
        worldItem = null;
    }

    public bool TrySetWorldItem(WorldItem worldItem) {
        if(this.worldItem == null) {
            this.worldItem = worldItem;
            return true;
        } else {
            return false;
        }
    }

    public void SetWorldItem(WorldItem worldItem) {
        this.worldItem = worldItem;
    }

    public override void DestroySelf() {
        if(worldItem != null) {
            worldItem.DestroySelf();
        }

        BeltManager.Instance.RemoveBelt(this);
        base.DestroySelf();
    }
}