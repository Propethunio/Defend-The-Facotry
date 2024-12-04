using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlacedObjectTypeSO : ScriptableObject {

    public string nameString;
    public Transform prefab;
    public Transform visual;
    public int width;
    public int height;

    public Vector2Int GetRotationOffset(Dir dir) {
        switch(dir) {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, width);
            case Dir.Up: return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir) {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch(dir) {
            default:
            case Dir.Down:
            case Dir.Up:
                for(int x = 0; x < width; x++) {
                    for(int y = 0; y < height; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for(int x = 0; x < height; x++) {
                    for(int y = 0; y < width; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }

        return gridPositionList;
    }

    public Vector2Int GetMachineBeltPosition(Vector2Int origin, Vector2Int beltPos, Dir dir) {
        int beltX = beltPos.x;
        int beltY = beltPos.y;
        Vector2Int rotatedBeltPos;

        switch(dir) {
            default:
            case Dir.Down:
                rotatedBeltPos = new Vector2Int(beltX, beltY);
                break;

            case Dir.Left:
                rotatedBeltPos = new Vector2Int(beltY, width - beltX - 1);
                break;

            case Dir.Up:
                rotatedBeltPos = new Vector2Int(width - beltX - 1, height - beltY - 1);
                break;

            case Dir.Right:
                rotatedBeltPos = new Vector2Int(height - beltY - 1, beltX);
                break;
        }

        return origin + rotatedBeltPos;
    }
}