using System;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceNode : BaseDataPlacedObject<ResourceNodeSO> {
    public event Action<ResourceNode> NodeGatheredCompletely;

    [SerializeField] private Transform model;
    [SerializeField] private float minScaleFactor;
    [SerializeField] private float maxScaleFactor;
    [SerializeField] private MMFeedbacks onClickFeedback;

    public int amountLeft { get; private set; }
    private int clicksLeft;

    public event Action ResourcesGathered; 
    
    public override void Start() {
        base.Start();
        model.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        float randomScale = Random.Range(minScaleFactor, maxScaleFactor);
        model.localScale *= randomScale;
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

    public override void MouseLeftClickObject() {
        onClickFeedback.PlayFeedbacks();
        ClickResource();
    }

    public void MineResource() {
        amountLeft--;
        ResourcesGathered?.Invoke();

        if (amountLeft == 0) {
            DestroySelf();
        }
    }

    private void ClickResource() {
        clicksLeft--;

        if (clicksLeft != 0) return;

        clicksLeft = buildableDataSO.clicksToGather;
        Injector.Resolve<ItemsManager>().AddItems(buildableDataSO.itemGatheredOnClick, 1);
        MineResource();
    }
}