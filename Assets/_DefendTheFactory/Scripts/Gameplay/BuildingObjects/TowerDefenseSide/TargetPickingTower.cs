using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetPickingTower : BaseTower<TargetPickingTowerSO> {
    protected override void Initialize(Vector2Int origin, BuildingDir dir, TargetPickingTowerSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    public override void GridSetupDone() {
        base.GridSetupDone();
    }
    
    protected override void Attack() {
        enemiesInRange[0].DamageMe(buildableDataSO.damage);
    }
}