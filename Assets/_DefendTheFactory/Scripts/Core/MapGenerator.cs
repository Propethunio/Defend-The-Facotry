using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MapGenerator {
    public int width { get; private set; }
    public int height { get; private set; }

    private Transform mainParent;
    private Transform terrainParent;
    private Transform pathParent;
    private Transform resourcesParent;
    private List<Vector2Int> path = new List<Vector2Int>();
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
    private HashSet<Vector2Int> basePaddingCells = new HashSet<Vector2Int>();

    public MapGenerator(MapDataSO mapData) {
        data = mapData;
        width = Random.Range(data.widthRange.x, data.widthRange.y);
        height = Random.Range(data.heightRange.x, data.heightRange.y);
        mainParent = new GameObject("Map Generator").transform;
        terrainParent = new GameObject("Terrain").transform;
        pathParent = new GameObject("Path").transform;
        resourcesParent = new GameObject("Resources").transform;
        terrainParent.parent = mainParent;
        pathParent.parent = mainParent;
        resourcesParent.parent = mainParent;
    }

    public void GenerateMap(bool async) {
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

            Debug.LogError($"[MAP GENERATOR ERROR] Path generation failed! Safety break: {safetyCheck} attempts!");
            return;
#endif
        }

#if UNITY_EDITOR
        Debug.Log($"[MAP GENERATOR INFO] Path valid after {safetyCheck} generations");
#endif

        if (async) {
            GenerateAsync();
            return;
        }

        PopulateMap();
        WaveManager.Instance.SetPath(path);
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

            if (data.shouldAllowPathSplits && loopsAmount < splitAmount && x < splitPadding.x && x > splitPadding.y && currentPathDir != BuildingDir.Right && Random.Range(0, 15) == 0) {
                GenerateSplitPaths(ref x, ref y);
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

    private void GenerateSplitPaths(ref int x, ref int y) {
        return;

        bool success = false;
        int attempts = 0;
        int maxAttempts = 1000; // you can adjust this value as needed

        while (!success && attempts < maxAttempts) {
            attempts++;

            // Calculate target merge point based on split parameters.
            int splitWidth = Random.Range(data.splitLengthRange.x, data.splitLengthRange.y);
            int splitHeight = Random.Range(data.splitHeightRange.x, data.splitHeightRange.y);
            Vector2Int mergePoint = new Vector2Int(x - splitWidth, y + Random.Range(-splitHeight, splitHeight));
            int lengthToMergePoint = splitWidth + Mathf.Abs(y - mergePoint.y);
            int minSplitPathLength = lengthToMergePoint + 6; // minimal length (can be tuned)
            int maxSplitPathLength = lengthToMergePoint + 18; // or any max you want to enforce

            // Two lists representing the two split paths (each starting at (x,y))
            List<Vector2Int> firstSplitPath = new List<Vector2Int>();
            List<Vector2Int> secondSplitPath = new List<Vector2Int>();
            firstSplitPath.Add(new Vector2Int(x, y));
            secondSplitPath.Add(new Vector2Int(x, y));

            // Determine the initial directions for the two split paths.
            BuildingDir firstSplitStartDir;
            BuildingDir secondSplitStartDir;
            int randomValue = Random.Range(0, 3);

            if (randomValue == 0) {
                firstSplitStartDir = GetDirectionRotatedByLastStepDirection(BuildingDir.Up);
                secondSplitStartDir = GetDirectionRotatedByLastStepDirection(BuildingDir.Down);
            }
            else if (randomValue == 1) {
                firstSplitStartDir = GetDirectionRotatedByLastStepDirection(BuildingDir.Up);
                secondSplitStartDir = GetDirectionRotatedByLastStepDirection(BuildingDir.Left);
            }
            else {
                firstSplitStartDir = GetDirectionRotatedByLastStepDirection(BuildingDir.Left);
                secondSplitStartDir = GetDirectionRotatedByLastStepDirection(BuildingDir.Down);
            }

            Vector2Int firstSplitMove = GetVectorBaseOnDirection(firstSplitStartDir);
            Vector2Int secondSplitMove = GetVectorBaseOnDirection(secondSplitStartDir);

            // Make a few initial moves (e.g., 2 steps) to separate the two branches.
            int initialSteps = 1;
            bool initialValid = true;

            for (int i = 0; i < initialSteps; i++) {
                Vector2Int nextFirst = firstSplitPath.Last() + firstSplitMove;
                Vector2Int nextSecond = secondSplitPath.Last() + secondSplitMove;

                if (IsValidSplitCell(nextFirst, firstSplitPath, secondSplitPath) && IsValidSplitCell(nextSecond, secondSplitPath, firstSplitPath)) {
                    firstSplitPath.Add(nextFirst);
                    secondSplitPath.Add(nextSecond);
                }
                else {
                    // Try new parameters if the initial separation fails.
                    initialValid = false;
                    break;
                }
            }

            if (!initialValid)
                continue; // retry from the beginning

            // Grow both branches concurrently until both reach the merge point
            // while enforcing equal branch lengths.
            bool generationFailed = false;

            while (firstSplitPath.Count <= maxSplitPathLength && secondSplitPath.Count <= maxSplitPathLength) {
                bool firstAtMerge = (firstSplitPath.Last() == mergePoint);
                bool secondAtMerge = (secondSplitPath.Last() == mergePoint);

                // When both branches have reached the merge point and are long enough, we are done.
                if (firstAtMerge && secondAtMerge && firstSplitPath.Count >= minSplitPathLength && secondSplitPath.Count >= minSplitPathLength) {
                    break;
                }

                // If one branch has reached the merge point too early, force a detour.
                if (firstAtMerge && !secondAtMerge) {
                    List<Vector2Int> detourMoves = GetValidSplitMovesAwayFromMerge(firstSplitPath.Last(), firstSplitPath, secondSplitPath, mergePoint);

                    if (detourMoves.Count > 0) {
                        firstSplitPath.Add(detourMoves[Random.Range(0, detourMoves.Count)]);
                        continue;
                    }
                    else {
                        generationFailed = true;
                        break;
                    }
                }

                if (secondAtMerge && !firstAtMerge) {
                    List<Vector2Int> detourMoves = GetValidSplitMovesAwayFromMerge(secondSplitPath.Last(), secondSplitPath, firstSplitPath, mergePoint);

                    if (detourMoves.Count > 0) {
                        secondSplitPath.Add(detourMoves[Random.Range(0, detourMoves.Count)]);
                        continue;
                    }
                    else {
                        generationFailed = true;
                        break;
                    }
                }

                // Both branches are still en route to merge: pick valid moves concurrently.
                List<Vector2Int> firstOptions = GetValidSplitMoves(firstSplitPath.Last(), firstSplitPath, secondSplitPath, mergePoint);
                List<Vector2Int> secondOptions = GetValidSplitMoves(secondSplitPath.Last(), secondSplitPath, firstSplitPath, mergePoint);

                if (firstOptions.Count == 0 || secondOptions.Count == 0) {
                    generationFailed = true;
                    break;
                }

                // Choose one move from each branch.
                Vector2Int nextFirst = firstOptions[Random.Range(0, firstOptions.Count)];
                Vector2Int nextSecond = secondOptions[Random.Range(0, secondOptions.Count)];

                // Ensure the new moves are not adjacent to each other.
                if (!AreCellsAdjacent(nextFirst, nextSecond)) {
                    firstSplitPath.Add(nextFirst);
                    secondSplitPath.Add(nextSecond);
                }
                else {
                    // If the chosen moves would cause the branches to touch, try another option.
                    firstOptions.Remove(nextFirst);

                    if (firstOptions.Count == 0) {
                        generationFailed = true;
                        break;
                    }

                    continue;
                }
            }

            // Check if both branches reached the merge point successfully.
            if (generationFailed || firstSplitPath.Last() != mergePoint || secondSplitPath.Last() != mergePoint) {
                continue; // retry with new random parameters
            }

            // Successfully generated both branches.
            foreach (var cell in firstSplitPath)
                path.Add(cell);

            foreach (var cell in secondSplitPath)
                path.Add(cell);

            // Update the main path’s current position to the merge point.
            x = mergePoint.x;
            y = mergePoint.y;
            success = true;
        }

        if (!success) {
            // If after maxAttempts we couldn't generate split paths,
            // signal failure so that the main generation loop can restart.
            //shouldGenerateAgain = true;
        }

        Debug.Log(success);
    }

// Helper: Checks if a cell is valid for a split branch.
    private bool IsValidSplitCell(Vector2Int cell, List<Vector2Int> ownPath, List<Vector2Int> otherPath) {
        // Do not allow cell if it already belongs to the main path or either branch.
        if (path.Contains(cell) || ownPath.Contains(cell) || otherPath.Contains(cell))
            return false;

        // Ensure the cell is within vertical bounds.
        if (!IsCellInBounds(cell.y))
            return false;

        // Allow only one neighbouring cell within the branch.
        if (GetNeighbourCount(cell, ownPath) > 1)
            return false;

        // Prevent cells that would be adjacent to the other branch.
        if (IsAdjacentToPath(cell, otherPath))
            return false;

        return true;
    }

// Helper: Count adjacent (non-diagonal) neighbours from a given list.
    private int GetNeighbourCount(Vector2Int cell, List<Vector2Int> pathList) {
        int count = 0;
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var d in directions) {
            if (pathList.Contains(cell + d))
                count++;
        }

        return count;
    }

// Helper: Check if a cell is adjacent (non-diagonally) to any cell in a given list.
    private bool IsAdjacentToPath(Vector2Int cell, List<Vector2Int> pathList) {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var d in directions) {
            if (pathList.Contains(cell + d))
                return true;
        }

        return false;
    }

