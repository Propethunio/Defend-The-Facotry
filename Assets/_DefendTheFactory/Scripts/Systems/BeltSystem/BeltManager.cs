using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilsClass;

public class BeltManager {
    public event Action OnBeltAdded;
    public event Action OnBeltRemoved;

    public Dictionary<ConveyorBelt, BeltPath> beltEndsDict { get; private set; } = new();

    private GridCell[,] gridArray;
    private List<BeltPath> beltPathList = new();
    private Transform debugVisualParent;

    public void Init(bool showDebug) {
        gridArray = Injector.Resolve<BuildingSystem>().grid.gridArray;
        Injector.Resolve<TimeTickSystem>().OnTick += OnTick;
        if (!showDebug) return;

        debugVisualParent = new GameObject("Belt Debug Visual").transform;
        new DebugVisual();
    }

    ~BeltManager() {
        Injector.Resolve<TimeTickSystem>().OnTick -= OnTick;
    }

    private void OnTick() {
        int beltPathsCount = beltPathList.Count;

        for (int i = 0; i < beltPathsCount; i++) {
            beltPathList[i].TakeAction();
        }
    }

    public void AddBelt(ConveyorBelt newBelt) {
        BeltPath connectingBeltPath = null;
        ConveyorBelt connectingBelt = TryGetConnectingBelt(newBelt.previousPosition);

        if (connectingBelt != null && connectingBelt.nextPosition == newBelt.origin && (connectingBelt.parentBuilding == null || newBelt.parentBuilding == null)) {
            ConnectToPreviousBelt(newBelt, connectingBelt, ref connectingBeltPath);
        }

        connectingBelt = TryGetConnectingBelt(newBelt.nextPosition);

        if (connectingBelt != null && (connectingBelt.parentBuilding == null || newBelt.parentBuilding == null)) {
            ConnectToNextBelt(newBelt, connectingBelt, ref connectingBeltPath);
        }

        if (connectingBeltPath == null) {
            CreateNewBeltPath(newBelt, ref connectingBeltPath);
        }

        OnBeltAdded?.Invoke();
    }

    private ConveyorBelt TryGetConnectingBelt(Vector2Int connectingPosition) {
        if (connectingPosition.x >= 0 && connectingPosition.x < gridArray.GetLength(0) && connectingPosition.y >= 0 && connectingPosition.y < gridArray.GetLength(1)) {
            return gridArray[connectingPosition.x, connectingPosition.y].placedObject as ConveyorBelt;
        }

        return null;
    }

    private void ConnectToPreviousBelt(ConveyorBelt newBelt, ConveyorBelt previousBelt, ref BeltPath connectingBeltPath) {
        connectingBeltPath = beltEndsDict[previousBelt];
        connectingBeltPath.beltList.Add(newBelt);
        beltEndsDict.Add(newBelt, connectingBeltPath);

        if (connectingBeltPath.beltList.Count > 2) {
            beltEndsDict.Remove(previousBelt);
        }
    }

    private void ConnectToNextBelt(ConveyorBelt newBelt, ConveyorBelt nextBelt, ref BeltPath connectingBeltPath) {
        if (nextBelt.parentBuilding == null && beltEndsDict.ContainsKey(nextBelt) && nextBelt.nextPosition != newBelt.origin) {
            ConveyorBelt beltConnectedToNextBelt = TryGetConnectingBelt(nextBelt.previousPosition);

            if (beltConnectedToNextBelt == null || beltConnectedToNextBelt.nextPosition != nextBelt.origin) {
                nextBelt.previousPosition = newBelt.origin;
            }
        }

        if (nextBelt.previousPosition != newBelt.origin) return;

        if (connectingBeltPath != null) {
            MergeBeltPaths(newBelt, nextBelt, connectingBeltPath);
        }
        else {
            InsertIntoExistingPath(newBelt, nextBelt, ref connectingBeltPath);
        }
    }

