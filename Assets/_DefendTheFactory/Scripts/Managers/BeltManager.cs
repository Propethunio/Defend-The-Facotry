using System;
using System.Collections.Generic;
using UnityEngine;

public class BeltManager {

    public static BeltManager Instance { get; private set; }

    public event Action OnBeltAdded;
    public event Action OnBeltRemoved;

    Dictionary<ConveyorBelt, BeltPath> beltEndsDict = new Dictionary<ConveyorBelt, BeltPath>();
    List<BeltPath> beltPathList;
    List<ConveyorBelt> fullBeltList;

    public Transform debugVisualParent { get; private set; }

    public BeltManager(bool showDebug) {
        if(Instance == null) Instance = this;
        else return;

        fullBeltList = new List<ConveyorBelt>();
        beltPathList = new List<BeltPath>();
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
        GridCell[,] gridArray = BuildingSystem.Instance.grid.gridArray;
        ConveyorBelt connectingBelt = IsConnectingToBelt(gridArray, belt.previousPosition);

        if(connectingBelt != null && connectingBelt.nextPosition == belt.origin) {
            connectedBeltPath = beltEndsDict[connectingBelt];
            connectedBeltPath.beltList.Add(belt);
            beltEndsDict.Add(belt, connectedBeltPath);
            if(connectedBeltPath.beltList.Count > 2) {
                beltEndsDict.Remove(connectingBelt);
            }
        }

        connectingBelt = IsConnectingToBelt(gridArray, belt.nextPosition);

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
                    beltEndsDict[path.beltList[path.beltList.Count - 1]] = connectedBeltPath;
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

    ConveyorBelt IsConnectingToBelt(GridCell[,] gridArray, Vector2Int connectingPosition) {
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

        public List<ConveyorBelt> GetBeltList() {
            return beltList;
        }

        public void TakeAction() {
            for(int i = beltList.Count - 1; i >= 0; i--) {
                ConveyorBelt belt = beltList[i];
                belt.TakeAction();
            }
        }

        public void ItemResetHasAlreadyMoved() {
            for(int i = beltList.Count - 1; i >= 0; i--) {
                ConveyorBelt belt = beltList[i];
                belt.ItemResetHasAlreadyMoved();
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
            Transform beltDebugVisualNodeTransform = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity, pathParent);
            beltDebugVisualNodeTransform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.green;

            if(beltPath.GetBeltList().Count == 1) {
                beltDebugVisualNodeTransform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.cyan;
                pathParent.position += new Vector3(0, .525f, 0);
                return;
            }

            gridPosition = beltPath.beltList[beltPath.beltList.Count - 1].GetGridPosition();
            beltDebugVisualNodeTransform = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity, pathParent);
            beltDebugVisualNodeTransform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.red;

            for(int i = 0; i < beltPath.GetBeltList().Count - 1; i++) {
                ConveyorBelt belt = beltPath.GetBeltList()[i];
                ConveyorBelt nextBelt = beltPath.GetBeltList()[i + 1];
                gridPosition = belt.GetGridPosition();
                Vector2Int nextGridPosition = nextBelt.GetGridPosition();

                if(i > 0) {
                    beltDebugVisualNodeTransform = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity, pathParent);
                    beltDebugVisualNodeTransform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.blue;
                }

                beltDebugVisualNodeTransform = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualLine, BuildingSystem.Instance.GetWorldPosition(gridPosition) + new Vector3(1, 0, 1) * .5f, Quaternion.identity, pathParent);
                beltDebugVisualNodeTransform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.yellow;

                Vector3 dirToNextBelt = (BuildingSystem.Instance.GetWorldPosition(nextGridPosition) - BuildingSystem.Instance.GetWorldPosition(gridPosition)).normalized;
                beltDebugVisualNodeTransform.eulerAngles = new Vector3(0, -CodeMonkey.Utils.UtilsClass.GetAngleFromVectorFloat3D(dirToNextBelt), 0);
            }

            pathParent.position += new Vector3(0, .525f, 0);
        }

        public void DestroySelf() {
            GameObject.Destroy(pathParent.gameObject);
        }
    }
}