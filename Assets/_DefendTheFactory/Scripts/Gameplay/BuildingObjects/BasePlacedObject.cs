using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlacedObject : MonoBehaviour, IReactOnMouse {
    [HideInInspector] public BuildingDir dir;
    public Vector2Int origin { get; protected set; }

    public abstract List<Vector2Int> GetGridPositionList();
    public abstract void SetData(Vector2Int origin, BuildingDir dir, BaseBuildableObjectSO placedObjectDataSO);
    protected virtual void Setup() { }
    public virtual void GridSetupDone() { }
    public virtual void DestroySelf() => Destroy(gameObject);
    public abstract bool ShouldHighlight();
    public virtual void MouseEnterObject() { }
    public virtual void MouseExitObject() { }
    public virtual void MouseLeftClickObject() { }
    public virtual void MouseScrollClickObject() { }
}