    private void MergeBeltPaths(ConveyorBelt beltEndFromMainPath, ConveyorBelt beltStartFromDeletedPath) {
        BeltPath mainBeltPath = beltEndsDict[beltEndFromMainPath];
        bool mergedSingleBelt = mainBeltPath.beltList.Count == 1;
        MergeBeltPaths(beltEndFromMainPath, beltStartFromDeletedPath, mainBeltPath);

        if (mergedSingleBelt) {
            beltEndsDict.Add(beltEndFromMainPath, mainBeltPath);
        }
    }

    private void MergeBeltPaths(ConveyorBelt endBeltFromMainPath, ConveyorBelt startBeltFromDeletedPath, BeltPath mainBeltPath) {
        BeltPath pathToMerge = beltEndsDict[startBeltFromDeletedPath];
        beltEndsDict.Remove(endBeltFromMainPath);

        if (mainBeltPath == pathToMerge) {
            beltEndsDict.Remove(startBeltFromDeletedPath);
            return;
        }

        mainBeltPath.beltList.AddRange(pathToMerge.beltList);

        if (pathToMerge.beltList.Count > 1) {
            beltEndsDict.Remove(startBeltFromDeletedPath);
        }

        beltEndsDict[pathToMerge.beltList[^1]] = mainBeltPath;
        beltPathList.Remove(pathToMerge);
    }

    private void InsertIntoExistingPath(ConveyorBelt newBelt, ConveyorBelt nextBelt, ref BeltPath connectingBeltPath) {
        connectingBeltPath = beltEndsDict[nextBelt];
        connectingBeltPath.beltList.Insert(0, newBelt);
        beltEndsDict.Add(newBelt, connectingBeltPath);

        if (connectingBeltPath.beltList.Count > 2) {
            beltEndsDict.Remove(nextBelt);
        }
    }

    private void CreateNewBeltPath(ConveyorBelt newBelt, ref BeltPath connectingBeltPath) {
        connectingBeltPath = new();
        connectingBeltPath.beltList.Add(newBelt);
        beltPathList.Add(connectingBeltPath);
        beltEndsDict.Add(newBelt, connectingBeltPath);
    }

    public void RemoveBelt(ConveyorBelt belt) {
        ConveyorBelt newStartBelt = null;
        ConveyorBelt newEndBelt = null;

        if (!beltEndsDict.TryGetValue(belt, out BeltPath beltPath)) {
            beltPath = beltPathList.FirstOrDefault(path => path.beltList.Contains(belt));
        }

        int beltIndex = beltPath.beltList.IndexOf(belt);

        if (beltPath.beltList[^1].origin == beltPath.beltList[0].previousPosition) {
            RemoveFromLoop(beltIndex, beltPath, ref newStartBelt, ref newEndBelt);
        }
        else if (beltIndex == 0) {
            RemoveFromPathStart(beltPath, belt, ref newStartBelt);
        }
        else if (beltIndex == beltPath.beltList.Count - 1) {
            RemoveFromPathEnd(beltIndex, beltPath, belt, ref newEndBelt);
        }
        else {
            RemoveFromPathMiddle(beltIndex, beltPath, ref newStartBelt, ref newEndBelt);
        }

        if (newStartBelt != null) {
            CheckForNewStartBeltConnections(newStartBelt);
        }

        if (newEndBelt != null) {
            CheckForNewEndBeltConnections(newEndBelt);
        }

        OnBeltRemoved?.Invoke();
    }

