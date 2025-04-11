using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Items/World Item")]
public class ItemSO : ScriptableObject {
    [field: SerializeField] public Sprite icon { get; private set; }
    [field: SerializeField] public string itemName { get; private set; }
    [field: SerializeField] public GameObject prefab { get; private set; }
}