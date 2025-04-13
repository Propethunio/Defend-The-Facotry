using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDataPlacedObject<T> : BasePlacedObject where T : BaseBuildableObjectSO {
    public T buildableDataSO { get; private set; }

    public static BasePlacedObject Create(Vector3 worldPosition, BuildingDir dir, T placedObjectDataSO) {
        return Instantiate(placedObjectDataSO.prefab, worldPosition, Quaternion.Euler(0, Injector.Resolve<BuildingSystem>().GetRotationAngle(dir), 0)).GetComponent<BasePlacedObject>();
    }

    protected abstract void Initialize(Vector2Int origin, BuildingDir dir, T placedObjectDataSO);

    public override void SetData(Vector2Int origin, BuildingDir dir, BaseBuildableObjectSO placedObjectDataSO) {
        if (placedObjectDataSO is T castedDataSO) {
            Initialize(origin, dir, castedDataSO);
        }
        else {
            Debug.LogError($"Invalid type passed to Initialize. Expected {typeof(T)} but got {placedObjectDataSO.GetType()}");
        }
    }

    protected void BaseDataSet(Vector2Int origin, BuildingDir dir, T placedObjectDataSO) {
        this.origin = origin;
        this.dir = dir;
        buildableDataSO = placedObjectDataSO;
        Setup();
    }

    protected virtual void TriggerGridObjectChanged() {
        foreach (Vector2Int gridPosition in GetGridPositionList()) {
            Injector.Resolve<BuildingSystem>().grid.TriggerGridObjectChanged(gridPosition.x, gridPosition.y);
        }
    }

    public override List<Vector2Int> GetGridPositionList() {
        return buildableDataSO.GetGridPositionList(origin, dir);
    }
}