    private void RemoveFromLoop(int beltIndex, BeltPath beltPath, ref ConveyorBelt newStartBelt, ref ConveyorBelt newEndBelt) {
        if (beltIndex == 0) {
            beltPath.beltList.RemoveAt(0);
        }
        else if (beltIndex == beltPath.beltList.Count - 1) {
            beltPath.beltList.RemoveAt(beltIndex);
        }
        else {
            List<ConveyorBelt> firstPart = beltPath.beltList.GetRange(0, beltIndex);
            List<ConveyorBelt> secondPart = beltPath.beltList.GetRange(beltIndex + 1, beltPath.beltList.Count - beltIndex - 1);
            beltPath.beltList.Clear();
            beltPath.beltList.AddRange(secondPart);
            beltPath.beltList.AddRange(firstPart);
        }

        newStartBelt = beltPath.beltList[0];
        newEndBelt = beltPath.beltList[^1];
        beltEndsDict.Add(newStartBelt, beltPath);
        beltEndsDict.Add(newEndBelt, beltPath);
    }

    private void RemoveFromPathStart(BeltPath beltPath, ConveyorBelt belt, ref ConveyorBelt newStartBelt) {
        beltPath.beltList.RemoveAt(0);
        beltEndsDict.Remove(belt);

        if (beltPath.beltList.Count > 0) {
            newStartBelt = beltPath.beltList[0];
            beltEndsDict[newStartBelt] = beltPath;
        }
        else {
            beltPathList.Remove(beltPath);
        }
    }

    private void RemoveFromPathEnd(int beltIndex, BeltPath beltPath, ConveyorBelt belt, ref ConveyorBelt newEndBelt) {
        beltPath.beltList.RemoveAt(beltIndex);
        beltEndsDict.Remove(belt);

        if (beltPath.beltList.Count < 2) return;

        newEndBelt = beltPath.beltList[^1];
        beltEndsDict[newEndBelt] = beltPath;
    }

    private void RemoveFromPathMiddle(int beltIndex, BeltPath beltPath, ref ConveyorBelt newStartBelt, ref ConveyorBelt newEndBelt) {
        List<ConveyorBelt> firstPart = beltPath.beltList.GetRange(0, beltIndex);
        List<ConveyorBelt> secondPart = beltPath.beltList.GetRange(beltIndex + 1, beltPath.beltList.Count - beltIndex - 1);
        beltPath.beltList.Clear();
        beltPath.beltList.AddRange(firstPart);
        beltEndsDict[firstPart[0]] = beltPath;

        if (firstPart.Count >= 2) {
            newEndBelt = firstPart[^1];
            beltEndsDict[newEndBelt] = beltPath;
        }

        BeltPath newSecondPath = new();
        newSecondPath.beltList.AddRange(secondPart);
        beltPathList.Add(newSecondPath);
        newStartBelt = secondPart[0];
        beltEndsDict[newStartBelt] = newSecondPath;

        if (secondPart.Count >= 2) {
            beltEndsDict[secondPart[^1]] = newSecondPath;
        }
    }

    public void CheckForNewStartBeltConnections(ConveyorBelt newStartBelt) {
        Vector2Int forwardDirection = newStartBelt.nextPosition - newStartBelt.origin;
        Vector2Int newPreviousPosition = newStartBelt.origin - forwardDirection;

        if (newPreviousPosition != newStartBelt.previousPosition) {
            if (TryToConnectStartBeltStraight(newStartBelt, newPreviousPosition)) {
                return;
            }

            if (TryToConnectStartBeltSides(newStartBelt, forwardDirection)) {
                return;
            }

            SetStartBeltStraightWithoutMerge(newStartBelt, newPreviousPosition);
            return;
        }

        TryToConnectStartBeltSides(newStartBelt, forwardDirection);
    }

    private bool TryToConnectStartBeltStraight(ConveyorBelt newStartBelt, Vector2Int newPreviousPosition) {
        ConveyorBelt connectedStraightBelt = TryGetConnectingBelt(newPreviousPosition);

        if (connectedStraightBelt == null || connectedStraightBelt.nextPosition != newStartBelt.origin) return false;

        ConnectNewStartBeltStraight(newStartBelt, connectedStraightBelt, newPreviousPosition);
        return true;
    }

