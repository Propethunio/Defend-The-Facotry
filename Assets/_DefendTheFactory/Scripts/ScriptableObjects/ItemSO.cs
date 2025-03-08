using UnityEngine;

[CreateAssetMenu()]
public class ItemSO : ScriptableObject {
    [field: SerializeField] public Sprite icon { get; private set; }
    [field: SerializeField] public string itemName { get; private set; }
    [field: SerializeField] public GameObject prefab { get; private set; }
}