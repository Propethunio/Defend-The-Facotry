using UnityEngine;

public class Portal : BaseDataPlacedObject<PortalSO> {
    protected override void Initialize(Vector2Int origin, BuildingDir dir, PortalSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }
}