    private void ConnectNewStartBeltStraight(ConveyorBelt newStartBelt, ConveyorBelt connectedStraightBelt, Vector2Int newPreviousPosition) {
        SetStartBeltStraightWithoutMerge(newStartBelt, newPreviousPosition);
        MergeBeltPaths(connectedStraightBelt, newStartBelt);
    }

    private void SetStartBeltStraightWithoutMerge(ConveyorBelt newStartBelt, Vector2Int newPreviousPosition) {
        newStartBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowStraightVisual();
        newStartBelt.previousPosition = newPreviousPosition;
    }

    private bool TryToConnectStartBeltSides(ConveyorBelt newStartBelt, Vector2Int forwardDirection) {
        Vector2Int leftPosition = newStartBelt.origin + new Vector2Int(-forwardDirection.y, forwardDirection.x);
        Vector2Int rightPosition = newStartBelt.origin + new Vector2Int(forwardDirection.y, -forwardDirection.x);
        ConveyorBelt connectedLeftBelt = TryGetConnectingBelt(leftPosition);
        ConveyorBelt connectedRightBelt = TryGetConnectingBelt(rightPosition);

        if (connectedLeftBelt != null && connectedLeftBelt.nextPosition == newStartBelt.origin && (connectedRightBelt == null || connectedRightBelt.nextPosition != newStartBelt.origin)) {
            ConnectNewStartBeltLeft(newStartBelt, connectedLeftBelt, leftPosition);
            return true;
        }

        if (connectedRightBelt == null || connectedRightBelt.nextPosition != newStartBelt.origin || (connectedLeftBelt != null && connectedLeftBelt.nextPosition == newStartBelt.origin))
            return false;

        ConnectNewStartBeltRight(newStartBelt, connectedRightBelt, rightPosition);
        return true;
    }

    private void ConnectNewStartBeltLeft(ConveyorBelt newStartBelt, ConveyorBelt connectedLeftBelt, Vector2Int leftPosition) {
        newStartBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowLeftVisual();
        newStartBelt.previousPosition = leftPosition;
        MergeBeltPaths(connectedLeftBelt, newStartBelt);
    }

    private void ConnectNewStartBeltRight(ConveyorBelt newStartBelt, ConveyorBelt connectedRightBelt, Vector2Int rightPosition) {
        newStartBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowRightVisual();
        newStartBelt.previousPosition = rightPosition;
        MergeBeltPaths(connectedRightBelt, newStartBelt);
    }

    private void CheckForNewEndBeltConnections(ConveyorBelt newEndBelt) {
        Vector2Int backDirection = newEndBelt.origin - newEndBelt.previousPosition;
        Vector2Int newNextPosition = newEndBelt.origin + backDirection;

        if (newNextPosition != newEndBelt.nextPosition) {
            if (TryToConnectEndBeltStraight(newEndBelt, newNextPosition)) {
                return;
            }

            if (TryToConnectEndBeltSides(newEndBelt, backDirection)) {
                return;
            }

            ConveyorBelt newEndBeltConnectedBelt = gridArray[newEndBelt.previousPosition.x, newEndBelt.previousPosition.y].placedObject as ConveyorBelt;
            SetEndBeltStraightWithoutMerge(newEndBelt, newNextPosition, newEndBeltConnectedBelt.dir);
            return;
        }

        TryToConnectEndBeltSides(newEndBelt, backDirection);
    }

    private bool TryToConnectEndBeltStraight(ConveyorBelt newEndBelt, Vector2Int newNextPosition) {
        ConveyorBelt connectedStraightBelt = TryGetConnectingBelt(newNextPosition);

        if (connectedStraightBelt == null || connectedStraightBelt.previousPosition != newEndBelt.origin) return false;

        ConnectNewEndBeltStraight(newEndBelt, connectedStraightBelt, newNextPosition);
        return true;
    }

