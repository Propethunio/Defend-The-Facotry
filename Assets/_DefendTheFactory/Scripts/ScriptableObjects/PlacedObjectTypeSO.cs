using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlacedObjectTypeSO : ScriptableObject {

    public static Dir GetNextDir(Dir dir) {
        switch(dir) {
            default:
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }

    public static Vector2Int GetDirForwardVector(Dir dir) {
        switch(dir) {
            default:
            case Dir.Down: return new Vector2Int(0, -1);
            case Dir.Left: return new Vector2Int(-1, 0);
            case Dir.Up: return new Vector2Int(0, +1);
            case Dir.Right: return new Vector2Int(+1, 0);
        }
    }

    public static Dir GetDir(Vector2Int from, Vector2Int to) {
        if(from.x < to.x) {
            return Dir.Right;
        } else {
            if(from.x > to.x) {
                return Dir.Left;
            } else {
                if(from.y < to.y) {
                    return Dir.Up;
                } else {
                    return Dir.Down;
                }
            }
        }
    }


    public enum Dir {
        Down,
        Left,
        Up,
        Right,
    }

    public string nameString;
    public Transform prefab;
    public Transform visual;
    public int width;
    public int height;


    public int GetRotationAngle(Dir dir) {
        switch(dir) {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return 90;
            case Dir.Up: return 180;
            case Dir.Right: return 270;
        }
    }

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
                // No rotation, position stays the same
                rotatedBeltPos = new Vector2Int(beltX, beltY);
                break;

            case Dir.Left:
                // 90 degrees counterclockwise
                // New position: (beltY, width - beltX - 1)
                rotatedBeltPos = new Vector2Int(beltY, width - beltX - 1);
                break;

            case Dir.Up:
                // 180 degrees rotation
                // New position: (width - beltX - 1, height - beltY - 1)
                rotatedBeltPos = new Vector2Int(width - beltX - 1, height - beltY - 1);
                break;

            case Dir.Right:
                // 90 degrees clockwise
                // New position: (height - beltY - 1, beltX)
                rotatedBeltPos = new Vector2Int(height - beltY - 1, beltX);
                break;
        }

        // The belt's absolute position on the grid is the origin plus the rotated position
        return origin + rotatedBeltPos;
    }
}