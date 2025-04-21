using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Enemies/Enemy")]
public class EnemySO : ScriptableObject {
    [field: SerializeField] public int health { get; private set; }
    [field: SerializeField] public int shield { get; private set; }
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public bool isFlying { get; private set; }
    [field: SerializeField] public Transform modelPrefab { get; private set; }
}