    private void ConnectNewEndBeltStraight(ConveyorBelt newEndBelt, ConveyorBelt connectedStraightBelt, Vector2Int newNextPosition) {
        SetEndBeltStraightWithoutMerge(newEndBelt, newNextPosition, connectedStraightBelt.dir);
        MergeBeltPaths(newEndBelt, connectedStraightBelt);
    }

    private void SetEndBeltStraightWithoutMerge(ConveyorBelt newEndBelt, Vector2Int newNextPosition, BuildingDir dir) {
        newEndBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowStraightVisual();
        newEndBelt.transform.rotation = GetRotation(dir);
        Vector2Int rotationOffset = GetRotationOffset(dir);
        newEndBelt.transform.position = new Vector3(newEndBelt.origin.x, 0, newEndBelt.origin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y);
        newEndBelt.nextPosition = newNextPosition;
        newEndBelt.dir = dir;
    }

    private Quaternion GetRotation(BuildingDir dir) {
        switch (dir) {
            default:
            case BuildingDir.Down: return Quaternion.Euler(0, 0, 0);
            case BuildingDir.Left: return Quaternion.Euler(0, 90, 0);
            case BuildingDir.Up: return Quaternion.Euler(0, 180, 0);
            case BuildingDir.Right: return Quaternion.Euler(0, 270, 0);
        }
    }

    private Vector2Int GetRotationOffset(BuildingDir dir) {
        switch (dir) {
            default:
            case BuildingDir.Down: return Vector2Int.zero;
            case BuildingDir.Left: return new Vector2Int(0, 1);
            case BuildingDir.Up: return Vector2Int.one;
            case BuildingDir.Right: return new Vector2Int(1, 0);
        }
    }

    private bool TryToConnectEndBeltSides(ConveyorBelt newEndBelt, Vector2Int backDirection) {
        Vector2Int leftPosition = newEndBelt.origin + new Vector2Int(-backDirection.y, backDirection.x);
        Vector2Int rightPosition = newEndBelt.origin + new Vector2Int(backDirection.y, -backDirection.x);
        ConveyorBelt connectedLeftBelt = TryGetConnectingBelt(leftPosition);
        ConveyorBelt connectedRightBelt = TryGetConnectingBelt(rightPosition);

        if (connectedLeftBelt != null && connectedLeftBelt.previousPosition == newEndBelt.origin && (connectedRightBelt == null || connectedRightBelt.previousPosition != newEndBelt.origin)) {
            ConnectNewEndBeltLeft(newEndBelt, connectedLeftBelt, leftPosition);
            return true;
        }

        if (connectedRightBelt == null || connectedRightBelt.previousPosition != newEndBelt.origin || (connectedLeftBelt != null && connectedLeftBelt.previousPosition == newEndBelt.origin))
            return false;

        ConnectNewEndBeltRight(newEndBelt, connectedRightBelt, rightPosition);
        return true;
    }

    private void ConnectNewEndBeltLeft(ConveyorBelt newEndBelt, ConveyorBelt connectedLeftBelt, Vector2Int leftPosition) {
        newEndBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowLeftVisual();
        newEndBelt.transform.rotation = GetRotation(connectedLeftBelt.dir);
        Vector2Int rotationOffset = GetRotationOffset(connectedLeftBelt.dir);
        newEndBelt.transform.position = new Vector3(newEndBelt.origin.x, 0, newEndBelt.origin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y);
        newEndBelt.nextPosition = leftPosition;
        newEndBelt.dir = connectedLeftBelt.dir;
        MergeBeltPaths(newEndBelt, connectedLeftBelt);
    }

