using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MapGenerator {
    public int width { get; private set; }
    public int height { get; private set; }

    private HashSet<Vector2Int> path = new HashSet<Vector2Int>();
    private MapDataSO data;
    private int currentStraightLength;
    private BuildingDir currentPathDir;
    private BuildingDir lastPathDir;
    private Vector2Int heightBounds;
    private bool shouldGenerateAgain;
    private bool preventDiagonal;
    private int backtrackedTimes;
    private Vector2Int backtrackingBorder;
    private Vector2Int pathStart;
    private Vector2Int pathEnd;
    private int loopsAmount;
    private Vector2Int splitPadding;
    private int splitAmount;

    public MapGenerator(MapDataSO mapData) {
        data = mapData;
        width = Random.Range(data.widthRange.x, data.widthRange.y);
        height = Random.Range(data.heightRange.x, data.heightRange.y);
    }

    public async Task GenerateMap() {
        int baseBorder = Random.Range(data.basePaddingRange.x, data.basePaddingRange.y);
        int y = Random.Range(baseBorder, height - baseBorder);
        int x = width - baseBorder + data.minimumStraightLenghtOnBase;
        int portalBorder = Random.Range(data.portalPaddingRange.x, data.portalPaddingRange.y);
        int portalX = portalBorder + data.minimumStraightLenghtOnPortal;
        heightBounds = new Vector2Int(data.heightPadding, height - data.heightPadding);
        backtrackingBorder = new Vector2Int(portalX + data.backtrackingPreventingPadding, x - data.backtrackingPreventingPadding);
        pathStart = new Vector2Int(x, y);
        splitPadding = new Vector2Int(x - data.splitPadding, portalX + data.splitPadding + data.splitLengthRange.y);
        splitAmount = Random.Range(data.splitsAmountRange.x, data.splitsAmountRange.y);
#if UNITY_EDITOR
        int safetyCheck = 0;
#endif

        while (path.Count < data.PathLenghtRange.x || path.Count > data.PathLenghtRange.y || backtrackedTimes < data.backtracksAmountRange.x || shouldGenerateAgain) {
            GeneratePath(x, y, portalX, portalBorder);
#if UNITY_EDITOR
            safetyCheck++;

            if (safetyCheck <= 1000) continue;

            Debug.LogError($"Path generation failed! Safety break: {safetyCheck} attempts!");
            return;
#endif
        }

#if UNITY_EDITOR
        Debug.Log($"[MAP GENERATOR INFO]Path valid after {safetyCheck} generations");
#endif
        await LayPathAsync();
        await PopulateMapAsync();
        await SpawnPortalAsync();
        await SpawnBase();
        await SpawnVegetationAsync();
    }

    private void GeneratePath(int x, int y, int portalX, int portalBorder) {
        path.Clear();
        currentPathDir = BuildingDir.Left;
        lastPathDir = BuildingDir.Left;
        currentStraightLength = 0;
        backtrackedTimes = 0;
        shouldGenerateAgain = false;
        preventDiagonal = false;
        loopsAmount = 0;
        path.Add(new Vector2Int(x, y));
        int mostProgressedX = x - data.backtrackingPreventingPadding;

        for (int i = 0; i < data.minimumStraightLenghtOnBase; i++) {
            x--;
            currentStraightLength++;
            path.Add(new Vector2Int(x, y));
        }

        while (x > portalX) {
            HashSet<int> attemptedMoves = new HashSet<int>();

            while (true) {
                int move = Random.Range(0, 4);

                if (move == 0 && PathCellIsValid(x - 1, y) && MoveIsValid(BuildingDir.Left)) {
                    x--;

                    if (x < mostProgressedX) {
                        mostProgressedX = x;
                    }

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

                if (move == 3 && CanBacktrack(mostProgressedX, x) && PathCellIsValid(x + 1, y) && MoveIsValid(BuildingDir.Right)) {
                    x++;
                    break;
                }

                attemptedMoves.Add(move);

                if (attemptedMoves.Count != 4) continue;

                shouldGenerateAgain = true;
                return;
            }

            path.Add(new Vector2Int(x, y));

            if(data.shouldAllowPathSplits && x < splitPadding.x && x > splitPadding.y && loopsAmount < splitAmount && Random.Range(0, 5) == 0)
            {
                GenerateSplitedPaths(x, y);
            }
        }

        for (int i = 0; i < data.minimumStraightLenghtOnPortal; i++) {
            x--;
            path.Add(new Vector2Int(x, y));
        }

        if (y < portalBorder || y > height - portalBorder) {
            shouldGenerateAgain = true;
            return;
        }

        pathEnd = new Vector2Int(x, y);
    }

    private void GenerateSplitedPaths(int x, int y)
    {
        int width = Random.Range(data.splitLengthRange.x, data.splitLengthRange.y);
        int height = Random.Range(data.splitHeightRange.x, data.splitHeightRange.y);

        Vector2Int mergePoint = new Vector2Int(x - width, Random.Range(-height, height));

        if (randomValue == 0)
        {

        } else if(randomValue == 1)
        {

        } else
        {

        }
    }

    private bool PathCellIsValid(int x, int y) {
        return !path.Contains(new Vector2Int(x, y)) && IsCellInBounds(y) && GetNeighbouringPathCount(x, y) == 1 && (!data.shouldPreventSquareLoops || GetNeighbouringPathCountDiagonal(x, y) <= 1);
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

    private int GetNeighbouringPathCountDiagonal(int x, int y) {
        int count = 0;
        if (path.Contains(new Vector2Int(x + 1, y + 1))) count++;
        if (path.Contains(new Vector2Int(x + 1, y - 1))) count++;
        if (path.Contains(new Vector2Int(x - 1, y + 1))) count++;
        if (path.Contains(new Vector2Int(x - 1, y - 1))) count++;
        return count;
    }

    private bool MoveIsValid(BuildingDir dir) {
        if (dir == currentPathDir) {
            if (currentStraightLength == data.maximumStraightPathLenght) return false;

            currentStraightLength++;

            if (currentStraightLength == 3) {
                preventDiagonal = false;
            }
        }
        else {
            if (dir == lastPathDir) {
                if (preventDiagonal) return false;

                preventDiagonal = true;
            }
            else {
                preventDiagonal = false;
            }

            lastPathDir = currentPathDir;
            currentPathDir = dir;
            currentStraightLength = 1;

            if (currentPathDir == BuildingDir.Right) {
                backtrackedTimes++;
            }
        }

        return true;
    }

    private bool CanBacktrack(int mostProgressedX, int x) {
        return backtrackedTimes != data.backtracksAmountRange.y && mostProgressedX + data.maximumBacktrackingPathLenght >= x && x > backtrackingBorder.x && x < backtrackingBorder.y;
    }

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

    private void PopulateMap() {
        LayPath();
    }

    private void LayPath() {
        GridCell[,] grid = BuildingSystem.Instance.grid.gridArray;

        foreach (var cell in path) {
            grid[cell.x, cell.y].MarkPathCell();
            int neighboursValue = GetNeighboursValue(cell.x, cell.y);
            GameObject.Instantiate(GetPathCellPrefab(neighboursValue), new Vector3(cell.x + .5f, 0, cell.y + .5f), Quaternion.Euler(0, GetRotation(neighboursValue), 0));
        }
    }

    private async Task LayPathAsync() {
        GridCell[,] grid = BuildingSystem.Instance.grid.gridArray;

        foreach (var cell in path) {
            grid[cell.x, cell.y].MarkPathCell();
            GameObject.Instantiate(GetPathCellPrefab(GetNeighboursValue(cell.x, cell.y)), new Vector3(cell.x + .5f, 0, cell.y + .5f), Quaternion.Euler(0, GetRotation(GetNeighboursValue(cell.x, cell.y)), 0));

            await Task.Delay(20);
        }
    }

    private async Task PopulateMapAsync() {
        var grid = BuildingSystem.Instance.grid.gridArray;
        List<Vector3> spawnPositions = new List<Vector3>();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (path.Contains(new Vector2Int(x, y))) continue;

                spawnPositions.Add(new Vector3(x + 0.5f, 0, y + 0.5f));

                if (spawnPositions.Count >= 7) {
                    await SpawnTilesBatch(spawnPositions);
                    spawnPositions.Clear();
                }
            }
        }

        if (spawnPositions.Count > 0) {
            await SpawnTilesBatch(spawnPositions);
        }
    }

    private async Task SpawnTilesBatch(List<Vector3> positions) {
        foreach (var pos in positions) {
            GameObject.Instantiate(data.groundPrefab, pos, Quaternion.identity);
        }

        await Task.Yield();
    }

    private async Task SpawnPortalAsync() {
        await Task.Delay(500);

        Vector2Int origin = pathEnd + new Vector2Int(-data.portalData.width, -data.baseData.height / 2);
        BuildingSystem.Instance.TryPlaceMapGeneratedObject(origin, data.portalData, BuildingDir.Down);
    }

    private async Task SpawnBase() {
        await Task.Delay(300);

        Vector2Int origin = pathStart + new Vector2Int(1, -data.baseData.height / 2);
        BuildingSystem.Instance.TryPlaceMapGeneratedObject(origin, data.baseData, BuildingDir.Right);
    }

    private async Task SpawnVegetationAsync() {
        await Task.Delay(200);
        
        List<Vector2Int> allCells = new List<Vector2Int>();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                allCells.Add(new Vector2Int(x, y));
            }
        }

        List<Vector2Int> spawnLocations = allCells.OrderBy(_ => Random.value).Take(Mathf.CeilToInt(allCells.Count * data.resourcesOnMapPercent / 100f)).ToList();

        int spawnsCount = spawnLocations.Count;

        for (int i = 0; i < spawnsCount; i++) {
            BuildingSystem.Instance.TryPlaceMapGeneratedObject(spawnLocations[i], SelectRandomResourceSO(), GetRandomRotation());
            await Task.Delay(5);
        }
    }

    private ResourceNodeSO SelectRandomResourceSO() {
        int totalWeight = data.resourcesOnMap.Sum(resource => resource.weight);
        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var resource in data.resourcesOnMap.OrderByDescending(r => r.weight)) {
            cumulative += resource.weight;

            if (roll < cumulative) return resource.resourceNode;
        }

        return null;
    }

    private BuildingDir GetRandomRotation() {
        int roll = Random.Range(0, 4);

        if (roll == 0) return BuildingDir.Right;
        if (roll == 1) return BuildingDir.Up;
        if (roll == 2) return BuildingDir.Down;

        return BuildingDir.Left;
    }
}