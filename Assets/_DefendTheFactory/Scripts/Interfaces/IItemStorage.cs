using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemStorage {

    event EventHandler OnItemStorageCountChanged;

    int GetItemStoredCount(ItemSO filterItemSO);
    bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO);
    bool TryStoreItem(ItemSO itemSO);
    ItemSO[] GetItemSOThatCanStore();

}