    private void ConnectNewEndBeltRight(ConveyorBelt newEndBelt, ConveyorBelt connectedRightBelt, Vector2Int rightPosition) {
        newEndBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowRightVisual();
        newEndBelt.transform.rotation = GetRotation(connectedRightBelt.dir);
        Vector2Int rotationOffset = GetRotationOffset(connectedRightBelt.dir);
        newEndBelt.transform.position = new Vector3(newEndBelt.origin.x, 0, newEndBelt.origin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y);
        newEndBelt.nextPosition = rightPosition;
        newEndBelt.dir = connectedRightBelt.dir;
        MergeBeltPaths(newEndBelt, connectedRightBelt);
    }

    public class BeltPath {
        public List<ConveyorBelt> beltList { get; private set; } = new();

        public void TakeAction() {
            if (beltList[^1].origin == beltList[0].previousPosition) {
                ExecuteLoopActions();
            }
            else {
                ExecuteStandardActions();
            }
        }

        private void ExecuteLoopActions() {
            List<ConveyorBelt> beltsToRepeat = new();
            int beltStopIndex = beltList.Count - 2;

            if (beltList[^1].TakeActionOnFirstLoopedBelt(out bool didMovedItem, beltList[0])) {
                beltsToRepeat.Add(beltList[^1]);

                for (int i = beltStopIndex; i >= 0; i--) {
                    if (beltList[i].TakeActionWithShouldRepeatFeedback(beltList[i + 1])) {
                        beltsToRepeat.Add(beltList[i]);
                    }
                    else {
                        beltStopIndex = i - 1;
                        break;
                    }
                }
            }

            // ✅ Ensure beltStopIndex is within valid range before using
            if (beltStopIndex < 0) beltStopIndex = beltList.Count - 1; // Loops back if necessary
            int nextIndex = (beltStopIndex + 1) % beltList.Count; // ✅ Safe looping index

            bool movedToNext = beltList[beltStopIndex].TakeActionOnFirstBeltAfterStop(beltList[nextIndex]);

            for (int i = beltStopIndex - 1; i > 0; i--) {
                beltList[i].TakeAction(beltList[i + 1]);
            }

            beltList[0].TakeActionOnLastLoopedBelt(didMovedItem, beltList[1]);

            // ✅ Fixes Off-by-One Error
            for (int i = 1; i < beltsToRepeat.Count; i++) {
                beltsToRepeat[i].TakeAction(beltsToRepeat[i - 1]);
            }

            // ✅ Prevents Out-of-Bounds Access
            if (beltsToRepeat.Count >= 2) {
                if (movedToNext) {
                    beltsToRepeat[^1].TakeActionOnLastRepeatBelt(beltsToRepeat[^2]);
                }
                else {
                    beltsToRepeat[^1].TakeAction(beltsToRepeat[^2]);
                }
            }
            else if (beltsToRepeat.Count == 1) {
                beltsToRepeat[0].TakeAction(null);
            }
        }

        private void ExecuteStandardActions() {
            beltList[^1].TakeLastBeltStandardAction();

            for (int i = beltList.Count - 2; i >= 0; i--) {
                beltList[i].TakeAction(beltList[i + 1]);
            }
        }
    }

    public void RefreshDebug() {
        OnBeltAdded?.Invoke();
    }

    /* ------------------------------------------------------- */
    /* ------------------ BELT DEBUG VISUAL ------------------ */
    /* ------------------------------------------------------- */

    private class DebugVisual {
        private readonly List<BeltPathDebugVisual> beltPathDebugVisualList = new();
        private BeltManager Instance = Injector.Resolve<BeltManager>();

        public DebugVisual() {
            Instance.OnBeltAdded += Instance_OnBeltAdded;
            Instance.OnBeltRemoved += Instance_OnBeltRemoved;
        }

        ~DebugVisual() {
            Instance.OnBeltAdded -= Instance_OnBeltAdded;
            Instance.OnBeltRemoved -= Instance_OnBeltRemoved;
        }

        private void Instance_OnBeltAdded() {
            RefreshVisual();
        }

        private void Instance_OnBeltRemoved() {
            RefreshVisual();
        }

