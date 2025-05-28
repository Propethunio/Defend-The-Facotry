using System;
using System.Collections;
using UnityEngine;

public class AoeTower : BaseTower<AoeTowerSO> {
    protected override void Initialize(Vector2Int origin, BuildingDir dir, AoeTowerSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    public override void GridSetupDone() {
        base.GridSetupDone();
    }
    protected override IEnumerator AttackCycle() {
        throw new NotImplementedException();
    }

    protected override void Attack() {
        throw new NotImplementedException();
    }
}