using UnityEngine;

public abstract class BaseTowerSO : BaseBuildableObjectSO {
    [field: SerializeField] public float damage { get; private set; }
    [field: SerializeField] public float attackSpeed { get; private set; }
    [field: SerializeField] public float range { get; private set; }
    [field: SerializeField] public bool canTargetFlying { get; private set; }

    public Vector2 GetCenterPositionForCollider(BuildingDir dir) {
        bool isLeftRightRotated = dir == BuildingDir.Left || dir == BuildingDir.Right;
        float rotatedW = isLeftRightRotated ? height : width;
        float rotatedH = isLeftRightRotated ? width : height;
        Vector2 centerOffset = new Vector2(rotatedW / 2, rotatedH / 2);

        switch (dir) {
            default:
            case BuildingDir.Down: return centerOffset;
            case BuildingDir.Left: return new Vector2(centerOffset.x, width - centerOffset.y);
            case BuildingDir.Up: return new Vector2(width - centerOffset.x, height - centerOffset.y);
            case BuildingDir.Right: return new Vector2(height - centerOffset.x, centerOffset.y);
        }
    }
}