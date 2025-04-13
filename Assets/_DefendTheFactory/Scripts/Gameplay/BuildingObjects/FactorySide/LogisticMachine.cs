using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class LogisticMachine<T> : BaseDataPlacedObject<T>, IItemProvider where T : BaseBuildableObjectSO {
    public event Action OnDestroyed;

    protected GridCell[,] gridArray;
    protected List<WorldItem> items = new();
    protected LogisticDir logisticDir = LogisticDir.Straight;
    protected Dictionary<Action, Vector2Int> objectChangedEvents = new();

    protected override void Initialize(Vector2Int origin, BuildingDir dir, T buildableDataSO) { }

    private void OnDestroy() {
        OnDestroyed?.Invoke();
        OnDestroyed = null;
        Unsubscribe();
    }

    public override void GridSetupDone() {
        gridArray = Injector.Resolve<BuildingSystem>().grid.gridArray;
        Subscribe();
    }

    public override void DestroySelf() {
        int indexCount = items.Count;

        for (int i = 0; i < indexCount; i++) {
            items[i].DestroySelf();
        }

        base.DestroySelf();
    }

    private void Subscribe() {
        TimeTickSystem timeTickSystem = Injector.Resolve<TimeTickSystem>();
        timeTickSystem.OnEarlyTick += OnEarlyTick;
        timeTickSystem.OnLateTick += OnLateTick;
    }

    private void Unsubscribe() {
        TimeTickSystem timeTickSystem = Injector.Resolve<TimeTickSystem>();
        timeTickSystem.OnEarlyTick -= OnEarlyTick;
        timeTickSystem.OnLateTick -= OnLateTick;

        foreach (KeyValuePair<Action, Vector2Int> kvp in objectChangedEvents) {
            gridArray[kvp.Value.x, kvp.Value.y].ObjectChanged -= kvp.Key;
        }
    }

    protected abstract void OnEarlyTick();
    protected abstract void OnLateTick();

    protected LogisticDir GetNextDir(LogisticDir dir) {
        switch (dir) {
            default:
            case LogisticDir.Straight: return LogisticDir.Left;
            case LogisticDir.Left: return LogisticDir.Right;
            case LogisticDir.Right: return LogisticDir.Straight;
        }
    }

    protected bool ShouldSnap(Vector2Int position, out IItemProvider itemProvider) {
        itemProvider = null;

        if (!IsPositionValid(position)) return false;

        itemProvider = gridArray[position.x, position.y].placedObject as IItemProvider;

        return itemProvider != null && itemProvider.ShouldSnapWithLogisticMachine(origin);
    }

    protected bool ShouldSnapBack(Vector2Int position, out ConveyorBelt belt) {
        belt = null;

        if (!IsPositionValid(position)) return false;

        belt = gridArray[position.x, position.y].placedObject as ConveyorBelt;
        return belt != null && belt.previousPosition == origin;
    }

    protected bool IsPositionValid(Vector2Int position) {
        return position.x >= 0 && position.x < gridArray.GetLength(0) && position.y >= 0 && position.y < gridArray.GetLength(1);
    }

    public abstract bool IsOnOutputCell(Vector2Int position);

    public bool HasItem() {
        return items.Count > 0;
    }

    public WorldItem GetWorldItem() {
        WorldItem worldItem = items[0];
        items.RemoveAt(0);
        return worldItem;
    }

    public BuildingDir GetDir() {
        return dir;
    }

    public abstract bool ShouldSnapWithLogisticMachine(Vector2Int logisticMachineOrigin);

    protected Vector2 CalculatePositionInsideMachine(BuildingDir dir) {
        switch (dir) {
            default:
            case BuildingDir.Down: return origin + new Vector2(0.5f, 0.75f);
            case BuildingDir.Left: return origin + new Vector2(0.75f, 0.5f);
            case BuildingDir.Up: return origin + new Vector2(0.5f, 0.25f);
            case BuildingDir.Right: return origin + new Vector2(0.25f, 0.5f);
        }
    }

    protected Vector2 CalculatePositionOnBelt(WorldItem item, ConveyorBelt belt) {
        BuildingDir calculatedDir = CalculateDirection(belt);
        SetupLeavingTransform(item, calculatedDir);

        switch (calculatedDir) {
            default:
            case BuildingDir.Down: return belt.origin + new Vector2(0.5f, 0.25f);
            case BuildingDir.Left: return belt.origin + new Vector2(0.25f, 0.5f);
            case BuildingDir.Up: return belt.origin + new Vector2(0.5f, 0.75f);
            case BuildingDir.Right: return belt.origin + new Vector2(0.75f, 0.5f);
        }
    }

    private void SetupLeavingTransform(WorldItem item, BuildingDir dir) {
        Vector2 calculatedPosition = CalculatePositionFromDirection(dir);
        item.transform.position = new Vector3(calculatedPosition.x, item.transform.position.y, calculatedPosition.y);
        item.transform.rotation = GetRotationFromDirection(dir);
    }

    private BuildingDir CalculateDirection(ConveyorBelt belt) {
        Vector2Int diff = origin - belt.origin;

        if (diff == Vector2Int.up) return BuildingDir.Up;
        if (diff == Vector2Int.down) return BuildingDir.Down;

        return diff == Vector2Int.left ? BuildingDir.Left : BuildingDir.Right;
    }

    private Vector2 CalculatePositionFromDirection(BuildingDir dir) {
        switch (dir) {
            default:
            case BuildingDir.Down: return origin + new Vector2(0.5f, 0.75f);
            case BuildingDir.Left: return origin + new Vector2(0.75f, 0.5f);
            case BuildingDir.Up: return origin + new Vector2(0.5f, 0.25f);
            case BuildingDir.Right: return origin + new Vector2(0.25f, 0.5f);
        }
    }

    private Quaternion GetRotationFromDirection(BuildingDir dir) {
        switch (dir) {
            default:
            case BuildingDir.Up: return Quaternion.Euler(0, 0, 0);
            case BuildingDir.Right: return Quaternion.Euler(0, 270, 0);
            case BuildingDir.Down: return Quaternion.Euler(0, 180, 0);
            case BuildingDir.Left: return Quaternion.Euler(0, 90, 0);
        }
    }
}