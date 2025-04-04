using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ItemRecipeSO : ScriptableObject {
    public List<ItemIntPair> inputItemList;
    public List<ItemIntPair> outputItemList;
    public int craftingTicks;
}