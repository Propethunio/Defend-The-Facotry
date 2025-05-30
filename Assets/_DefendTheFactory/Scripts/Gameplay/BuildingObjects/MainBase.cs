using System.Collections.Generic;
using UnityEngine;

public class MainBase : BaseDataPlacedObject<MainBaseSO> {
	[SerializeField] private GameObject visual;

	private ItemsManager itemsManager;
	private List<ConveyorBelt> inputBelts = new List<ConveyorBelt>();
	private int beltsCount;

	public int level { get; private set; }

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

	public void UpgradeLevel() {
		if(!CanUpgrade()) return;
		
		for (int index = 0; index < buildableDataSO.UpgradesList[level].cost.Count; index++) {
			ItemIntPair cost = buildableDataSO.UpgradesList[level].cost[index];
			itemsManager.RemoveItems(cost.item, cost.amount);
		}

		level++;

		if (level > buildableDataSO.UpgradesList.Count) {
			//WIN
			Debug.Log("WIN!!!!!");
		}
		else {
			GameObject newVis = Instantiate(buildableDataSO.UpgradesList[level - 1].upgradePrefab, visual.transform.position, visual.transform.rotation);
			Destroy(visual);
			visual = newVis;
		}
	}

	private bool CanUpgrade() {
		for (int index = 0; index < buildableDataSO.UpgradesList[level].cost.Count; index++) {
			ItemIntPair cost = buildableDataSO.UpgradesList[level].cost[index];
			if (!itemsManager.CanAfford(cost.item, cost.amount)) return false;
		}
		
		return true;
	}
}