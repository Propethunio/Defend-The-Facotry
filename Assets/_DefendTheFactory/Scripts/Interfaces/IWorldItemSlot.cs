using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldItemSlot {

    bool TrySetWorldItem(WorldItem worldItem);
    bool TryGetWorldItem(ItemSO[] filterItemSO, out WorldItem worldItem);
    Vector2Int GetGridPosition();
    ItemSO[] GetItemSOThatCanStore();
}