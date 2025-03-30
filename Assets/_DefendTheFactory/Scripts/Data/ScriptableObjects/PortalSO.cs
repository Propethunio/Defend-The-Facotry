using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Buildable Objects/Portal")]
public class PortalSO : BaseBuildableObjectSO {
    [field: SerializeField] public Vector2Int spawnPointOffsetFromCenter { get; private set; }
}