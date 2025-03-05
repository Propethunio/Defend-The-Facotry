using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeltManager {

    public static BeltManager Instance { get; private set; }

    public event Action OnBeltAdded;
    public event Action OnBeltRemoved;

    public Dictionary<ConveyorBelt, BeltPath> beltEndsDict { get; private set; } = new();

    GridCell[,] gridArray = BuildingSystem.Instance.grid.gridArray;
    List<BeltPath> beltPathList = new();

    public Transform debugVisualParent { get; private set; }

    public BeltManager(bool showDebug) {
        if(Instance == null) Instance = this;
        else return;

        TimeTickSystem.Instance.OnTick += OnTick;
        TimeTickSystem.Instance.OnSubTick += OnTick;

        if(showDebug) {
            debugVisualParent = new GameObject("Belt Debug Visual").transform;
            new DebugVisual();
        }
    }

    ~BeltManager() {
        TimeTickSystem.Instance.OnTick -= OnTick;
        TimeTickSystem.Instance.OnSubTick -= OnTick;
    }

    void OnTick() {
        int beltPathsCount = beltPathList.Count;

        for(int i = 0; i < beltPathsCount; i++) {
            beltPathList[i].TakeAction();
        }
    }

    public void AddBelt(ConveyorBelt newBelt) {
        BeltPath connectingBeltPath = null;
        ConveyorBelt connectingBelt = TryGetConnectingBelt(newBelt.previousPosition);

        if(connectingBelt != null && connectingBelt.nextPosition == newBelt.origin && (connectingBelt.parentBuilding == null || newBelt.parentBuilding == null)) {
            ConnectToPreviousBelt(newBelt, connectingBelt, ref connectingBeltPath);
        }

        connectingBelt = TryGetConnectingBelt(newBelt.nextPosition);

        if(connectingBelt != null && (connectingBelt.parentBuilding == null || newBelt.parentBuilding == null)) {
            ConnectToNextBelt(newBelt, connectingBelt, ref connectingBeltPath);
        }

        if(connectingBeltPath == null) {
            CreateNewBeltPath(newBelt, ref connectingBeltPath);
        }

        OnBeltAdded?.Invoke();
    }

    ConveyorBelt TryGetConnectingBelt(Vector2Int connectingPosition) {
        if(connectingPosition.x >= 0 && connectingPosition.x < gridArray.GetLength(0) && connectingPosition.y >= 0 && connectingPosition.y < gridArray.GetLength(1)) {
            return gridArray[connectingPosition.x, connectingPosition.y].placedObject as ConveyorBelt;
        }
        return null;
    }

    void ConnectToPreviousBelt(ConveyorBelt newBelt, ConveyorBelt previousBelt, ref BeltPath connectingBeltPath) {
        connectingBeltPath = beltEndsDict[previousBelt];
        connectingBeltPath.beltList.Add(newBelt);
        beltEndsDict.Add(newBelt, connectingBeltPath);

        if(connectingBeltPath.beltList.Count > 2) {
            beltEndsDict.Remove(previousBelt);
        }
    }

    void ConnectToNextBelt(ConveyorBelt newBelt, ConveyorBelt nextBelt, ref BeltPath connectingBeltPath) {
        if(nextBelt.parentBuilding == null && beltEndsDict.ContainsKey(nextBelt) && nextBelt.nextPosition != newBelt.origin) {
            ConveyorBelt beltConnectedToNextBelt = TryGetConnectingBelt(nextBelt.previousPosition);

            if(beltConnectedToNextBelt == null || beltConnectedToNextBelt.nextPosition != nextBelt.origin) {
                nextBelt.previousPosition = newBelt.origin;
            }
        }

        if(nextBelt.previousPosition == newBelt.origin) {
            if(connectingBeltPath != null) {
                MergeBeltPaths(newBelt, nextBelt, connectingBeltPath);
            } else {
                InsertIntoExistingPath(newBelt, nextBelt, ref connectingBeltPath);
            }
        }
    }

    void MergeBeltPaths(ConveyorBelt beltEndFromMainPath, ConveyorBelt beltStartFromDeletedPath) {
        BeltPath mainBeltPath = beltEndsDict[beltEndFromMainPath];
        bool mergedSingleBelt = mainBeltPath.beltList.Count == 1;
        MergeBeltPaths(beltEndFromMainPath, beltStartFromDeletedPath, mainBeltPath);

        if(mergedSingleBelt) {
            beltEndsDict.Add(beltEndFromMainPath, mainBeltPath);
        }
    }

    void MergeBeltPaths(ConveyorBelt endBeltFromMainPath, ConveyorBelt startBeltFromDeletedPath, BeltPath mainBeltPath) {
        BeltPath pathToMerge = beltEndsDict[startBeltFromDeletedPath];
        beltEndsDict.Remove(endBeltFromMainPath);

        if(mainBeltPath == pathToMerge) {
            beltEndsDict.Remove(startBeltFromDeletedPath);
            return;
        }

        mainBeltPath.beltList.AddRange(pathToMerge.beltList);

        if(pathToMerge.beltList.Count > 1) {
            beltEndsDict.Remove(startBeltFromDeletedPath);
        }

        beltEndsDict[pathToMerge.beltList[^1]] = mainBeltPath;
        beltPathList.Remove(pathToMerge);
    }

    void InsertIntoExistingPath(ConveyorBelt newBelt, ConveyorBelt nextBelt, ref BeltPath connectingBeltPath) {
        connectingBeltPath = beltEndsDict[nextBelt];
        connectingBeltPath.beltList.Insert(0, newBelt);
        beltEndsDict.Add(newBelt, connectingBeltPath);

        if(connectingBeltPath.beltList.Count > 2) {
            beltEndsDict.Remove(nextBelt);
        }
    }

    void CreateNewBeltPath(ConveyorBelt newBelt, ref BeltPath connectingBeltPath) {
        connectingBeltPath = new();
        connectingBeltPath.beltList.Add(newBelt);
        beltPathList.Add(connectingBeltPath);
        beltEndsDict.Add(newBelt, connectingBeltPath);
    }

    public void RemoveBelt(ConveyorBelt belt) {
        ConveyorBelt newStartBelt = null;
        ConveyorBelt newEndBelt = null;

        if(!beltEndsDict.TryGetValue(belt, out BeltPath beltPath)) {
            beltPath = beltPathList.FirstOrDefault(path => path.beltList.Contains(belt));
        }

        int beltIndex = beltPath.beltList.IndexOf(belt);

        if(beltPath.beltList[^1].origin == beltPath.beltList[0].previousPosition) {
            RemoveFromLoop(beltIndex, beltPath, ref newStartBelt, ref newEndBelt);
        } else if(beltIndex == 0) {
            RemoveFromPathStart(beltPath, belt, ref newStartBelt, ref newEndBelt);
        } else if(beltIndex == beltPath.beltList.Count - 1) {
            RemoveFromPathEnd(beltIndex, beltPath, belt, ref newStartBelt, ref newEndBelt);
        } else {
            RemoveFromPathMiddle(beltIndex, beltPath, ref newStartBelt, ref newEndBelt);
        }

        if(newStartBelt != null) {
            CheckForNewStartBeltConnections(newStartBelt);
        }

        if(newEndBelt != null) {
            CheckForNewEndBeltConnections(newEndBelt);
        }

        OnBeltRemoved?.Invoke();
    }

    void RemoveFromLoop(int beltIndex, BeltPath beltPath, ref ConveyorBelt newStartBelt, ref ConveyorBelt newEndBelt) {
        if(beltIndex == 0) {
            beltPath.beltList.RemoveAt(0);
        } else if(beltIndex == beltPath.beltList.Count - 1) {
            beltPath.beltList.RemoveAt(beltIndex);
        } else {
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

    void RemoveFromPathStart(BeltPath beltPath, ConveyorBelt belt, ref ConveyorBelt newStartBelt, ref ConveyorBelt newEndBelt) {
        beltPath.beltList.RemoveAt(0);
        beltEndsDict.Remove(belt);

        if(beltPath.beltList.Count > 0) {
            newStartBelt = beltPath.beltList[0];
            beltEndsDict[newStartBelt] = beltPath;
        } else {
            beltPathList.Remove(beltPath);
        }
    }

    void RemoveFromPathEnd(int beltIndex, BeltPath beltPath, ConveyorBelt belt, ref ConveyorBelt newStartBelt, ref ConveyorBelt newEndBelt) {
        beltPath.beltList.RemoveAt(beltIndex);
        beltEndsDict.Remove(belt);

        if(beltPath.beltList.Count >= 2) {
            newEndBelt = beltPath.beltList[^1];
            beltEndsDict[newEndBelt] = beltPath;
        }
    }

    void RemoveFromPathMiddle(int beltIndex, BeltPath beltPath, ref ConveyorBelt newStartBelt, ref ConveyorBelt newEndBelt) {
        List<ConveyorBelt> firstPart = beltPath.beltList.GetRange(0, beltIndex);
        List<ConveyorBelt> secondPart = beltPath.beltList.GetRange(beltIndex + 1, beltPath.beltList.Count - beltIndex - 1);
        beltPath.beltList.Clear();
        beltPath.beltList.AddRange(firstPart);
        beltEndsDict[firstPart[0]] = beltPath;

        if(firstPart.Count >= 2) {
            newEndBelt = firstPart[^1];
            beltEndsDict[newEndBelt] = beltPath;
        }

        BeltPath newSecondPath = new();
        newSecondPath.beltList.AddRange(secondPart);
        beltPathList.Add(newSecondPath);
        newStartBelt = secondPart[0];
        beltEndsDict[newStartBelt] = newSecondPath;

        if(secondPart.Count >= 2) {
            beltEndsDict[secondPart[^1]] = newSecondPath;
        }
    }

    void CheckForNewStartBeltConnections(ConveyorBelt newStartBelt) {
        Vector2Int forwardDirection = newStartBelt.nextPosition - newStartBelt.origin;
        Vector2Int newPreviousPosition = newStartBelt.origin - forwardDirection;

        if(newPreviousPosition != newStartBelt.previousPosition) {

            if(TryToConnectStartBeltStraight(newStartBelt, newPreviousPosition)) {
                return;
            }

            if(TryToConnectStartBeltSides(newStartBelt, forwardDirection)) {
                return;
            }

            SetStartBeltStraightWithoutMerge(newStartBelt, newPreviousPosition);
            return;
        }

        TryToConnectStartBeltSides(newStartBelt, forwardDirection);
    }

    bool TryToConnectStartBeltStraight(ConveyorBelt newStartBelt, Vector2Int newPreviousPosition) {
        ConveyorBelt connectedStraightBelt = TryGetConnectingBelt(newPreviousPosition);

        if(connectedStraightBelt != null && connectedStraightBelt.nextPosition == newStartBelt.origin) {
            ConnectNewStartBeltStraight(newStartBelt, connectedStraightBelt, newPreviousPosition);
            return true;
        }

        return false;
    }

    void ConnectNewStartBeltStraight(ConveyorBelt newStartBelt, ConveyorBelt connectedStraightBelt, Vector2Int newPreviousPosition) {
        SetStartBeltStraightWithoutMerge(newStartBelt, newPreviousPosition);
        MergeBeltPaths(connectedStraightBelt, newStartBelt);
    }

    void SetStartBeltStraightWithoutMerge(ConveyorBelt newStartBelt, Vector2Int newPreviousPosition) {
        newStartBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowStraightVisual();
        newStartBelt.previousPosition = newPreviousPosition;
    }

    bool TryToConnectStartBeltSides(ConveyorBelt newStartBelt, Vector2Int forwardDirection) {
        Vector2Int leftPosition = newStartBelt.origin + new Vector2Int(-forwardDirection.y, forwardDirection.x);
        Vector2Int rightPosition = newStartBelt.origin + new Vector2Int(forwardDirection.y, -forwardDirection.x);
        ConveyorBelt connectedLeftBelt = TryGetConnectingBelt(leftPosition);
        ConveyorBelt connectedRightBelt = TryGetConnectingBelt(rightPosition);

        if(connectedLeftBelt != null && connectedLeftBelt.nextPosition == newStartBelt.origin && (connectedRightBelt == null || connectedRightBelt.nextPosition != newStartBelt.origin)) {
            ConnectNewStartBeltLeft(newStartBelt, connectedLeftBelt, leftPosition);
            return true;
        }

        if(connectedRightBelt != null && connectedRightBelt.nextPosition == newStartBelt.origin && (connectedLeftBelt == null || connectedLeftBelt.nextPosition != newStartBelt.origin)) {
            ConnectNewStartBeltRight(newStartBelt, connectedRightBelt, rightPosition);
            return true;
        }

        return false;
    }

    void ConnectNewStartBeltLeft(ConveyorBelt newStartBelt, ConveyorBelt connectedLeftBelt, Vector2Int leftPosition) {
        newStartBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowLeftVisual();
        newStartBelt.previousPosition = leftPosition;
        MergeBeltPaths(connectedLeftBelt, newStartBelt);
    }

    void ConnectNewStartBeltRight(ConveyorBelt newStartBelt, ConveyorBelt connectedRightBelt, Vector2Int rightPosition) {
        newStartBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowRightVisual();
        newStartBelt.previousPosition = rightPosition;
        MergeBeltPaths(connectedRightBelt, newStartBelt);
    }

    void CheckForNewEndBeltConnections(ConveyorBelt newEndBelt) {
        Vector2Int backDirection = newEndBelt.origin - newEndBelt.previousPosition;
        Vector2Int newNextPosition = newEndBelt.origin + backDirection;

        if(newNextPosition != newEndBelt.nextPosition) {

            if(TryToConnectEndBeltStraight(newEndBelt, newNextPosition)) {
                return;
            }

            if(TryToConnectEndBeltSides(newEndBelt, backDirection)) {
                return;
            }

            ConveyorBelt newEndBeltConnectedBelt = gridArray[newEndBelt.previousPosition.x, newEndBelt.previousPosition.y].placedObject as ConveyorBelt;
            SetEndBeltStraightWithoutMerge(newEndBelt, newNextPosition, newEndBeltConnectedBelt.dir);
            return;
        }

        TryToConnectEndBeltSides(newEndBelt, backDirection);
    }

    bool TryToConnectEndBeltStraight(ConveyorBelt newEndBelt, Vector2Int newNextPosition) {
        ConveyorBelt connectedStraightBelt = TryGetConnectingBelt(newNextPosition);

        if(connectedStraightBelt != null && connectedStraightBelt.previousPosition == newEndBelt.origin) {
            ConnectNewEndBeltStraight(newEndBelt, connectedStraightBelt, newNextPosition);
            return true;
        }

        return false;
    }

    void ConnectNewEndBeltStraight(ConveyorBelt newEndBelt, ConveyorBelt connectedStraightBelt, Vector2Int newNextPosition) {
        SetEndBeltStraightWithoutMerge(newEndBelt, newNextPosition, connectedStraightBelt.dir);
        MergeBeltPaths(newEndBelt, connectedStraightBelt);
    }

    void SetEndBeltStraightWithoutMerge(ConveyorBelt newEndBelt, Vector2Int newNextPosition, BuildingDir dir) {
        newEndBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowStraightVisual();
        newEndBelt.transform.rotation = GetRotation(dir);
        Vector2Int rotationOffset = GetRotationOffset(dir);
        newEndBelt.transform.position = new Vector3(newEndBelt.origin.x, 0, newEndBelt.origin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y);
        newEndBelt.nextPosition = newNextPosition;
        newEndBelt.dir = dir;
    }

    Quaternion GetRotation(BuildingDir dir) {
        switch(dir) {
            default:
            case BuildingDir.Down: return Quaternion.Euler(0, 0, 0);
            case BuildingDir.Left: return Quaternion.Euler(0, 90, 0);
            case BuildingDir.Up: return Quaternion.Euler(0, 180, 0);
            case BuildingDir.Right: return Quaternion.Euler(0, 270, 0);
        }
    }

    Vector2Int GetRotationOffset(BuildingDir dir) {
        switch(dir) {
            default:
            case BuildingDir.Down: return Vector2Int.zero;
            case BuildingDir.Left: return new Vector2Int(0, 1);
            case BuildingDir.Up: return Vector2Int.one;
            case BuildingDir.Right: return new Vector2Int(1, 0);
        }
    }

    bool TryToConnectEndBeltSides(ConveyorBelt newEndBelt, Vector2Int backDirection) {
        Vector2Int leftPosition = newEndBelt.origin + new Vector2Int(-backDirection.y, backDirection.x);
        Vector2Int rightPosition = newEndBelt.origin + new Vector2Int(backDirection.y, -backDirection.x);
        ConveyorBelt connectedLeftBelt = TryGetConnectingBelt(leftPosition);
        ConveyorBelt connectedRightBelt = TryGetConnectingBelt(rightPosition);

        if(connectedLeftBelt != null && connectedLeftBelt.previousPosition == newEndBelt.origin && (connectedRightBelt == null || connectedRightBelt.previousPosition != newEndBelt.origin)) {
            ConnectNewEndBeltLeft(newEndBelt, connectedLeftBelt, leftPosition);
            return true;
        }

        if(connectedRightBelt != null && connectedRightBelt.previousPosition == newEndBelt.origin && (connectedLeftBelt == null || connectedLeftBelt.previousPosition != newEndBelt.origin)) {
            ConnectNewEndBeltRight(newEndBelt, connectedRightBelt, rightPosition);
            return true;
        }

        return false;
    }

    void ConnectNewEndBeltLeft(ConveyorBelt newEndBelt, ConveyorBelt connectedLeftBelt, Vector2Int leftPosition) {
        newEndBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowLeftVisual();
        newEndBelt.transform.rotation = GetRotation(connectedLeftBelt.dir);
        Vector2Int rotationOffset = GetRotationOffset(connectedLeftBelt.dir);
        newEndBelt.transform.position = new Vector3(newEndBelt.origin.x, 0, newEndBelt.origin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y);
        newEndBelt.nextPosition = leftPosition;
        newEndBelt.dir = connectedLeftBelt.dir;
        MergeBeltPaths(newEndBelt, connectedLeftBelt);
    }

    void ConnectNewEndBeltRight(ConveyorBelt newEndBelt, ConveyorBelt connectedRightBelt, Vector2Int rightPosition) {
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
            if(beltList[^1].origin == beltList[0].previousPosition) {
                ExecuteLoopActions();
            } else {
                ExecuteStandardActions();
            }
        }

        void ExecuteLoopActions() {
            List<ConveyorBelt> beltsToRepeat = new();
            int beltStopIndex = beltList.Count - 2;

            if(beltList[^1].TakeActionOnFirstLoopedBelt(out bool didMovedItem, beltList[0])) {
                beltsToRepeat.Add(beltList[^1]);

                for(int i = beltStopIndex; i >= 0; i--) {
                    if(beltList[i].TakeActionWithShouldRepeatFeedback(beltList[i + 1])) {
                        beltsToRepeat.Add(beltList[i]);
                    } else {
                        beltStopIndex = i - 1;
                        break;
                    }
                }
            }

            // ✅ Ensure beltStopIndex is within valid range before using
            if(beltStopIndex < 0) beltStopIndex = beltList.Count - 1; // Loops back if necessary
            int nextIndex = (beltStopIndex + 1) % beltList.Count; // ✅ Safe looping index

            bool movedToNext = beltList[beltStopIndex].TakeActionOnFirstBeltAfterStop(beltList[nextIndex]);

            for(int i = beltStopIndex - 1; i > 0; i--) {
                beltList[i].TakeAction(beltList[i + 1]);
            }

            beltList[0].TakeActionOnLastLoopedBelt(didMovedItem, beltList[1]);

            // ✅ Fixes Off-by-One Error
            for(int i = 1; i < beltsToRepeat.Count; i++) {
                beltsToRepeat[i].TakeAction(beltsToRepeat[i - 1]);
            }

            // ✅ Prevents Out-of-Bounds Access
            if(beltsToRepeat.Count >= 2) {
                if(movedToNext) {
                    beltsToRepeat[^1].TakeActionOnLastRepeatBelt(beltsToRepeat[^2]);
                } else {
                    beltsToRepeat[^1].TakeAction(beltsToRepeat[^2]);
                }
            } else if(beltsToRepeat.Count == 1) {
                beltsToRepeat[0].TakeAction(null);
            }
        }

        void ExecuteStandardActions() {
            beltList[^1].TakeLastBeltStandardAction();

            for(int i = beltList.Count - 2; i >= 0; i--) {
                beltList[i].TakeAction(beltList[i + 1]);
            }
        }
    }

    /* --------------- BELT DEBUG VISUAL --------------- */

    class DebugVisual {

        readonly List<BeltPathDebugVisual> beltPathDebugVisualList = new();

        public DebugVisual() {
            Instance.OnBeltAdded += Instance_OnBeltAdded;
            Instance.OnBeltRemoved += Instance_OnBeltRemoved;
        }

        ~DebugVisual() {
            Instance.OnBeltAdded -= Instance_OnBeltAdded;
            Instance.OnBeltRemoved -= Instance_OnBeltRemoved;
        }

        void Instance_OnBeltAdded() {
            RefreshVisual();
        }

        void Instance_OnBeltRemoved() {
            RefreshVisual();
        }

        void RefreshVisual() {
            foreach(BeltPathDebugVisual beltPathDebugVisual in beltPathDebugVisualList) {
                beltPathDebugVisual.DestroySelf();
            }

            beltPathDebugVisualList.Clear();
            int pathNumber = 0;

            foreach(BeltPath beltPath in Instance.beltPathList) {
                pathNumber++;
                beltPathDebugVisualList.Add(new BeltPathDebugVisual(beltPath, pathNumber));
            }
        }
    }

    class BeltPathDebugVisual {

        readonly Transform pathParent;

        public BeltPathDebugVisual(BeltPath beltPath, int pathNumber) {
            pathParent = new GameObject($"Path: {pathNumber}").transform;
            pathParent.parent = Instance.debugVisualParent;

            Vector2Int gridPosition = beltPath.beltList[0].origin;
            Transform nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity, pathParent);

            if(beltPath.beltList.Count == 1) {
                nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.cyan;
                pathParent.position += new Vector3(0, .33f, 0);
                return;
            } else {
                nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.green;
            }

            gridPosition = beltPath.beltList[^1].origin;
            nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity, pathParent);
            nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.red;

            for(int i = 0; i < beltPath.beltList.Count - 1; i++) {
                ConveyorBelt belt = beltPath.beltList[i];
                ConveyorBelt nextBelt = beltPath.beltList[i + 1];
                gridPosition = belt.origin;
                Vector2Int nextGridPosition = nextBelt.origin;

                if(i > 0) {
                    nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity, pathParent);
                    nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.blue;
                }

                nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualLine, BuildingSystem.Instance.GetWorldPosition(gridPosition) + new Vector3(.5f, 0, .5f), Quaternion.identity, pathParent);
                nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.yellow;
                Vector3 dirToNextBelt = (BuildingSystem.Instance.GetWorldPosition(nextGridPosition) - BuildingSystem.Instance.GetWorldPosition(gridPosition)).normalized;
                nodeVisual.eulerAngles = new Vector3(0, -CodeMonkey.Utils.UtilsClass.GetAngleFromVectorFloat3D(dirToNextBelt), 0);
            }

            if(beltPath.beltList[^1].nextPosition == beltPath.beltList[0].origin && beltPath.beltList[^1].origin == beltPath.beltList[0].previousPosition) {
                gridPosition = beltPath.beltList[^1].origin;
                nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualLine, BuildingSystem.Instance.GetWorldPosition(gridPosition) + new Vector3(.5f, 0, .5f), Quaternion.identity, pathParent);
                nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.yellow;
                Vector3 dirToNextBelt = (BuildingSystem.Instance.GetWorldPosition(beltPath.beltList[0].origin) - BuildingSystem.Instance.GetWorldPosition(gridPosition)).normalized;
                nodeVisual.eulerAngles = new Vector3(0, -CodeMonkey.Utils.UtilsClass.GetAngleFromVectorFloat3D(dirToNextBelt), 0);
            }

            pathParent.position += new Vector3(0, .33f, 0);
        }

        public void DestroySelf() {
            GameObject.Destroy(pathParent.gameObject);
        }
    }
}