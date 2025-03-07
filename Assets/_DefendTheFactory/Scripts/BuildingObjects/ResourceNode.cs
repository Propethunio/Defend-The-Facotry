using System;
using UnityEngine;

public class ResourceNode : BaseDataPlacedObject<ResourceNodeSO> {

    public event Action<ResourceNode> NodeGatheredCompletly;

    private int amountLeft;

    public override void Initialize(Vector2Int origin, BuildingDir dir, ResourceNodeSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    public override void GridSetupDone() {
        base.GridSetupDone();
    }

    protected override void Setup() {
        amountLeft = buildableDataSO.amount;
    }

    public override void DestroySelf() {
        NodeGatheredCompletly?.Invoke(this);
        base.DestroySelf();
    }

    public void MineRsource() {
        amountLeft--;

        if(amountLeft == 0) {
            DestroySelf();
        }
    }
}