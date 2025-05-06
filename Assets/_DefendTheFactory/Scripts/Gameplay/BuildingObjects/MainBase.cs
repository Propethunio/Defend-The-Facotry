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
		int inputBeltsCount = buildableDataSO.inputBeltPositions.Count;

		for (int i = 0; i < inputBeltsCount; i++) {
			ConveyorBelt belt = gameObject.AddComponent<ConveyorBelt>();
			Vector2Int beltPos = buildableDataSO.GetMachineBeltPosition(origin, buildableDataSO.inputBeltPositions[i].beltPosition, dir);
			belt.SetupBuildingBelt(beltPos, GetRotatedBeltDir(buildableDataSO.inputBeltPositions[i].beltDir), this);
			inputBelts.Add(belt);
		}

		beltsCount = inputBelts.Count;
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

	private BuildingDir GetRotatedBeltDir(BuildingDir beltDir) {
		switch (dir) {
			default:
			case BuildingDir.Up: return beltDir;
			case BuildingDir.Left: return RotateDirectionClockwise(beltDir, 3);
			case BuildingDir.Down: return RotateDirectionClockwise(beltDir, 2);
			case BuildingDir.Right: return RotateDirectionClockwise(beltDir, 1);
		}
	}

	private BuildingDir RotateDirectionClockwise(BuildingDir dir, int steps) {
		int newDir = ((int)dir + steps) % 4;
		return (BuildingDir)newDir;
	}
}