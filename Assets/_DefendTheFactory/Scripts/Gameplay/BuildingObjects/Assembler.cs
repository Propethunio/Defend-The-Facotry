using System;
using System.Collections.Generic;
using UnityEngine;

public class Assembler : BaseDataPlacedObject<AssemblerSO> {
	[SerializeField] private Transform modelTransform;

	public ItemRecipeSO currentRecipe { get; private set; }
	public Dictionary<ItemSO, int> storedInputItems { get; private set; } = new Dictionary<ItemSO, int>();
	public int storedOutputItems { get; private set; }
	public int buildingLevel { get; private set; } = 2;

	private List<ConveyorBelt> inputBelts = new List<ConveyorBelt>();
	private ConveyorBelt outputBelt;
	private Dictionary<ItemSO, int> maxStoredInputItems = new Dictionary<ItemSO, int>();
	private int productionTicks;
	private FlyweightFactory factory;
	private int inputBeltsCount;

	public event Action<int> StoredInputItemsCountChanged, StoredOutputItemsCountChanged;
	public event Action<float> ProductionTicksChanged;
	public event Action BuildingUpgraded;

	protected override void Initialize(Vector2Int origin, BuildingDir dir, AssemblerSO buildableDataSO) {
		BaseDataSet(origin, dir, buildableDataSO);
		factory = Injector.Resolve<FlyweightFactory>();
	}

	private void OnDestroy() {
		Unsubscribe();
	}

	public override void GridSetupDone() {
		SetupBelts();
		SetupRecipe(0);
		Subscribe();
	}

	public override void DestroySelf() {
		for (int i = 0; i < inputBeltsCount; i++) {
			inputBelts[i].DestroySelf();
		}

		outputBelt.DestroySelf();
		base.DestroySelf();
	}

	private void SetupBelts() {
		outputBelt = gameObject.AddComponent<ConveyorBelt>();
		Vector2Int beltPos = buildableDataSO.GetMachineBeltPosition(origin, buildableDataSO.outputBeltPosition, dir);
		outputBelt.SetupBuildingBelt(beltPos, dir, this);
		inputBeltsCount = buildableDataSO.inputBeltPositions.Count;

		for (int i = 0; i < inputBeltsCount; i++) {
			ConveyorBelt belt = gameObject.AddComponent<ConveyorBelt>();
			beltPos = buildableDataSO.GetMachineBeltPosition(origin, buildableDataSO.inputBeltPositions[i].beltPosition, dir);
			belt.SetupBuildingBelt(beltPos, GetRotatedBeltDir(buildableDataSO.inputBeltPositions[i].beltDir), this);
			inputBelts.Add(belt);
		}
	}

	private void Subscribe() {
		TimeTickSystem timeTickSystem = Injector.Resolve<TimeTickSystem>();
		timeTickSystem.OnMicroTick += OnMicroTick;
		timeTickSystem.OnEarlyTick += OnEarlyTick;
	}

	private void Unsubscribe() {
		TimeTickSystem timeTickSystem = Injector.Resolve<TimeTickSystem>();
		timeTickSystem.OnMicroTick -= OnMicroTick;
		timeTickSystem.OnEarlyTick -= OnEarlyTick;
	}

	public void SetupRecipe(int index) {
		currentRecipe = buildableDataSO.itemRecipeList[index];
		maxStoredInputItems.Clear();
		storedInputItems.Clear();
		int inputCount = currentRecipe.inputItemList.Count;

		for (int i = 0; i < inputCount; i++) {
			ItemIntPair itemIntPair = currentRecipe.inputItemList[i];
			maxStoredInputItems.Add(itemIntPair.item, itemIntPair.amount * 2);
			storedInputItems.Add(itemIntPair.item, 0);
		}

		storedOutputItems = 0;
		productionTicks = 0;
	}

	private void OnMicroTick() {
		if (!EnoughInput() || storedOutputItems > currentRecipe.outputItemList[0].amount) return;

		productionTicks++;

		if (productionTicks == currentRecipe.craftingTicks) {
			productionTicks = 0;
			Craft();
		}

		ProductionTicksChanged?.Invoke(GetTargetProgressNormalized());
	}

	private bool EnoughInput() {
		for (int i = 0; i < currentRecipe.inputItemList.Count; i++) {
			if (storedInputItems[currentRecipe.inputItemList[i].item] < currentRecipe.inputItemList[i].amount) {
				return false;
			}
		}

		return true;
	}

	private void Craft() {
		for (int index = 0; index < currentRecipe.inputItemList.Count; index++) {
			ItemIntPair input = currentRecipe.inputItemList[index];
			storedInputItems[input.item] -= input.amount;
		}

		storedOutputItems += currentRecipe.outputItemList[0].amount;
		StoredInputItemsCountChanged?.Invoke(1);
		StoredOutputItemsCountChanged?.Invoke(storedOutputItems);
	}

	private void OnEarlyTick() {
		TryGetItemFromInputBelts();
		TryPutItemOnOutputBelt();
	}
	
	private void TryGetItemFromInputBelts()
	{
		for (int i = 0; i < inputBeltsCount; i++)
		{
			ConveyorBelt belt = inputBelts[i];

			if (belt.endItem == null) continue;

			ItemSO incomingItem = belt.endItem.itemSO;

			if (storedInputItems.ContainsKey(incomingItem) && storedInputItems[incomingItem] < maxStoredInputItems[incomingItem])
			{
				belt.endItem.DestroySelf();
				belt.ResetWorldItem();
				storedInputItems[incomingItem]++;
				StoredInputItemsCountChanged?.Invoke(1);
			}
		}
	}

	private void TryPutItemOnOutputBelt() {
		if (outputBelt.startItem != null || storedOutputItems == 0) return;

		outputBelt.SetWorldItem(factory.CreateWorldItem(outputBelt.origin, dir, currentRecipe.outputItemList[0].item));
		storedOutputItems--;
		StoredOutputItemsCountChanged?.Invoke(storedOutputItems);
	}

	public float GetTargetProgressNormalized() {
		return (float)(productionTicks + 1) / currentRecipe.craftingTicks;
	}

	public float GetProgressNormalized() {
		return (float)productionTicks / currentRecipe.craftingTicks;
	}

	public void UpgradeLvl() {
		Quaternion tmpQuaternion = modelTransform.rotation;
		Vector3 tmpPosition = modelTransform.position;
		Destroy(modelTransform.gameObject);
		Instantiate(buildableDataSO.upgradedModel, tmpPosition, tmpQuaternion, transform);
		buildingLevel++;
		BuildingUpgraded?.Invoke();
	}
}