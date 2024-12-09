using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlacedObjectTypeSO : ScriptableObject {

    public string nameString;
    public Transform prefab;
    public Transform visual;
    public int width;
    public int height;

    public Vector2Int GetRotationOffset(BuildingDir dir) {
        switch(dir) {
            default:
            case BuildingDir.Down: return new Vector2Int(0, 0);
            case BuildingDir.Left: return new Vector2Int(0, width);
            case BuildingDir.Up: return new Vector2Int(width, height);
            case BuildingDir.Right: return new Vector2Int(height, 0);
        }
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, BuildingDir dir) {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch(dir) {
            default:
            case BuildingDir.Down:
            case BuildingDir.Up:
                for(int x = 0; x < width; x++) {
                    for(int y = 0; y < height; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case BuildingDir.Left:
            case BuildingDir.Right:
                for(int x = 0; x < height; x++) {
                    for(int y = 0; y < width; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }

        return gridPositionList;
    }

    public Vector2Int GetMachineBeltPosition(Vector2Int origin, Vector2Int beltPos, BuildingDir dir) {
        int beltX = beltPos.x;
        int beltY = beltPos.y;
        Vector2Int rotatedBeltPos;

        switch(dir) {
            default:
            case BuildingDir.Down:
                rotatedBeltPos = new Vector2Int(beltX, beltY);
                break;

            case BuildingDir.Left:
                rotatedBeltPos = new Vector2Int(beltY, width - beltX - 1);
                break;

            case BuildingDir.Up:
                rotatedBeltPos = new Vector2Int(width - beltX - 1, height - beltY - 1);
                break;

            case BuildingDir.Right:
                rotatedBeltPos = new Vector2Int(height - beltY - 1, beltX);
                break;
        }

        return origin + rotatedBeltPos;
    }
}