using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Towers/Standard Tower")]
public class TargetPickingTowerSO : BaseTowerSO {
    [field: SerializeField] public bool shouldRotate { get; private set; }
    [field: SerializeField] public float rotationSpeed { get; private set; }
    [field: SerializeField] public GameObject projectile { get; private set; }
}