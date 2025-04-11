using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Items/Simple Item Recipe")]
public class SimpleItemRecipeSO : ScriptableObject {
    [field: SerializeField] public ItemIntPair inputItem { get; private set; }
    [field: SerializeField] public ItemIntPair outputItem { get; private set; }
    [field: SerializeField] public int craftingTicks { get; private set; }
}