using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ItemSO : ScriptableObject {

    public string itemName;
    public Sprite sprite;

    public static bool IsItemSOInFilter(ItemSO itemSO, ItemSO[] filterItemSOArray) {
        // Does this Item match this Filter?
        foreach (ItemSO filterItemSO in filterItemSOArray) {
            if (itemSO == filterItemSO) {
                // Matches this exact one
                return true;
            }
            if (filterItemSO == GameAssets.i.itemSO_Refs.any && itemSO != GameAssets.i.itemSO_Refs.none) {
                // Filter is Any and the testing one is not None
                return true;
            }
        }
        return false;
    }

    public static void DebugFilter(ItemSO[] filterItemSO) {
        Debug.Log("## DebugFilter:");
        foreach (ItemSO itemSO in filterItemSO) {
            Debug.Log(itemSO);
        }
    }

    public static ItemSO[] GetCombinedFilter(ItemSO[] filterA, ItemSO[] filterB) {
        List<ItemSO> filterAList = new List<ItemSO>(filterA);
        List<ItemSO> filterBList = new List<ItemSO>(filterB);

        if (filterAList.Contains(GameAssets.i.itemSO_Refs.none) ||
            filterBList.Contains(GameAssets.i.itemSO_Refs.none)) {
            // One of the filters contains "NONE", combined will be NONE
            return new ItemSO[] { GameAssets.i.itemSO_Refs.none };
        }

        if (filterAList.Contains(GameAssets.i.itemSO_Refs.any)) {
            // filterA contains ANY, resulting filter will be filterB
            return filterB;
        }

        if (filterBList.Contains(GameAssets.i.itemSO_Refs.any)) {
            // filterB contains ANY, resulting filter will be filterA
            return filterA;
        }

        // Doesn't have ANY or NONE

        List<ItemSO> combinedFilter = new List<ItemSO>();

        foreach (ItemSO itemA in filterA) {
            if (filterBList.Contains(itemA)) {
                combinedFilter.Add(itemA);
            }
        }

        return combinedFilter.ToArray();
    }

}
