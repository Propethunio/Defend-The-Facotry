using UnityEngine;

public class ConveyorBelt : PlacedObject, IWorldItemSlot {

    [SerializeField] GameObject straightBeltVisual;
    [SerializeField] GameObject leftTurnVisual;
    [SerializeField] GameObject rightTurnVisual;

    [HideInInspector] public Vector2Int previousPosition;
    public Vector2Int nextPosition { get; private set; }
    public WorldItem worldItem { get; private set; }
    public bool isPartOfBuilding { get; private set; }

    BuildingSystem buildingSystem;

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
        SetUpVisual();
    }

    public void SetupBuildingBelt(Vector2Int origin, Dir dir) {
        isPartOfBuilding = true;
        this.origin = origin;
        this.dir = dir;
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
        IWorldItemSlot worldItemSlot = buildingSystem.GetGridObject(nextPosition).placedObject as IWorldItemSlot;
        if(worldItemSlot == null) return false;
        if(!worldItemSlot.TrySetWorldItem(worldItem)) return true;
        worldItem.MoveToGridPosition(worldItemSlot.GetGridPosition());
        worldItem.SetHasAlreadyMoved();
        worldItem = null;
        return false;
    }

    public bool TrySetWorldItem(WorldItem worldItem) {
        if(this.worldItem == null) {
            this.worldItem = worldItem;
            return true;
        } else {
            return false;
        }
    }

    public override void DestroySelf() {
        if(worldItem != null) {
            worldItem.DestroySelf();
        }

        base.DestroySelf();
    }

    public bool TryGetWorldItem(ItemSO[] filterItemSO, out WorldItem worldItem) {
        if(this.worldItem != null && (ItemSO.IsItemSOInFilter(this.worldItem.GetItemSO(), filterItemSO) || ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.any, filterItemSO))) {
            worldItem = this.worldItem;
            this.worldItem = null;
            return true;
        } else {
            worldItem = null;
            return false;
        }
    }

    public ItemSO[] GetItemSOThatCanStore() {
        return new ItemSO[] { GameAssets.i.itemSO_Refs.any };
    }

    void SetUpVisual() {
        Vector2Int forwardVector = buildingSystem.GetDirForwardVector(dir);
        Vector2Int backPosition = origin - forwardVector;

        if(previousPosition == backPosition) return;

        Vector2Int rightVector = new Vector2Int(forwardVector.y, -forwardVector.x);
        Vector2Int leftPosition = origin - rightVector;

        if(previousPosition == leftPosition) {
            ShowLeftVisual();
        } else {
            ShowRightVisual();
        }
    }

    public void ShowStraightVisual() {
        straightBeltVisual.SetActive(true);
        leftTurnVisual.SetActive(false);
        rightTurnVisual.SetActive(false);
    }

    public void ShowLeftVisual() {
        straightBeltVisual.SetActive(false);
        leftTurnVisual.SetActive(true);
        rightTurnVisual.SetActive(false);
    }

    public void ShowRightVisual() {
        straightBeltVisual.SetActive(false);
        leftTurnVisual.SetActive(false);
        rightTurnVisual.SetActive(true);
    }
}