using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assembler : PlacedObject, IItemStorage {

    public event EventHandler OnItemStorageCountChanged;


    private ItemRecipeSO itemRecipeSO;
    private List<ItemStack> inputItemStackList;
    private List<ItemStack> outputItemStackList;
    private float craftingProgress;

    protected override void Setup() {
        Debug.Log("Assembler.Setup()");
        inputItemStackList = new List<ItemStack>();
        outputItemStackList = new List<ItemStack>();
    }

    public override string ToString() {
        string str = "";
        foreach (ItemStack itemStack in inputItemStackList) {
            str += "I: " + itemStack.itemSO.itemName + "x" + itemStack.amount;
            str += "\n";
        }
        foreach (ItemStack itemStack in outputItemStackList) {
            str += "O: " + itemStack.itemSO.itemName + "x" + itemStack.amount;
            str += "\n";
        }
        return str;
    }

    private void Update() {
        if (!HasItemRecipe()) return;

        if (HasEnoughItemsToCraft()) {
            craftingProgress += Time.deltaTime;

            if (craftingProgress >= itemRecipeSO.craftingEffort) {
                // Item crafting complete
                craftingProgress = 0f;

                // Add Crafted Output Items
                foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.outputItemList) {
                    AddItemToOutputItemStack(recipeItem.item, recipeItem.amount);
                }

                // Consume Input Items
                foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.inputItemList) {
                    ItemStack itemStack = GetInputItemStackWithItemType(recipeItem.item);
                    itemStack.amount -= recipeItem.amount;
                }

                OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
                TriggerGridObjectChanged();
            }
        }
    }

    public float GetCraftingProgressNormalized() {
        if (HasItemRecipe()) {
            return craftingProgress / itemRecipeSO.craftingEffort;
        } else {
            return 0f;
        }
    }

    public int GetItemStoredCount(ItemSO filterItemSO) {
        int amount = 0;
        foreach (ItemStack itemStack in outputItemStackList) {
            if (filterItemSO == GameAssets.i.itemSO_Refs.any || filterItemSO == itemStack.itemSO) {
                amount += itemStack.amount;
            }
        }
        foreach (ItemStack itemStack in inputItemStackList) {
            if (filterItemSO == GameAssets.i.itemSO_Refs.any || filterItemSO == itemStack.itemSO) {
                amount += itemStack.amount;
            }
        }
        return amount;
    }

    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO) {
        if (!HasItemRecipe()) {
            itemSO = null;
            return false;
        }

        if (ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.any, filterItemSO) ||
            ItemSO.IsItemSOInFilter(itemRecipeSO.outputItemList[0].item, filterItemSO)) {
            // If filter matches any or filter matches this itemType
            ItemStack itemStack = GetOutputItemStackWithItemType(itemRecipeSO.outputItemList[0].item);
            if (itemStack != null) {
                if (itemStack.amount > 0) {
                    itemStack.amount -= 1;
                    itemSO = itemStack.itemSO;
                    OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
                    TriggerGridObjectChanged();
                    return true;
                } else {
                    itemSO = null;
                    return false;
                }
            } else {
                itemSO = null;
                return false;
            }
        } else {
            itemSO = null;
            return false;
        }
    }

    public ItemSO[] GetItemSOThatCanStore() {
        if (!HasItemRecipe()) return new ItemSO[] { GameAssets.i.itemSO_Refs.none };

        List<ItemSO> canStoreItemSOList = new List<ItemSO>();
        foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.inputItemList) {
            canStoreItemSOList.Add(recipeItem.item);
        }

        return canStoreItemSOList.ToArray();
    }

    public bool TryStoreItem(ItemSO itemSO) {
        if (!HasItemRecipe()) return false;

        foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.inputItemList) {
            if (itemSO == recipeItem.item) {
                // Can add item to input stack?
                if (CanAddItemToInputStack(itemSO)) {
                    AddItemToInputItemStack(itemSO);
                    OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
                    TriggerGridObjectChanged();
                    return true;
                } else {
                    // It's this item but cannot fit in stack
                    return false;
                }
            }
        }
        return false;
    }

    private bool CanAddItemToInputStack(ItemSO itemSO, int amount = 1) {
        ItemStack itemStack = GetInputItemStackWithItemType(itemSO);
        if (itemStack != null) {
            // Stack already exists, has space?
            if (itemStack.amount + amount <= 200) {
                // Can add
                return true;
            } else {
                // Stack full
                return false;
            }
        } else {
            // No item stack exists, can add
            return true;
        }
    }

    private void AddItemToInputItemStack(ItemSO itemSO, int amount = 1) {
        ItemStack itemStack = GetInputItemStackWithItemType(itemSO);
        if (itemStack != null) {
            itemStack.amount += amount;
        } else {
            itemStack = new ItemStack { itemSO = itemSO, amount = amount };
            inputItemStackList.Add(itemStack);
        }
    }

    private ItemStack GetInputItemStackWithItemType(ItemSO itemSO) {
        foreach (ItemStack itemStack in inputItemStackList) {
            if (itemStack.itemSO == itemSO) {
                return itemStack;
            }
        }
        return null;
    }

    private void AddItemToOutputItemStack(ItemSO itemSO, int amount = 1) {
        ItemStack itemStack = GetOutputItemStackWithItemType(itemSO);
        if (itemStack != null) {
            itemStack.amount += amount;
        } else {
            itemStack = new ItemStack { itemSO = itemSO, amount = amount };
            outputItemStackList.Add(itemStack);
        }
    }

    private ItemStack GetOutputItemStackWithItemType(ItemSO itemSO) {
        foreach (ItemStack itemStack in outputItemStackList) {
            if (itemStack.itemSO == itemSO) {
                return itemStack;
            }
        }
        return null;
    }

    private bool HasEnoughItemsToCraft() {
        if (!HasItemRecipe()) return false;

        foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.inputItemList) {
            ItemStack itemStack = GetInputItemStackWithItemType(recipeItem.item);
            if (itemStack == null) {
                // There's no item stack with this item type
                return false;
            } else {
                if (itemStack.amount < recipeItem.amount) {
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
