using System;
using UnityEngine;

public class ConveyorBelt : BaseDataPlacedObject<BaseBuildableObjectSO> {

    [HideInInspector] public Vector2Int previousPosition;
    [HideInInspector] public Vector2Int nextPosition;
    public WorldItem startItem { get; private set; }
    public WorldItem endItem { get; private set; }
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

    public override void DestroySelf() {
        if(startItem != null) {
            startItem.DestroySelf();
        }

        if(endItem != null) {
            endItem.DestroySelf();
        }

        BeltManager.Instance.RemoveBelt(this);
        base.DestroySelf();
    }

    public void SetupBuildingBelt(Vector2Int origin, BuildingDir dir, BasePlacedObject parentBuilding) {
        this.origin = origin;
        this.dir = dir;
        this.parentBuilding = parentBuilding;
        Setup();
        buildingSystem.AddGhostBeltToGrid(origin, this);
        GridSetupDone();
    }

    public void ResetWorldItem() {
        endItem = null;
    }

    public bool TrySetWorldItem(WorldItem worldItem) {
        if(startItem == null) {
            startItem = worldItem;
            return true;
        }
        return false;
    }

    public void SetWorldItem(WorldItem worldItem) {
        startItem = worldItem;
    }

    public bool TakeActionOnFirstLoopedBelt(out bool didMovedItem, ConveyorBelt nextBelt) {
        bool hasItem = endItem != null;
        bool feedback = TakeActionOnEndItemWithFeedback(nextBelt);
        didMovedItem = hasItem != (endItem != null);
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
        if(endItem == null) return false;
        if(nextBelt == null) {
            nextBelt = buildingSystem.GetGridObject(nextPosition).placedObject as ConveyorBelt;
            if(nextBelt == null) return false;
        }
        if(!nextBelt.TrySetWorldItem(endItem)) return true;
        MoveEndItem(nextBelt);
        endItem = null;
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
        if(endItem == null) return;
        if(nextBelt == null) {
            nextBelt = buildingSystem.GetGridObject(nextPosition).placedObject as ConveyorBelt;
            if(nextBelt == null) return;
        }
        if(!nextBelt.TrySetWorldItem(endItem)) return;
        MoveEndItem(nextBelt);
        endItem = null;
    }

    void TakeActionOnStartItem() {
        if(startItem == null || endItem != null) return;
        MoveStartItem();
        endItem = startItem;
        startItem = null;
    }

    public bool TakeActionOnFirstBeltAfterStop(ConveyorBelt nextBelt) {
        bool hasItem = endItem != null;
        TakeActionOnEndItem(nextBelt);
        bool movedItem = hasItem && endItem == null;
        TakeActionOnStartItem();
        return movedItem;
    }

    public void TakeActionOnLastRepeatBelt(ConveyorBelt nextBelt) {
        TakeActionOnEndItem(nextBelt);
    }

    void MoveStartItem() {
        Vector2 nextPosition = CalculateNextPositionForStartItem();
        bool isCurved = false;

        switch(dir) {
            case BuildingDir.Down:
            case BuildingDir.Up:
                isCurved = startItem.transform.position.x != nextPosition.x;
                break;

            case BuildingDir.Left:
            case BuildingDir.Right:
                isCurved = startItem.transform.position.z != nextPosition.y;
                break;
        }

        if(isCurved) {
            startItem.MoveToPositionCurved(nextPosition);
        } else {
            startItem.MoveToPosition(nextPosition);
        }
    }

    void MoveEndItem(ConveyorBelt nextBelt) {
        endItem.MoveToPosition(CalculateNextPositionForEndItem(nextBelt));
    }

    Vector2 CalculateNextPositionForStartItem() {
        switch(dir) {
            default:
            case BuildingDir.Down: return origin + new Vector2(0.5f, 0.25f);
            case BuildingDir.Left: return origin + new Vector2(0.25f, 0.5f);
            case BuildingDir.Up: return origin + new Vector2(0.5f, 0.75f);
            case BuildingDir.Right: return origin + new Vector2(0.75f, 0.5f);
        }
    }

    Vector2 CalculateNextPositionForEndItem(ConveyorBelt nextBelt) {
        switch(dir) {
            default:
            case BuildingDir.Down: return nextBelt.origin + new Vector2(0.5f, 0.75f);
            case BuildingDir.Left: return nextBelt.origin + new Vector2(0.75f, 0.5f);
            case BuildingDir.Up: return nextBelt.origin + new Vector2(0.5f, 0.25f);
            case BuildingDir.Right: return nextBelt.origin + new Vector2(0.25f, 0.5f);
        }
    }
}