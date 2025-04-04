using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class LogisticMachine<T> : BaseDataPlacedObject<T> where T : BaseBuildableObjectSO {
    public event Action OnDestroyed;
    
    [SerializeField] protected int maxStorage;

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
        gridArray = BuildingSystem.Instance.grid.gridArray;
        Subscribe();
    }

    public override void DestroySelf() {
        foreach (WorldItem item in items) {
            item.DestroySelf();
        }

        base.DestroySelf();
    }

    private void Subscribe() {
        TimeTickSystem.Instance.OnEarlyTick += OnEarlyTick;
        TimeTickSystem.Instance.OnLateTick += OnLateTick;
    }

    private void Unsubscribe() {
        TimeTickSystem.Instance.OnEarlyTick -= OnEarlyTick;
        TimeTickSystem.Instance.OnLateTick -= OnLateTick;

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

    protected bool ShouldSnap(Vector2Int position, out ConveyorBelt belt) {
        belt = null;

        if (!IsPositionValid(position)) return false;

        belt = gridArray[position.x, position.y].placedObject as ConveyorBelt;
        return belt != null && belt.nextPosition == origin;
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
}