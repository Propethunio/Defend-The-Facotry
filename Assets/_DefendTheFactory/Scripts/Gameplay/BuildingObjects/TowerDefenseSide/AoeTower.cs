using System;
using System.Collections.Generic;
using UnityEngine;

public class AoeTower : BaseTower<AoeTowerSO> {
    protected override void Initialize(Vector2Int origin, BuildingDir dir, AoeTowerSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    public override void GridSetupDone() {
        base.GridSetupDone();
    }

    protected override void Attack() {
        throw new NotImplementedException();
    }
}