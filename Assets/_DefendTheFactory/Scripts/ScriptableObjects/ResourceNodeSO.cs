using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Buildable Objects/Resource Node")]
public class ResourceNodeSO : BaseBuildableObjectSO {

    [field: SerializeField] public ResourcesEnum resourceType { get; private set; }
    [field: SerializeField] public int amount { get; private set; }
}