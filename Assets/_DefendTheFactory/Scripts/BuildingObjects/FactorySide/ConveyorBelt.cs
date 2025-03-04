using System;
using UnityEngine;

public class ConveyorBelt : BaseDataPlacedObject<BaseBuildableObjectSO> {

    [HideInInspector] public Vector2Int previousPosition;
    [HideInInspector] public Vector2Int nextPosition;
    public WorldItem firstItem { get; private set; }
    public WorldItem secondItem { get; private set; }
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

    public bool TakeActionOnFirstLoopedBelt(out bool didMovedItem, ConveyorBelt nextBelt) {
        bool hasItem = secondItem != null;
        bool feedback = TakeActionOnEndItemWithFeedback(nextBelt);
        didMovedItem = hasItem != (secondItem != null);
        TakeActionOnStartItem();
        return feedback;
    }

    public void TakeActionOnLastLoopedBelt(bool recivedItemInThisTick, ConveyorBelt nextBelt) {
        if(recivedItemInThisTick) return;
        TakeAction(nextBelt);
    }

    public bool TakeActionWithShouldRepeatFeedback(ConveyorBelt nextBelt) {
        bool feedback = TakeActionOnEndItemWithFeedback(nextBelt);
        TakeActionOnStartItem();
        return feedback;
    }

    bool TakeActionOnEndItemWithFeedback(ConveyorBelt nextBelt) {
        if(secondItem == null) return false;
        if(nextBelt == null) {
            nextBelt = buildingSystem.GetGridObject(nextPosition).placedObject as ConveyorBelt;
            if(nextBelt == null) return false;
        }
        if(!nextBelt.TrySetWorldItem(secondItem)) return true;
        secondItem.MoveToGridPosition(nextBelt.origin);
        secondItem = null;
        return false;
    }

    public void TakeAction(ConveyorBelt nextBelt) {
        TakeActionOnEndItem(nextBelt);
        TakeActionOnStartItem();
    }

    public void TakeLastBeltStandardAction() {
        TakeActionOnStartItem();
    }

    void TakeActionOnEndItem(ConveyorBelt nextBelt) {
        if(secondItem == null) return;
        if(nextBelt == null) {
            nextBelt = buildingSystem.GetGridObject(nextPosition).placedObject as ConveyorBelt;
            if(nextBelt == null) return;
        }
        if(!nextBelt.TrySetWorldItem(secondItem)) return;
        secondItem.MoveToGridPosition(nextBelt.origin);
        secondItem = null;
    }

    void TakeActionOnStartItem() {
        if(firstItem == null || secondItem != null) return;
        firstItem.MoveToGridPosition(nextPosition);
        secondItem = firstItem;
        firstItem = null;
    }

    public bool TakeActionOnFirstBeltAfterStop(ConveyorBelt nextBelt) {
        bool hasItem = secondItem != null;
        TakeActionOnEndItem(nextBelt);
        bool movedItem = hasItem && secondItem == null;
        TakeActionOnStartItem();
        return movedItem;
    }

    public void TakeActionOnLastRepeatBelt(ConveyorBelt nextBelt) {
        TakeActionOnEndItem(nextBelt);
    }

    public void ResetWorldItem() {
        secondItem = null;
    }

    public bool TrySetWorldItem(WorldItem worldItem) {
        if(firstItem == null) {
            firstItem = worldItem;
            return true;
        }
        return false;
    }

    public void SetWorldItem(WorldItem worldItem) {
        firstItem = worldItem;
    }

    public override void DestroySelf() {
        if(secondItem != null) {
            secondItem.DestroySelf();
        }

        BeltManager.Instance.RemoveBelt(this);
        base.DestroySelf();
    }
}