using System.Collections.Generic;

public class LogisticMachine : PlacedObject {

    public int maxStorage;
    public int currentStorage;
    public BuildingSystem buildingSystem;
    protected List<WorldItem> items = new();

    void OnDestroy() {
        Unsubscribe();
    }

    public override void GridSetupDone() {
        buildingSystem = BuildingSystem.Instance;
        Subscribe();
    }

    public override void DestroySelf() {
        foreach(WorldItem item in items) {
            item.DestroySelf();
        }
        base.DestroySelf();
    }

    public virtual void Subscribe() {
        TimeTickSystem.Instance.OnEarlyTick += OnEarlyTick;
        TimeTickSystem.Instance.OnLateTick += OnLateTick;
    }

    public virtual void Unsubscribe() {
        TimeTickSystem.Instance.OnEarlyTick -= OnEarlyTick;
        TimeTickSystem.Instance.OnLateTick -= OnLateTick;
    }

    public virtual void OnEarlyTick() { }

    public virtual void OnLateTick() { }

    public virtual LogisticDir GetNextDir(LogisticDir dir) {
        switch(dir) {
            default:
            case LogisticDir.Straight: return LogisticDir.Right;
            case LogisticDir.Right: return LogisticDir.Left;
            case LogisticDir.Left: return LogisticDir.Straight;
        }
    }
}