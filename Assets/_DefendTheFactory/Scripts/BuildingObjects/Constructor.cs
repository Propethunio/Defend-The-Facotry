using UnityEngine;

public class Constructor : BaseDataPlacedObject<ConstructorSO> {
    private ConveyorBelt inputBelt;
    private ConveyorBelt outputBelt;
    private int storedInputItems;
    private int storedOutputItems;
    private float craftingProgress;

    public override void Initialize(Vector2Int origin, BuildingDir dir, ConstructorSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    private void Update() {
        if(storedInputItems < buildableDataSO.itemRecipeList[0].inputItemList[0].amount || storedOutputItems >= buildableDataSO.maxStoredOutputItems) return;

        craftingProgress += Time.deltaTime;

        if(craftingProgress >= buildableDataSO.itemRecipeList[0].craftingTicks) {
            craftingProgress -= buildableDataSO.itemRecipeList[0].craftingTicks;
            storedOutputItems += buildableDataSO.itemRecipeList[0].outputItemList[0].amount;
            storedInputItems -= buildableDataSO.itemRecipeList[0].inputItemList[0].amount;
        }
    }

    private void OnDestroy() {
        Unsubscribe();
    }

    public override void GridSetupDone() {
        SetupBelts();
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
        TimeTickSystem.Instance.OnEarlyTick += OnEarlyTick;
    }

    private void Unsubscribe() {
        TimeTickSystem.Instance.OnEarlyTick -= OnEarlyTick;
    }

    private void OnEarlyTick() {
        TryGetItemFromInputBelt();
        TryPutItemOnOutputBelt();
    }

    private void TryGetItemFromInputBelt() {
        if(inputBelt.endItem == null || storedInputItems == buildableDataSO.maxStoredInputItems || inputBelt.endItem.itemSO != buildableDataSO.itemRecipeList[0].inputItemList[0].item) return;

        inputBelt.endItem.DestroySelf();
        storedInputItems++;
    }

    private void TryPutItemOnOutputBelt() {
        if(outputBelt.startItem != null || storedOutputItems == 0) return;

        WorldItem worldItem = WorldItem.Create(outputBelt.origin, dir, buildableDataSO.itemRecipeList[0].outputItemList[0].item);
        outputBelt.SetWorldItem(worldItem);
        storedOutputItems--;
    }
}