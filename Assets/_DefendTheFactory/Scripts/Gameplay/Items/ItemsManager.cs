using System;
using System.Collections.Generic;

public class ItemsManager {
    public event Action<ItemSO> ItemCreated, ItemRemoved;
    public event Action<ItemSO, int> ItemChanged;

    private Dictionary<ItemSO, int> itemsAmounts { get; set; } = new Dictionary<ItemSO, int>();

    public void AddItems(ItemSO item, int amount) {
        if (!itemsAmounts.TryAdd(item, amount)) {
            itemsAmounts[item] += amount;
        }
        else {
            ItemCreated?.Invoke(item);
        }
        
        ItemChanged?.Invoke(item, itemsAmounts[item]);
    }

    public bool CanAfford(ItemSO item, int amount) {
        return itemsAmounts.ContainsKey(item) && itemsAmounts[item] >= amount;
    }

    public void RemoveItems(ItemSO item, int amount) {
        itemsAmounts[item] -= amount;
        ItemChanged?.Invoke(item, itemsAmounts[item]);

        if (itemsAmounts[item] != 0) return;
        itemsAmounts.Remove(item);
        ItemRemoved?.Invoke(item);
    }

    public int GetAmount(ItemSO item) {
        return itemsAmounts.GetValueOrDefault(item, 0);
    }
}