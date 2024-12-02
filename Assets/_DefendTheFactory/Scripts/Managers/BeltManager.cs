using System;
using System.Collections.Generic;
using UnityEngine;

public class BeltManager {

    public static BeltManager Instance { get; private set; }

    public event Action OnBeltAdded;
    public event Action OnBeltRemoved;

    GridCell[,] gridArray = BuildingSystem.Instance.grid.gridArray;
    Dictionary<ConveyorBelt, BeltPath> beltEndsDict = new Dictionary<ConveyorBelt, BeltPath>();
    List<BeltPath> beltPathList = new List<BeltPath>();

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
        for(int i = 0; i < beltPathList.Count; i++) {
            beltPathList[i].ItemResetHasAlreadyMoved();
        }

        for(int i = 0; i < beltPathList.Count; i++) {
            beltPathList[i].TakeAction();
        }
    }

    public void AddBelt(ConveyorBelt belt) {
        BeltPath connectedBeltPath = null;
        ConveyorBelt connectingBelt = IsConnectingToBelt(belt.previousPosition);

        if(connectingBelt != null && connectingBelt.nextPosition == belt.origin) {
            connectedBeltPath = beltEndsDict[connectingBelt];
            connectedBeltPath.beltList.Add(belt);
            beltEndsDict.Add(belt, connectedBeltPath);
            if(connectedBeltPath.beltList.Count > 2) {
                beltEndsDict.Remove(connectingBelt);
            }
        }

        connectingBelt = IsConnectingToBelt(belt.nextPosition);

        if(connectingBelt != null && connectingBelt.previousPosition == belt.origin) {
            if(connectedBeltPath != null) {
                BeltPath path = beltEndsDict[connectingBelt];
                beltEndsDict.Remove(belt);
                if(connectedBeltPath == path) {
                    beltEndsDict.Remove(connectingBelt);
                } else {
                    connectedBeltPath.beltList.AddRange(path.beltList);
                    if(path.beltList.Count > 1) {
                        beltEndsDict.Remove(connectingBelt);
                    }
                    beltEndsDict[path.beltList[^1]] = connectedBeltPath;
                    beltPathList.Remove(path);
                }
            } else {
                connectedBeltPath = beltEndsDict[connectingBelt];
                connectedBeltPath.beltList.Insert(0, belt);
                beltEndsDict.Add(belt, connectedBeltPath);
                if(connectedBeltPath.beltList.Count > 2) {
                    beltEndsDict.Remove(connectingBelt);
                }
            }
        }

        if(connectedBeltPath == null) {
            connectedBeltPath = new BeltPath();
            connectedBeltPath.beltList.Add(belt);
            beltPathList.Add(connectedBeltPath);
            beltEndsDict.Add(belt, connectedBeltPath);
        }

        OnBeltAdded?.Invoke();
    }

    ConveyorBelt IsConnectingToBelt(Vector2Int connectingPosition) {
        if(connectingPosition.x >= 0 && connectingPosition.x < gridArray.GetLength(0) && connectingPosition.y >= 0 && connectingPosition.y < gridArray.GetLength(1)) {
            return gridArray[connectingPosition.x, connectingPosition.y].placedObject as ConveyorBelt;
        }
        return null;
    }

    public void RemoveBelt(ConveyorBelt belt) {
        //TODO
        OnBeltRemoved?.Invoke();
    }

    class BeltPath {

        public List<ConveyorBelt> beltList { get; private set; } = new List<ConveyorBelt>();

        public void TakeAction() {
            if(beltList[^1].origin == beltList[0].previousPosition) {
                bool loopRepeat = true;
                List<ConveyorBelt> beltsToRepeat = new List<ConveyorBelt>();

                for(int i = beltList.Count - 1; i >= 0; i--) {
                    if(beltList[i].TakeAction() && loopRepeat) {
                        beltsToRepeat.Add(beltList[i]);
                    } else {
                        loopRepeat = false;
                    }
                }

                int count = beltsToRepeat.Count;

                for(int i = 0; i < count; i++) {
                    beltsToRepeat[i].TakeAction();
                }

                return;
            }

            /*Vector2Int nextPosition = beltList[^1].nextPosition;
            PlacedObject nextObject = Instance.gridArray[nextPosition.x, nextPosition.y].placedObject;

            if(nextObject is ConveyorMerger) {
                beltList[^1].TakeAction();
            }*/

            for(int i = beltList.Count - 2; i >= 0; i--) {
                beltList[i].TakeAction();
            }
        }

        public void ItemResetHasAlreadyMoved() {
            for(int i = beltList.Count - 1; i >= 0; i--) {
                beltList[i].ItemResetHasAlreadyMoved();
            }
        }
    }

    /* --------------- BELT DEBUG VISUAL --------------- */

    class DebugVisual {

        List<BeltPathDebugVisual> beltPathDebugVisualList = new List<BeltPathDebugVisual>();

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

        Transform pathParent;

        public BeltPathDebugVisual(BeltPath beltPath, int pathNumber) {
            pathParent = new GameObject($"Path: {pathNumber}").transform;
            pathParent.parent = Instance.debugVisualParent;

            Vector2Int gridPosition = beltPath.beltList[0].GetGridPosition();
            Transform nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity, pathParent);

            if(beltPath.beltList.Count == 1) {
                nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.cyan;
                pathParent.position += new Vector3(0, .525f, 0);
                return;
            } else {
                nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.green;
            }

            gridPosition = beltPath.beltList[^1].GetGridPosition();
            nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity, pathParent);
            nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.red;

            for(int i = 0; i < beltPath.beltList.Count - 1; i++) {
                ConveyorBelt belt = beltPath.beltList[i];
                ConveyorBelt nextBelt = beltPath.beltList[i + 1];
                gridPosition = belt.GetGridPosition();
                Vector2Int nextGridPosition = nextBelt.GetGridPosition();

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
                gridPosition = beltPath.beltList[^1].GetGridPosition();
                nodeVisual = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualLine, BuildingSystem.Instance.GetWorldPosition(gridPosition) + new Vector3(.5f, 0, .5f), Quaternion.identity, pathParent);
                nodeVisual.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.yellow;
                Vector3 dirToNextBelt = (BuildingSystem.Instance.GetWorldPosition(beltPath.beltList[0].GetGridPosition()) - BuildingSystem.Instance.GetWorldPosition(gridPosition)).normalized;
                nodeVisual.eulerAngles = new Vector3(0, -CodeMonkey.Utils.UtilsClass.GetAngleFromVectorFloat3D(dirToNextBelt), 0);
            }

            pathParent.position += new Vector3(0, .525f, 0);
        }

        public void DestroySelf() {
            GameObject.Destroy(pathParent.gameObject);
        }
    }
}