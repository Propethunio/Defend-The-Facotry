using System.Collections.Generic;
using UnityEngine;

public class MainBase : BaseDataPlacedObject<MainBaseSO> {

    ItemsManager itemsManager;
    List<ConveyorBelt> inputBelts = new();
    int beltsCount;

    public override void Initialize(Vector2Int origin, BuildingDir dir, MainBaseSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    void OnDestroy() {
        Unsubscribe();
    }

    protected override void Setup() {
        itemsManager = ItemsManager.Instance;
    }

    public override void GridSetupDone() {
        SetupBelts();
        Subscribe();
    }

    void Subscribe() {
        TimeTickSystem.Instance.OnEarlyTick += OnEarlyTick;
    }

    void Unsubscribe() {
        TimeTickSystem.Instance.OnEarlyTick -= OnEarlyTick;
    }

    void SetupBelts() {
        int inputBeltsCount = buildableDataSO.inputBeltPositions.Count;

        for(int i = 0; i < inputBeltsCount; i++) {
            ConveyorBelt belt = gameObject.AddComponent<ConveyorBelt>();
            Vector2Int beltPos = buildableDataSO.GetMachineBeltPosition(origin, buildableDataSO.inputBeltPositions[i].beltPosition, dir);
            belt.SetupBuildingBelt(beltPos, buildableDataSO.inputBeltPositions[i].beltDir, this);
            inputBelts.Add(belt);
        }

        beltsCount = inputBelts.Count;
    }

    void OnEarlyTick() {
        for(int i = 0; i < beltsCount; i++) {
            TryGetItemFromInputBelt(inputBelts[i]);
        }
    }

    void TryGetItemFromInputBelt(ConveyorBelt belt) {
        if(belt.endItem == null) return;

        itemsManager.AddItems(belt.endItem.itemSO, 1);
        belt.endItem.DestroySelf();
        belt.ResetWorldItem();
    }
}