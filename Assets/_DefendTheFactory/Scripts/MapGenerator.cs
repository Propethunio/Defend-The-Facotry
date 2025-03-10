using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MapGenerator {
    public int width { get; private set; }
    public int height { get; private set; }

    private HashSet<Vector2Int> path = new HashSet<Vector2Int>();
    private MapDataSO data;
    private int currentStraightLength;
    private BuildingDir pathDir;
    private Vector2Int heightBounds;
    private bool shouldGenerateAgain;

    public MapGenerator(MapDataSO mapData) {
        data = mapData;
        width = Random.Range(data.widthRange.x, data.widthRange.y);
        height = Random.Range(data.heightRange.x, data.heightRange.y);
    }

    public void GenerateMap() {
        int x = Random.Range(data.portalBorderRange.x, data.portalBorderRange.y);
        int y = Random.Range(data.heightBorder, height - data.heightBorder);
        int baseBorder = Random.Range(data.baseBorderRange.x, data.baseBorderRange.y);
        int pathEndX = width - baseBorder - data.minimumStraightLenghtOnEnd;
        heightBounds = new Vector2Int(data.heightBorder, height - data.heightBorder);
        int safetyCheck = 0;

        while (path.Count < data.minimumPathLenght || path.Count > data.maximumPathLenght || shouldGenerateAgain) {
            safetyCheck++;

            if (safetyCheck > 550) {
                Debug.Log("SAFETY BREAK");
                return;
            }

            GeneratePath(x, y, baseBorder, pathEndX);
        }

        LayPathAsync();

        //PopulateMap(width, height);
    }

    private void GeneratePath(int x, int y, int baseBorder, int pathEndX) {
        path.Clear();
        pathDir = BuildingDir.Right;
        currentStraightLength = 0;
        shouldGenerateAgain = false;
        path.Add(new Vector2Int(x, y));

        for (int i = 0; i < data.minimumStraightLenghtOnStart; i++) {
            x++;
            currentStraightLength++;
            path.Add(new Vector2Int(x, y));
        }

        while (x < pathEndX) {
            while (true) {
                int move = Random.Range(0, 3);

                if (move == 0 && PathCellIsValid(x + 1, y) && MoveIsValid(BuildingDir.Right)) {
                    x++;
                    break;
                }

                if (move == 1 && PathCellIsValid(x, y + 1) && MoveIsValid(BuildingDir.Up)) {
                    y++;
                    break;
                }

                if (move == 2 && PathCellIsValid(x, y - 1) && MoveIsValid(BuildingDir.Down)) {
                    y--;
                    break;
                }
            }

            path.Add(new Vector2Int(x, y));
        }

        for (int i = 0; i < data.minimumStraightLenghtOnEnd; i++) {
            x++;
            path.Add(new Vector2Int(x, y));
        }

        if (y < baseBorder || y > height - baseBorder) shouldGenerateAgain = true;
    }

    private bool PathCellIsValid(int x, int y) {
        return !path.Contains(new Vector2Int(x, y)) && IsCellInBounds(y) && GetNeighbouringPathCount(x, y) == 1;
    }

    private bool IsCellInBounds(int y) {
        return y >= heightBounds.x && y < heightBounds.y;
    }

    private int GetNeighbouringPathCount(int x, int y) {
        int count = 0;
        if (path.Contains(new Vector2Int(x + 1, y))) count++;
        if (path.Contains(new Vector2Int(x - 1, y))) count++;
        if (path.Contains(new Vector2Int(x, y + 1))) count++;
        if (path.Contains(new Vector2Int(x, y - 1))) count++;
        return count;
    }

    private bool MoveIsValid(BuildingDir dir) {
        if (dir == pathDir) {
            if (currentStraightLength >= data.maximumStraightLenght) return false;

            currentStraightLength++;
        }
        else {
            pathDir = dir;
            currentStraightLength = 1;
        }

        return true;
    }

    private void LayPath() { }

    private int GetNeighboursValue(int x, int y) {
        int value = 0;
        if (path.Contains(new Vector2Int(x + 1, y))) value++;
        if (path.Contains(new Vector2Int(x - 1, y))) value += 2;
        if (path.Contains(new Vector2Int(x, y + 1))) value += 4;
        if (path.Contains(new Vector2Int(x, y - 1))) value += 8;
        return value;
    }

    private GameObject GetPathCellPrefab(int value) {
        if (value is 1 or 2 or 3 or 12) return data.pathStraightPrefab;
        if (value is 5 or 6 or 9 or 10) return data.pathTurnPrefab;
        if (value is 7 or 11 or 13 or 14) return data.pathTurnPrefab;

        Debug.Log($"value: {value}");
        return null;
    }

    private int GetRotation(int value) {
        if (value is 9 or 11 or 12) return 0;
        if (value is 1 or 2 or 3 or 14 or 10) return 90;
        if (value is 7 or 6) return 180;
        if (value is 5 or 13) return 270;

        Debug.Log($"value: {value}");
        return 0;
    }

    private void PopulateMap(int width, int height) { }

    private async Task LayPathAsync() {
        GridCell[,] grid = BuildingSystem.Instance.grid.gridArray;

        foreach (var cell in path) {
            grid[cell.x, cell.y].MarkPathCell();
            GameObject prefab = GetPathCellPrefab(GetNeighboursValue(cell.x, cell.y));
            GameObject.Instantiate(prefab, new Vector3(cell.x + .5f, 0, cell.y + .5f), Quaternion.Euler(0, GetRotation(GetNeighboursValue(cell.x, cell.y)), 0));

            await Task.Delay(1);
        }
    }
}