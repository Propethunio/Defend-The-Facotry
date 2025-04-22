using System;
using UnityEngine;

public class Constructor : BaseDataPlacedObject<ConstructorSO> {
	[SerializeField] private Transform modelTransform;

	public SimpleItemRecipeSO currentRecipe { get; private set; }
	public int storedInputItems { get; private set; }
	public int storedOutputItems { get; private set; }
	public int buildingLevel { get; private set; } = 1;

	private ConveyorBelt inputBelt;
	private ConveyorBelt outputBelt;
	private int maxStoredInputItems;
	private int productionTicks;
	private FlyweightFactory factory;

	public event Action<int> StoredInputItemsCountChanged, StoredOutputItemsCountChanged;
	public event Action<float> ProductionTicksChanged;
	public event Action BuildingUpgraded;

	protected override void Initialize(Vector2Int origin, BuildingDir dir, ConstructorSO buildableDataSO) {
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
		inputBelt.DestroySelf();
		outputBelt.DestroySelf();
		base.DestroySelf();
	}

	private void SetupBelts() {
		inputBelt = gameObject.AddComponent<ConveyorBelt>();
		outputBelt = gameObject.AddComponent<ConveyorBelt>();
		Vector2Int beltPos = buildableDataSO.GetMachineBeltPosition(origin, buildableDataSO.inputBeltPosition, dir);
		inputBelt.SetupBuildingBelt(beltPos, dir, this);
		beltPos = buildableDataSO.GetMachineBeltPosition(origin, buildableDataSO.outputBeltPosition, dir);
		outputBelt.SetupBuildingBelt(beltPos, dir, this);
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
		maxStoredInputItems = currentRecipe.inputItem.amount * 2;
		storedInputItems = 0;
		storedOutputItems = 0;
		productionTicks = 0;
	}

	private void OnMicroTick() {
		if (storedInputItems < currentRecipe.inputItem.amount || storedOutputItems > currentRecipe.outputItem.amount) return;

		productionTicks++;

		if (productionTicks == currentRecipe.craftingTicks) {
			productionTicks = 0;
			Craft();
		}

		ProductionTicksChanged?.Invoke(GetTargetProgressNormalized());
	}

	private void Craft() {
		storedInputItems -= currentRecipe.inputItem.amount;
		storedOutputItems += currentRecipe.outputItem.amount;
		StoredInputItemsCountChanged?.Invoke(storedInputItems);
		StoredOutputItemsCountChanged?.Invoke(storedOutputItems);
	}

	private void OnEarlyTick() {
		TryGetItemFromInputBelt();
		TryPutItemOnOutputBelt();
	}

	private void TryGetItemFromInputBelt() {
		if (inputBelt.endItem == null || storedInputItems == maxStoredInputItems || inputBelt.endItem.itemSO != currentRecipe.inputItem.item) return;

		inputBelt.endItem.DestroySelf();
		inputBelt.ResetWorldItem();
		storedInputItems++;
		StoredInputItemsCountChanged?.Invoke(storedInputItems);
	}

	private void TryPutItemOnOutputBelt() {
		if (outputBelt.startItem != null || storedOutputItems == 0) return;

		outputBelt.SetWorldItem(factory.CreateWorldItem(outputBelt.origin, dir, currentRecipe.outputItem.item));
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