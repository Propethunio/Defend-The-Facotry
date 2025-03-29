using System;
using UnityEngine;

public class ResourceNode : BaseDataPlacedObject<ResourceNodeSO> {
    public event Action<ResourceNode> NodeGatheredCompletely;

    private HoverOutline outline;
    private int amountLeft;
    private int clicksLeft;

    private void Start() {
        outline = GetComponent<HoverOutline>();
    }

    protected override void Initialize(Vector2Int origin, BuildingDir dir, ResourceNodeSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    protected override void Setup() {
        amountLeft = buildableDataSO.amount;
        clicksLeft = buildableDataSO.clicksToGather;
    }

    public override void DestroySelf() {
        NodeGatheredCompletely?.Invoke(this);
        base.DestroySelf();
    }

    public override void MouseEnterObject() {
        outline.SetOutline(true);
    }

    public override void MouseExitObject() {
        outline.SetOutline(false);
    }

    public override void MouseLeftClickObject() {
        ClickResource();
    }

    public void MineResource() {
        amountLeft--;

        if (amountLeft == 0) {
            DestroySelf();
        }
    }

    private void ClickResource() {
        clicksLeft--;

        if (clicksLeft != 0) return;

        clicksLeft = buildableDataSO.clicksToGather;
        ItemsManager.Instance.AddItems(buildableDataSO.itemGatheredOnClick, 1);
        MineResource();
    }
}