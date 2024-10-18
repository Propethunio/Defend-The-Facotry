using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldItemSlot {

    bool IsEmpty();
    bool TrySetWorldItem(WorldItem worldItem);
    bool TryGetWorldItem(ItemSO[] filterItemSO, out WorldItem worldItem);
    WorldItem GetWorldItem();
    void RemoveWorldItem();
    Vector2Int GetGridPosition();
    ItemSO[] GetItemSOThatCanStore();

}
