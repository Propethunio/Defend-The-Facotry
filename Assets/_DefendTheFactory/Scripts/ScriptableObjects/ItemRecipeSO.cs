using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ItemRecipeSO : ScriptableObject {

    public List<RecipeItem> outputItemList;
    public List<RecipeItem> inputItemList;
    public float craftingEffort;


    [System.Serializable]
    public struct RecipeItem {

        public ItemSO item;
        public int amount;

    }

}