// Helper: Get valid next moves for a split branch (biased to not stray too far from the merge point).
    private List<Vector2Int> GetValidSplitMoves(Vector2Int current, List<Vector2Int> ownPath, List<Vector2Int> otherPath, Vector2Int mergePoint) {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        int currentDist = Mathf.Abs(current.x - mergePoint.x) + Mathf.Abs(current.y - mergePoint.y);

        foreach (var d in directions) {
            Vector2Int next = current + d;

            if (!IsValidSplitCell(next, ownPath, otherPath))
                continue;

            int nextDist = Mathf.Abs(next.x - mergePoint.x) + Mathf.Abs(next.y - mergePoint.y);

            // Allow moves that do not increase the distance by more than 1.
            if (nextDist <= currentDist + 1)
                moves.Add(next);
        }

        return moves;
    }

// Helper: When a branch is at the merge point too early, find a detour move (avoiding the merge point).
    private List<Vector2Int> GetValidSplitMovesAwayFromMerge(Vector2Int current, List<Vector2Int> ownPath, List<Vector2Int> otherPath, Vector2Int mergePoint) {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var d in directions) {
            Vector2Int next = current + d;

            if (next == mergePoint)
                continue;

            if (IsValidSplitCell(next, ownPath, otherPath))
                moves.Add(next);
        }

        return moves;
    }

