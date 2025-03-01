using System;
using System.Collections.Generic;
using UnityEngine;

public class Assembler : BaseDataPlacedObject<BaseMachineSO> {

    public event EventHandler OnItemStorageCountChanged;

    private ItemRecipeSO itemRecipeSO;
    private List<ItemStack> inputItemStackList;
    private List<ItemStack> outputItemStackList;
    private float craftingProgress;

    public override void Initialize(Vector2Int origin, BuildingDir dir, BaseMachineSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    protected override void Setup() {
        inputItemStackList = new List<ItemStack>();
        outputItemStackList = new List<ItemStack>();
    }

    private void Update() {
        if(!HasItemRecipe()) return;

        if(HasEnoughItemsToCraft()) {
            craftingProgress += Time.deltaTime;

            if(craftingProgress >= itemRecipeSO.craftingTime) {
                // Item crafting complete
                craftingProgress = 0f;

                // Add Crafted Output Items
                foreach(ItemIntPair recipeItem in itemRecipeSO.outputItemList) {
                    AddItemToOutputItemStack(recipeItem.item, recipeItem.amount);
                }

                // Consume Input Items
                foreach(ItemIntPair recipeItem in itemRecipeSO.inputItemList) {
                    ItemStack itemStack = GetInputItemStackWithItemType(recipeItem.item);
                    itemStack.amount -= recipeItem.amount;
                }

                OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
                TriggerGridObjectChanged();
            }
        }
    }

    public float GetCraftingProgressNormalized() {
        if(HasItemRecipe()) {
            return craftingProgress / itemRecipeSO.craftingTime;
        } else {
            return 0f;
        }
    }

    public int GetItemStoredCount(ItemSO filterItemSO) {
        int amount = 0;
        foreach(ItemStack itemStack in outputItemStackList) {
            if(filterItemSO == GameAssets.i.itemSO_Refs.any || filterItemSO == itemStack.itemSO) {
                amount += itemStack.amount;
            }
        }
        foreach(ItemStack itemStack in inputItemStackList) {
            if(filterItemSO == GameAssets.i.itemSO_Refs.any || filterItemSO == itemStack.itemSO) {
                amount += itemStack.amount;
            }
        }
        return amount;
    }

    private ItemStack GetInputItemStackWithItemType(ItemSO itemSO) {
        foreach(ItemStack itemStack in inputItemStackList) {
            if(itemStack.itemSO == itemSO) {
                return itemStack;
            }
        }
        return null;
    }

    private void AddItemToOutputItemStack(ItemSO itemSO, int amount = 1) {
        ItemStack itemStack = GetOutputItemStackWithItemType(itemSO);
        if(itemStack != null) {
            itemStack.amount += amount;
        } else {
            itemStack = new ItemStack { itemSO = itemSO, amount = amount };
            outputItemStackList.Add(itemStack);
        }
    }

    private ItemStack GetOutputItemStackWithItemType(ItemSO itemSO) {
        foreach(ItemStack itemStack in outputItemStackList) {
            if(itemStack.itemSO == itemSO) {
                return itemStack;
            }
        }
        return null;
    }

    private bool HasEnoughItemsToCraft() {
        if(!HasItemRecipe()) return false;

        foreach(ItemIntPair recipeItem in itemRecipeSO.inputItemList) {
            ItemStack itemStack = GetInputItemStackWithItemType(recipeItem.item);
            if(itemStack == null) {
                // There's no item stack with this item type
                return false;
            } else {
                if(itemStack.amount < recipeItem.amount) {
                    // Not enough amount of this item type
                    return false;
                }
            }
        }
        // Everything is here, ready to craft
        return true;
    }

    public bool HasItemRecipe() {
        return itemRecipeSO != null;
    }

    public ItemRecipeSO GetItemRecipeSO() {
        return itemRecipeSO;
    }

    public void SetItemRecipeScriptableObject(ItemRecipeSO itemRecipeSO) {
        this.itemRecipeSO = itemRecipeSO;
    }
}