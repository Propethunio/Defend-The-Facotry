using UnityEngine;

public abstract class BaseMachineSO : BaseBuildableObjectSO {

    [field: SerializeField] public Vector2Int outputBeltPosition { get; private set; }
    [field: SerializeField] public int maxStoredOutputItems { get; private set; }

    public Vector2Int GetMachineBeltPosition(Vector2Int origin, Vector2Int beltPos, BuildingDir dir) {
        switch(dir) {
            default:
            case BuildingDir.Down: return origin + beltPos;
            case BuildingDir.Left: return origin + new Vector2Int(beltPos.y, width - beltPos.x - 1);
            case BuildingDir.Up: return origin + new Vector2Int(width - beltPos.x - 1, height - beltPos.y - 1);
            case BuildingDir.Right: return origin + new Vector2Int(height - beltPos.y - 1, beltPos.x);
        }
    }
}