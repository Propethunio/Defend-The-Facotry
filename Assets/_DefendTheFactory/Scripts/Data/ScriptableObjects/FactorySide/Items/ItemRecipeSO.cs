using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Items/Item Recipe")]
public class ItemRecipeSO : ScriptableObject {
    [field: SerializeField] public List<ItemIntPair> inputItemList { get; private set; }
    [field: SerializeField] public List<ItemIntPair> outputItemList { get; private set; }
    [field: SerializeField] public int craftingTicks { get; private set; }
}