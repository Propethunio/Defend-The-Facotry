using System;
using System.Collections.Generic;

public class ItemsManager {
    public event Action<ItemSO> ItemCreated;
    public event Action<ItemSO, int> ItemAdded;
    public event Action<ItemSO, int> ItemRemoved;
    public event Action<ItemSO, int> ItemChanged;

    private Dictionary<ItemSO, int> itemsAmounts { get; set; } = new();

    public void AddItems(ItemSO item, int amount) {
        if (!itemsAmounts.TryAdd(item, amount)) {
            itemsAmounts[item] += amount;
        }
        else {
            ItemCreated?.Invoke(item);
        }

        ItemAdded?.Invoke(item, itemsAmounts[item]);
        ItemChanged?.Invoke(item, itemsAmounts[item]);
    }

    public bool CanAfford(ItemSO item, int amount) {
        return itemsAmounts.ContainsKey(item) && itemsAmounts[item] >= amount;
    }

    public void RemoveItems(ItemSO item, int amount) {
        itemsAmounts[item] -= amount;
        ItemRemoved?.Invoke(item, itemsAmounts[item]);
        ItemChanged?.Invoke(item, itemsAmounts[item]);
    }

    public int GetAmount(ItemSO item) {
        return itemsAmounts.GetValueOrDefault(item, 0);
    }
}