// Helper: Checks if two cells are adjacent (non-diagonally).
    private bool AreCellsAdjacent(Vector2Int a, Vector2Int b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }

    private Vector2Int GetVectorBaseOnDirection(BuildingDir dir) {
        if (dir == BuildingDir.Up) return Vector2Int.up;
        if (dir == BuildingDir.Down) return Vector2Int.down;
        if (dir == BuildingDir.Left) return Vector2Int.left;

        return Vector2Int.right;
    }

    private BuildingDir GetDirectionRotatedByLastStepDirection(BuildingDir direction) {
        if (currentPathDir == BuildingDir.Up) {
            if (direction == BuildingDir.Left) return BuildingDir.Up;
            if (direction == BuildingDir.Up) return BuildingDir.Right;
            if (direction == BuildingDir.Down) return BuildingDir.Left;
        }

        if (currentPathDir == BuildingDir.Down) {
            if (direction == BuildingDir.Left) return BuildingDir.Down;
            if (direction == BuildingDir.Up) return BuildingDir.Left;
            if (direction == BuildingDir.Down) return BuildingDir.Right;
        }

        return direction;
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
        GenerateGround();
        SpawnPortal();
        SpawnBase();
        SpawnVegetation();
    }

    private void LayPath() {
        GridCell[,] grid = BuildingSystem.Instance.grid.gridArray;

        foreach (var cell in path) {
            grid[cell.x, cell.y].MarkPathCell();
            int neighboursValue = GetNeighboursValue(cell.x, cell.y);
            GameObject.Instantiate(GetPathCellPrefab(neighboursValue), new Vector3(cell.x + .5f, 0, cell.y + .5f), Quaternion.Euler(0, GetRotation(neighboursValue), 0), pathParent);
        }
    }

    private void GenerateGround() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (path.Contains(new Vector2Int(x, y))) continue;

                GameObject.Instantiate(data.groundPrefab, new Vector3(x + 0.5f, 0, y + 0.5f), Quaternion.identity, terrainParent);
            }
        }
    }

    private void SpawnPortal() {
        Vector2Int origin = pathEnd + new Vector2Int(-data.portalData.width, -data.baseData.height / 2);
        BuildingSystem.Instance.TryPlaceMapGeneratedObject(origin, data.portalData, BuildingDir.Down, mainParent);
        Vector2 portalCenter = data.portalData.GetCenterPosition(origin, BuildingDir.Down);
        WaveManager.Instance.SetSpawnPosition(new Vector3(portalCenter.x, 0f, portalCenter.y));
    }

    private void SpawnBase() {
        Vector2Int origin = pathStart + new Vector2Int(1, -data.baseData.height / 2);
        BuildingSystem.Instance.TryPlaceMapGeneratedObject(origin, data.baseData, BuildingDir.Right, mainParent);
        Vector2 baseCenterPosition = data.baseData.GetCenterPosition(origin, BuildingDir.Right);
        int endX = (int)baseCenterPosition.x + data.basePaddingPreventingObjectGeneration;
        int endY = (int)baseCenterPosition.y + data.basePaddingPreventingObjectGeneration;
        GridCell[,] grid = BuildingSystem.Instance.grid.gridArray;

        for (int x = (int)baseCenterPosition.x - data.basePaddingPreventingObjectGeneration; x <= endX; x++) {
            for (int y = (int)baseCenterPosition.y - data.basePaddingPreventingObjectGeneration; y <= endY; y++) {
                if (grid[x, y].isPathCell) continue;

                grid[x, y].MarkPathCell();
                basePaddingCells.Add(new Vector2Int(x, y));
            }
        }
    }

    private void SpawnVegetation() {
        List<Vector2Int> allCells = new List<Vector2Int>();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                allCells.Add(new Vector2Int(x, y));
            }
        }

        List<Vector2Int> spawnLocations = allCells.OrderBy(_ => Random.value).Take(Mathf.CeilToInt(allCells.Count * data.resourcesOnMapPercent / 100f)).ToList();
        int spawnsCount = spawnLocations.Count;

        for (int i = 0; i < spawnsCount; i++) {
            BuildingSystem.Instance.TryPlaceMapGeneratedObject(spawnLocations[i], SelectRandomResourceSO(), GetRandomRotation(), resourcesParent);
        }

        GridCell[,] grid = BuildingSystem.Instance.grid.gridArray;

        foreach (Vector2Int basePaddingCell in basePaddingCells) {
            grid[basePaddingCell.x, basePaddingCell.y].UnmarkPathCell();
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

    // ---------- ASYNC GENERATION TO SHOWCASE HOW IT WORKS ----------

    private async Task GenerateAsync() {
        await LayPathAsync();
        await GenerateGroundAsync();
        await SpawnPortalAsync();
        await SpawnBaseAsync();
        await SpawnVegetationAsync();
    }

    private async Task LayPathAsync() {
        GridCell[,] grid = BuildingSystem.Instance.grid.gridArray;

        foreach (var cell in path) {
            grid[cell.x, cell.y].MarkPathCell();
            GameObject.Instantiate(GetPathCellPrefab(GetNeighboursValue(cell.x, cell.y)), new Vector3(cell.x + .5f, 0, cell.y + .5f), Quaternion.Euler(0, GetRotation(GetNeighboursValue(cell.x, cell.y)), 0), pathParent);

            await Task.Delay(20);
        }
    }

    private async Task GenerateGroundAsync() {
        List<Vector3> spawnPositions = new List<Vector3>();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (path.Contains(new Vector2Int(x, y))) continue;

                spawnPositions.Add(new Vector3(x + 0.5f, 0, y + 0.5f));

                if (spawnPositions.Count >= 7) {
                    await SpawnGroundCellsBatch(spawnPositions);
                    spawnPositions.Clear();
                }
            }
        }

        if (spawnPositions.Count > 0) {
            await SpawnGroundCellsBatch(spawnPositions);
        }
    }

    private async Task SpawnGroundCellsBatch(List<Vector3> positions) {
        foreach (var pos in positions) {
            GameObject.Instantiate(data.groundPrefab, pos, Quaternion.identity, terrainParent);
        }

        await Task.Yield();
    }

    private async Task SpawnPortalAsync() {
        await Task.Delay(500);

        Vector2Int origin = pathEnd + new Vector2Int(-data.portalData.width, -data.baseData.height / 2);
        BuildingSystem.Instance.TryPlaceMapGeneratedObject(origin, data.portalData, BuildingDir.Down, mainParent);
    }

    private async Task SpawnBaseAsync() {
        await Task.Delay(300);

        Vector2Int origin = pathStart + new Vector2Int(1, -data.baseData.height / 2);
        BuildingSystem.Instance.TryPlaceMapGeneratedObject(origin, data.baseData, BuildingDir.Right, mainParent);
        Vector2 baseCenterPosition = data.baseData.GetCenterPosition(origin, BuildingDir.Right);
        int endX = (int)baseCenterPosition.x + data.basePaddingPreventingObjectGeneration;
        int endY = (int)baseCenterPosition.y + data.basePaddingPreventingObjectGeneration;
        GridCell[,] grid = BuildingSystem.Instance.grid.gridArray;

        for (int x = (int)baseCenterPosition.x - data.basePaddingPreventingObjectGeneration; x <= endX; x++) {
            for (int y = (int)baseCenterPosition.y - data.basePaddingPreventingObjectGeneration; y <= endY; y++) {
                if (grid[x, y].isPathCell) continue;

                grid[x, y].MarkPathCell();
                basePaddingCells.Add(new Vector2Int(x, y));
            }
        }
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
            BuildingSystem.Instance.TryPlaceMapGeneratedObject(spawnLocations[i], SelectRandomResourceSO(), GetRandomRotation(), resourcesParent);
            await Task.Delay(5);
        }

        GridCell[,] grid = BuildingSystem.Instance.grid.gridArray;

        foreach (Vector2Int basePaddingCell in basePaddingCells) {
            grid[basePaddingCell.x, basePaddingCell.y].UnmarkPathCell();
        }
    }
}