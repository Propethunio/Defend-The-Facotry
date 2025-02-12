using System;
using UnityEngine;

public class Constructor : PlacedObject {

    public event EventHandler OnItemStorageCountChanged;

    [SerializeField] ConveyorBelt inputBelt;
    [SerializeField] ConveyorBelt outputBelt;
    [SerializeField] Vector2Int inputGhostBeltPosition;
    [SerializeField] Vector2Int outputGhostBeltPosition;
    [SerializeField] int maxStoredInputItems;
    [SerializeField] int maxStoredOutputItems;

    int storedInputItems;
    int storedOutputItems;

    [field: SerializeField] public ItemRecipeSO itemRecipeSO { get; private set; }
    ItemStackList inputItemStackList = new();
    ItemStackList outputItemStackList = new();
    float craftingProgress;

    void Update() {
        if(storedInputItems < itemRecipeSO.inputItemList[0].amount || storedOutputItems >= maxStoredOutputItems) return;

        craftingProgress += Time.deltaTime;

        if(craftingProgress >= itemRecipeSO.craftingTime) {
            craftingProgress -= itemRecipeSO.craftingTime;
            storedOutputItems += itemRecipeSO.outputItemList[0].amount;
            storedInputItems -= itemRecipeSO.inputItemList[0].amount;

            OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
            TriggerGridObjectChanged();
        }
    }

    void OnDestroy() {
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

    void SetupBelts() {
        Vector2Int beltPos = buildableDataSO.GetMachineBeltPosition(origin, inputGhostBeltPosition, dir);
        inputBelt.SetupBuildingBelt(beltPos, dir, this);
        beltPos = buildableDataSO.GetMachineBeltPosition(origin, outputGhostBeltPosition, dir);
        outputBelt.SetupBuildingBelt(beltPos, dir, this);
    }

    void Subscribe() {
        TimeTickSystem.Instance.OnEarlyTick += OnEarlyTick;
    }

    void Unsubscribe() {
        TimeTickSystem.Instance.OnEarlyTick -= OnEarlyTick;
    }

    void OnEarlyTick() {
        TryGetItemFromInputBelt();
        TryPutItemOnOutputBelt();
    }

    void TryGetItemFromInputBelt() {
        if(inputBelt.worldItem == null || storedInputItems == maxStoredInputItems || inputBelt.worldItem.itemSO != itemRecipeSO.inputItemList[0].item) return;

        inputBelt.worldItem.DestroySelf();
        storedInputItems++;
    }

    void TryPutItemOnOutputBelt() {
        if(outputBelt.worldItem != null || storedOutputItems == 0) return;

        WorldItem worldItem = WorldItem.Create(outputBelt.origin, itemRecipeSO.outputItemList[0].item);
        outputBelt.SetWorldItem(worldItem);
        storedOutputItems--;
    }

    public float GetCraftingProgressNormalized() {
        return craftingProgress / itemRecipeSO.craftingTime;
    }

    public int GetItemStoredCount(ItemSO filterItemSO) {
        int amount = 0;

        amount += outputItemStackList.GetItemStoredCount(filterItemSO);
        amount += inputItemStackList.GetItemStoredCount(filterItemSO);

        return amount;
    }

    public void SetItemRecipeScriptableObject(ItemRecipeSO itemRecipeSO) {
        this.itemRecipeSO = itemRecipeSO;
    }
}