using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Towers/Standard Tower")]
public class TargetPickingTowerSO : BaseTowerSO {
    [field: SerializeField] public GameObject projectile { get; private set; }
}