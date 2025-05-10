using System.Collections.Generic;
using UnityEngine;

public class MainBase : BaseDataPlacedObject<MainBaseSO> {
	private ItemsManager itemsManager;
	private List<ConveyorBelt> inputBelts = new List<ConveyorBelt>();
	private int beltsCount;

	protected override void Initialize(Vector2Int origin, BuildingDir dir, MainBaseSO buildableDataSO) {
		BaseDataSet(origin, dir, buildableDataSO);
	}

	private void OnDestroy() {
		Unsubscribe();
	}

	protected override void Setup() {
		itemsManager = Injector.Resolve<ItemsManager>();
	}

	public override void GridSetupDone() {
		SetupBelts();
		Subscribe();
	}

	private void Subscribe() {
		Injector.Resolve<TimeTickSystem>().OnEarlyTick += OnEarlyTick;
	}

	private void Unsubscribe() {
		Injector.Resolve<TimeTickSystem>().OnEarlyTick -= OnEarlyTick;
	}

	private void SetupBelts() {
        beltsCount = buildableDataSO.inputBeltPositions.Count;

		for (int i = 0; i < beltsCount; i++) {
			ConveyorBelt belt = gameObject.AddComponent<ConveyorBelt>();
			Vector2Int beltPos = buildableDataSO.GetMachineBeltPosition(origin, buildableDataSO.inputBeltPositions[i].beltPosition, dir);
			belt.SetupBuildingBelt(beltPos, GetRotatedBeltDir(buildableDataSO.inputBeltPositions[i].beltDir), this);
			inputBelts.Add(belt);
		}
	}

	private void OnEarlyTick() {
		for (int i = 0; i < beltsCount; i++) {
			TryGetItemFromInputBelt(inputBelts[i]);
		}
	}

	private void TryGetItemFromInputBelt(ConveyorBelt belt) {
		if (belt.endItem == null) return;

		itemsManager.AddItems(belt.endItem.itemSO, 1);
		belt.endItem.DestroySelf();
		belt.ResetWorldItem();
	}
}