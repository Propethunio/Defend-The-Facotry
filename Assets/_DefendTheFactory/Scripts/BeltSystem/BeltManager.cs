using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

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
        if(showDebug) {
            debugVisualParent = new GameObject("Belt Debug Visual").transform;
            new DebugVisual();
        }
    }

    ~BeltManager() {
        TimeTickSystem.Instance.OnTick -= OnTick;
    }

    void OnTick() {
        int beltPathsCount = beltPathList.Count;

        for(int i = 0; i < beltPathsCount; i++) {
            beltPathList[i].RefreshMovedItems();
        }

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

    void MergeBeltPaths(ConveyorBelt endBeltFromMainPath, ConveyorBelt startBeltFromDeletedPath) {
        BeltPath mainBeltPath = beltEndsDict[endBeltFromMainPath];
        MergeBeltPaths(endBeltFromMainPath, startBeltFromDeletedPath, mainBeltPath);
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

        // Removing from the loop
        if(beltPath.beltList[^1].origin == beltPath.beltList[0].previousPosition) {

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

            beltEndsDict.Add(beltPath.beltList[0], beltPath);
            beltEndsDict.Add(beltPath.beltList[^1], beltPath);
            newStartBelt = beltPath.beltList[0];
            newEndBelt = beltPath.beltList[^1];
        }

        // Removing from the start of the path
        else if(beltIndex == 0) {
            beltPath.beltList.RemoveAt(0);
            beltEndsDict.Remove(belt);

            if(beltPath.beltList.Count > 0) {
                newStartBelt = beltPath.beltList[0];
                beltEndsDict[newStartBelt] = beltPath;
            } else {
                beltPathList.Remove(beltPath);
            }
        }

        // Removing from the end of the path
        else if(beltIndex == beltPath.beltList.Count - 1) {
            beltPath.beltList.RemoveAt(beltIndex);
            beltEndsDict.Remove(belt);

            if(beltPath.beltList.Count >= 2) {
                newEndBelt = beltPath.beltList[^1];
                beltEndsDict[newEndBelt] = beltPath;
            }
        }

        // Removing from the middle of the path
        else {
            List<ConveyorBelt> firstPart = beltPath.beltList.GetRange(0, beltIndex);
            List<ConveyorBelt> secondPart = beltPath.beltList.GetRange(beltIndex + 1, beltPath.beltList.Count - beltIndex - 1);
            beltPathList.Remove(beltPath);

            BeltPath newFirstPath = new();
            newFirstPath.beltList.AddRange(firstPart);
            beltPathList.Add(newFirstPath);
            beltEndsDict[firstPart[0]] = newFirstPath;

            if(firstPart.Count >= 2) {
                newEndBelt = firstPart[^1];
                beltEndsDict[newEndBelt] = newFirstPath;
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

        if(newStartBelt != null) {
            Vector2Int forwardDirection = newStartBelt.nextPosition - newStartBelt.origin;
            Vector2Int newPreviousPosition = newStartBelt.origin - forwardDirection;

            if(newPreviousPosition != newStartBelt.previousPosition) {
                ConveyorBelt connectedStraightBelt = TryGetConnectingBelt(newPreviousPosition);

                if(connectedStraightBelt != null && connectedStraightBelt.nextPosition == newStartBelt.origin) {
                    newStartBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowStraightVisual();
                    newStartBelt.previousPosition = newPreviousPosition;
                    MergeBeltPaths(connectedStraightBelt, newStartBelt);
                } else {
                    Vector2Int leftPosition = newStartBelt.origin + new Vector2Int(-forwardDirection.y, forwardDirection.x);
                    Vector2Int rightPosition = newStartBelt.origin + new Vector2Int(forwardDirection.y, -forwardDirection.x);
                    ConveyorBelt connectedLeftBelt = TryGetConnectingBelt(leftPosition);
                    ConveyorBelt connectedRightBelt = TryGetConnectingBelt(rightPosition);

                    if(connectedLeftBelt != null && connectedLeftBelt.nextPosition == newStartBelt.origin && (connectedRightBelt == null || connectedRightBelt.nextPosition != newStartBelt.origin)) {
                        newStartBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowLeftVisual();
                        newStartBelt.previousPosition = leftPosition;
                        MergeBeltPaths(connectedLeftBelt, newStartBelt);
                    } else if(connectedRightBelt != null && connectedRightBelt.nextPosition == newStartBelt.origin && (connectedLeftBelt == null || connectedLeftBelt.nextPosition != newStartBelt.origin)) {
                        newStartBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowRightVisual();
                        newStartBelt.previousPosition = rightPosition;
                        MergeBeltPaths(connectedRightBelt, newStartBelt);
                    } else {
                        newStartBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowStraightVisual();
                        newStartBelt.previousPosition = newPreviousPosition;
                    }
                }
            } else {
                Vector2Int leftPosition = newStartBelt.origin + new Vector2Int(-forwardDirection.y, forwardDirection.x);
                Vector2Int rightPosition = newStartBelt.origin + new Vector2Int(forwardDirection.y, -forwardDirection.x);
                ConveyorBelt connectedLeftBelt = TryGetConnectingBelt(leftPosition);
                ConveyorBelt connectedRightBelt = TryGetConnectingBelt(rightPosition);

                if(connectedLeftBelt != null && connectedLeftBelt.nextPosition == newStartBelt.origin && (connectedRightBelt == null || connectedRightBelt.nextPosition != newStartBelt.origin)) {
                    newStartBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowLeftVisual();
                    newStartBelt.previousPosition = leftPosition;
                    MergeBeltPaths(connectedLeftBelt, newStartBelt);
                } else if(connectedRightBelt != null && connectedRightBelt.nextPosition == newStartBelt.origin && (connectedLeftBelt == null || connectedLeftBelt.nextPosition != newStartBelt.origin)) {
                    newStartBelt.gameObject.GetComponent<ConveyorBeltVisualController>().ShowRightVisual();
                    newStartBelt.previousPosition = rightPosition;
                    MergeBeltPaths(connectedRightBelt, newStartBelt);
                }
            }
        }

        if(newEndBelt != null) {
            Vector2Int newNextPosition = newEndBelt.origin + (newEndBelt.origin - newEndBelt.previousPosition);

            if(newNextPosition == newEndBelt.nextPosition) {

            } else {

            }
        }

        OnBeltRemoved?.Invoke();
    }

    /*
    void CheckAndConnectAdjacentBelts(ConveyorBelt belt) {
        Vector2Int leftPosition = belt.origin + new Vector2Int(-1, 0);
        Vector2Int rightPosition = belt.origin + new Vector2Int(1, 0);

        ConveyorBelt leftBelt = TryGetConnectingBelt(leftPosition);
        ConveyorBelt rightBelt = TryGetConnectingBelt(rightPosition);

        if(leftBelt != null && leftBelt.nextPosition == belt.origin) {
            ConnectToPreviousBelt(belt, leftBelt, ref beltEndsDict[belt]);
        }

        if(rightBelt != null && belt.nextPosition == rightBelt.origin) {
            ConnectToNextBelt(belt, rightBelt, ref beltEndsDict[belt]);
        }
    }*/

    public class BeltPath {

        public List<ConveyorBelt> beltList { get; private set; } = new();

        public void RefreshMovedItems() {
            for(int i = beltList.Count - 1; i >= 0; i--) {
                beltList[i].ItemResetHasAlreadyMoved();
            }
        }

        public void TakeAction() {
            if(beltList[^1].origin == beltList[0].previousPosition) {
                ExecuteLoopActions();
            } else {
                ExecuteStandardActions();
            }
        }

        void ExecuteLoopActions() {
            List<ConveyorBelt> beltsToRepeat = new();
            bool shouldRepeat = true;

            for(int i = beltList.Count - 1; i >= 0; i--) {
                if(beltList[i].TakeAction() && shouldRepeat) {
                    beltsToRepeat.Add(beltList[i]);
                } else {
                    shouldRepeat = false;
                }
            }

            int repeatCount = beltsToRepeat.Count;

            for(int i = 0; i < repeatCount; i++) {
                beltsToRepeat[i].TakeAction();
            }
        }

        void ExecuteStandardActions() {
            for(int i = beltList.Count - 2; i >= 0; i--) {
                beltList[i].TakeAction();
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