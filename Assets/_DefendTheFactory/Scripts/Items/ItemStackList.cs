using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStackList {


    private List<ItemStack> itemStackList;

    public ItemStackList() {
        itemStackList = new List<ItemStack>();
    }

    public override string ToString() {
        string str = "";
        foreach (ItemStack itemStack in itemStackList) {
            str += "O: " + itemStack.itemSO.itemName + "x" + itemStack.amount;
            str += "\n";
        }
        return str;
    }

    public List<ItemStack> GetItemStackList() {
        return itemStackList;
    }

    public int GetItemStoredCount(ItemSO filterItemSO) {
        int amount = 0;
        foreach (ItemStack itemStack in itemStackList) {
            if (filterItemSO == GameAssets.i.itemSO_Refs.any || filterItemSO == itemStack.itemSO) {
                amount += itemStack.amount;
            }
        }
        return amount;
    }

    public bool CanAddItemToItemStack(ItemSO itemSO, int amount = 1) {
        ItemStack itemStack = GetItemStackWithItemType(itemSO);
        if (itemStack != null) {
            // Stack already exists, has space?
            if (itemStack.amount + amount <= itemSO.maxStackAmount) {
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

    public void AddItemToItemStack(ItemSO itemSO, int amount = 1) {
        ItemStack itemStack = GetItemStackWithItemType(itemSO);
        if (itemStack != null) {
            itemStack.amount += amount;
        } else {
            itemStack = new ItemStack { itemSO = itemSO, amount = amount };
            itemStackList.Add(itemStack);
        }
    }

    public ItemStack GetItemStackWithItemType(ItemSO itemSO) {
        foreach (ItemStack itemStack in itemStackList) {
            if (itemStack.itemSO == itemSO) {
                return itemStack;
            }
        }
        return null;
    }

    public ItemStack GetFirstItemStackWithFilter(ItemSO[] filterItemSO) {
        foreach (ItemSO itemSO in filterItemSO) {
            foreach (ItemStack itemStack in itemStackList) {
                if (itemStack.itemSO == itemSO) {
                    return itemStack;
                }
            }
        }
        return null;
    }


}