        private void RefreshVisual() {
            foreach (BeltPathDebugVisual beltPathDebugVisual in beltPathDebugVisualList) {
                beltPathDebugVisual.DestroySelf();
            }

            beltPathDebugVisualList.Clear();
            int pathNumber = 0;

            foreach (BeltPath beltPath in Instance.beltPathList) {
                pathNumber++;
                beltPathDebugVisualList.Add(new BeltPathDebugVisual(beltPath, pathNumber));
            }
        }
    }

    private class BeltPathDebugVisual {
        private readonly Transform pathParent;
        private BeltManager Instance = Injector.Resolve<BeltManager>();

        public BeltPathDebugVisual(BeltPath beltPath, int pathNumber) {
            pathParent = new GameObject($"Path: {pathNumber}").transform;
            pathParent.parent = Instance.debugVisualParent;

            Vector2Int gridPosition = beltPath.beltList[0].origin;
            Transform nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, Injector.Resolve<BuildingSystem>().GetWorldPosition(gridPosition), Quaternion.identity, pathParent);

            if (beltPath.beltList.Count == 1) {
                nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.magenta;
                pathParent.position += new Vector3(0, .33f, 0);
                return;
            }

            nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = beltPath.beltList[0].parentBuilding is LogisticMachine<BaseBuildableObjectSO> ? Color.cyan : Color.green;

            gridPosition = beltPath.beltList[^1].origin;
            nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, Injector.Resolve<BuildingSystem>().GetWorldPosition(gridPosition), Quaternion.identity, pathParent);

            nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Instance.gridArray[beltPath.beltList[^1].nextPosition.x, beltPath.beltList[^1].nextPosition.y].placedObject is LogisticMachine<BaseBuildableObjectSO> ? Color.black : Color.red;

            for (int i = 0; i < beltPath.beltList.Count - 1; i++) {
                ConveyorBelt belt = beltPath.beltList[i];
                ConveyorBelt nextBelt = beltPath.beltList[i + 1];
                gridPosition = belt.origin;
                Vector2Int nextGridPosition = nextBelt.origin;

                if (i > 0) {
                    nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, Injector.Resolve<BuildingSystem>().GetWorldPosition(gridPosition), Quaternion.identity, pathParent);
                    nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.blue;
                }

                nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualLine, Injector.Resolve<BuildingSystem>().GetWorldPosition(gridPosition) + new Vector3(.5f, 0, .5f), Quaternion.identity, pathParent);
                nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.yellow;
                Vector3 dirToNextBelt = (Injector.Resolve<BuildingSystem>().GetWorldPosition(nextGridPosition) - Injector.Resolve<BuildingSystem>().GetWorldPosition(gridPosition)).normalized;
                nodeVisual.eulerAngles = new Vector3(0, -MyUtils.GetAngleFromVectorFloat3D(dirToNextBelt), 0);
            }

            if (beltPath.beltList[^1].nextPosition == beltPath.beltList[0].origin && beltPath.beltList[^1].origin == beltPath.beltList[0].previousPosition) {
                gridPosition = beltPath.beltList[^1].origin;
                nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualLine, Injector.Resolve<BuildingSystem>().GetWorldPosition(gridPosition) + new Vector3(.5f, 0, .5f), Quaternion.identity, pathParent);
                nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.yellow;
                Vector3 dirToNextBelt = (Injector.Resolve<BuildingSystem>().GetWorldPosition(beltPath.beltList[0].origin) - Injector.Resolve<BuildingSystem>().GetWorldPosition(gridPosition)).normalized;
                nodeVisual.eulerAngles = new Vector3(0, -MyUtils.GetAngleFromVectorFloat3D(dirToNextBelt), 0);
            }

            pathParent.position += new Vector3(0, .33f, 0);
        }

        public void DestroySelf() {
            GameObject.Destroy(pathParent.gameObject);
        }
    }
}