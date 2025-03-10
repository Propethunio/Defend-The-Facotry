using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Map Data")]
public class MapDataSO : ScriptableObject {
    [field: SerializeField] public Vector2Int widthRange { get; private set; }
    [field: SerializeField] public Vector2Int heightRange { get; private set; }
    [field: SerializeField] public Vector2Int portalBorderRange { get; private set; }
    [field: SerializeField] public Vector2Int baseBorderRange { get; private set; }
    [field: SerializeField] public int heightBorder { get; private set; }
    [field: SerializeField] public int minimumPathLenght { get; private set; }
    [field: SerializeField] public int maximumPathLenght { get; private set; }
    [field: SerializeField] public int minimumStraightLenghtOnStart { get; private set; }
    [field: SerializeField] public int minimumStraightLenghtOnEnd { get; private set; }
    [field: SerializeField] public int maximumStraightLenght { get; private set; }
    [field: SerializeField] public GameObject pathStraightPrefab { get; private set; }
    [field: SerializeField] public GameObject pathTurnPrefab { get; private set; }
    [field: SerializeField] public GameObject pathSplitPrefab { get; private set; }
}