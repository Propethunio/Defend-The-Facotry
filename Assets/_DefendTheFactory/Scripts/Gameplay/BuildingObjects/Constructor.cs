using UnityEngine;

public class Constructor : BaseDataPlacedObject<ConstructorSO> {
    private ConveyorBelt inputBelt;
    private ConveyorBelt outputBelt;
    private int storedInputItems;
    private int maxStoredInputItems;
    private int storedOutputItems;
    private ItemRecipeSO currentRecipe;
    private int productionTicks;

    protected override void Initialize(Vector2Int origin, BuildingDir dir, ConstructorSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
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
        TimeTickSystem.Instance.OnMicroTick += OnMicroTick;
        TimeTickSystem.Instance.OnEarlyTick += OnEarlyTick;
    }

    private void Unsubscribe() {
        TimeTickSystem.Instance.OnMicroTick -= OnMicroTick;
        TimeTickSystem.Instance.OnEarlyTick -= OnEarlyTick;
    }

    public void SetupRecipe(int index) {
        currentRecipe = buildableDataSO.itemRecipeList[index];
        maxStoredInputItems = currentRecipe.inputItemList[0].amount * 2;
        storedInputItems = 0;
        storedOutputItems = 0;
    }

    private void OnMicroTick() {
        if (storedInputItems < currentRecipe.inputItemList[0].amount || storedOutputItems > currentRecipe.outputItemList[0].amount) return;

        productionTicks++;

        if (productionTicks != currentRecipe.craftingTicks) return;

        productionTicks = 0;
        Craft();
    }

    private void Craft() {
        storedInputItems -= currentRecipe.inputItemList[0].amount;
        storedOutputItems += currentRecipe.outputItemList[0].amount;
    }

    private void OnEarlyTick() {
        TryGetItemFromInputBelt();
        TryPutItemOnOutputBelt();
    }

    private void TryGetItemFromInputBelt() {
        if (inputBelt.endItem == null || storedInputItems == maxStoredInputItems || inputBelt.endItem.itemSO != currentRecipe.inputItemList[0].item) return;

        inputBelt.endItem.DestroySelf();
        storedInputItems++;
    }

    private void TryPutItemOnOutputBelt() {
        if (outputBelt.startItem != null || storedOutputItems == 0) return;

        WorldItem worldItem = WorldItem.Create(outputBelt.origin, dir, currentRecipe.outputItemList[0].item);
        outputBelt.SetWorldItem(worldItem);
        storedOutputItems--;
    }
}