using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Map Data")]
public class MapDataSO : ScriptableObject {
    [field: SerializeField] public int width { get; private set; }
    [field: SerializeField] public int height { get; private set; }
}