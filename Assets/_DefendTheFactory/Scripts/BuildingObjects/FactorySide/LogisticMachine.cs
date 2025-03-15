using UnityEngine;

public class LogisticMachine : PlacedObject {

    public int maxStorage;
    public int currentStorage;
    public BuildingSystem buildingSystem;
    [SerializeField] ConveyorBelt ghostBelt;

    void OnDestroy() {
        Unsubscribe();
    }

    public virtual void Subscribe() {
        TimeTickSystem.Instance.OnEarlyTick += OnEarlyTick;
        TimeTickSystem.Instance.OnTick += OnTick;
        TimeTickSystem.Instance.OnLateTick += OnLateTick;
    }

    public virtual void Unsubscribe() {
        TimeTickSystem.Instance.OnEarlyTick -= OnEarlyTick;
        TimeTickSystem.Instance.OnTick -= OnTick;
        TimeTickSystem.Instance.OnLateTick -= OnLateTick;
    }

    public virtual void OnEarlyTick() { }
    public virtual void OnTick() { }
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