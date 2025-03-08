using System;
using UnityEngine;

public class ResourceNode : BaseDataPlacedObject<ResourceNodeSO> {
    public event Action<ResourceNode> NodeGatheredCompletely;

    private int amountLeft;

    protected override void Initialize(Vector2Int origin, BuildingDir dir, ResourceNodeSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    protected override void Setup() {
        amountLeft = buildableDataSO.amount;
    }

    public override void DestroySelf() {
        NodeGatheredCompletely?.Invoke(this);
        base.DestroySelf();
    }

    public void MineResource() {
        amountLeft--;

        if (amountLeft == 0) {
            DestroySelf();
        }
    }
}