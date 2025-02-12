using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BaseBuildableObjectSO : ScriptableObject {

    [field: SerializeField] public string nameString { get; private set; }
    [field: SerializeField] public Transform prefab { get; private set; }
    [field: SerializeField] public Transform visual { get; private set; }
    [field: SerializeField] public int width { get; private set; }
    [field: SerializeField] public int height { get; private set; }

    public Vector2Int GetRotationOffset(BuildingDir dir) {
        switch(dir) {
            default:
            case BuildingDir.Down: return Vector2Int.zero;
            case BuildingDir.Left: return new Vector2Int(0, width);
            case BuildingDir.Up: return new Vector2Int(width, height);
            case BuildingDir.Right: return new Vector2Int(height, 0);
        }
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, BuildingDir dir) {
        List<Vector2Int> gridPositionList = new List<Vector2Int>(width * height);
        bool isLeftRightRotated = dir == BuildingDir.Left || dir == BuildingDir.Right;
        int rotatedW = isLeftRightRotated ? height : width;
        int rotatedH = isLeftRightRotated ? width : height;

        for(int x = 0; x < rotatedW; x++) {
            for(int y = 0; y < rotatedH; y++) {
                gridPositionList.Add(offset + new Vector2Int(x, y));
            }
        }

        return gridPositionList;
    }

    public Vector2Int GetMachineBeltPosition(Vector2Int origin, Vector2Int beltPos, BuildingDir dir) {
        switch(dir) {
            default:
            case BuildingDir.Down: return origin + beltPos;
            case BuildingDir.Left: return origin + new Vector2Int(beltPos.y, width - beltPos.x - 1);
            case BuildingDir.Up: return origin + new Vector2Int(width - beltPos.x - 1, height - beltPos.y - 1);
            case BuildingDir.Right: return origin + new Vector2Int(height - beltPos.y - 1, beltPos.x);
        }
    }

    public Vector2 GetCenterPosition(Vector2Int origin, BuildingDir dir) {
        bool isLeftRightRotated = dir == BuildingDir.Left || dir == BuildingDir.Right;
        float rotatedW = isLeftRightRotated ? height : width;
        float rotatedH = isLeftRightRotated ? width : height;
        Vector2 centerOffset = new Vector2(rotatedW / 2, rotatedH / 2);

        switch(dir) {
            default:
            case BuildingDir.Down: return origin + centerOffset;
            case BuildingDir.Left: return origin + new Vector2(centerOffset.x, width - centerOffset.y);
            case BuildingDir.Up: return origin + new Vector2(width - centerOffset.x, height - centerOffset.y);
            case BuildingDir.Right: return origin + new Vector2(height - centerOffset.x, centerOffset.y);
        }
    }
}