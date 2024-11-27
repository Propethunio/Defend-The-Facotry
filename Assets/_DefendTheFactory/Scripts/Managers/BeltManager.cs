using System;
using System.Collections.Generic;
using UnityEngine;

public class BeltManager {

    public static BeltManager Instance { get; private set; }

    public event Action OnBeltAdded;
    public event Action OnBeltRemoved;

    List<ConveyorBelt> fullBeltList;
    List<BeltPath> beltPathList;

    public Transform debugVisualParent { get; private set; }

    public BeltManager(bool showDebug) {
        if(Instance == null) Instance = this;
        else return;

        fullBeltList = new List<ConveyorBelt>();
        beltPathList = new List<BeltPath>();
        TimeTickSystem.Instance.OnTick += TimeTickSystem_OnTick;
        if(showDebug) {
            debugVisualParent = new GameObject("Belt Debug Visual").transform;
            new DebugVisual();
        }
    }

    ~BeltManager() {
        TimeTickSystem.Instance.OnTick -= TimeTickSystem_OnTick;
    }

    void TimeTickSystem_OnTick() {
        for(int i = 0; i < beltPathList.Count; i++) {
            beltPathList[i].ItemResetHasAlreadyMoved();
        }

        for(int i = 0; i < beltPathList.Count; i++) {
            beltPathList[i].TakeAction();
        }
    }

    public void AddBelt(ConveyorBelt belt) {
        fullBeltList.Add(belt);
        RefreshBeltPathList();
        OnBeltAdded?.Invoke();
    }

    public void RemoveBelt(ConveyorBelt belt) {
        fullBeltList.Remove(belt);
        RefreshBeltPathList();
        OnBeltRemoved?.Invoke();
    }

    void RefreshBeltPathList() {
        beltPathList.Clear();
        List<ConveyorBelt> beltList = new List<ConveyorBelt>(fullBeltList);

        while(beltList.Count > 0) {
            ConveyorBelt belt = beltList[0];
            beltList.RemoveAt(0);
            bool foundMatchingBeltPath = false;

            foreach(BeltPath beltPath in beltPathList) {
                if(beltPath.IsGridPositionPartOfBeltPath(belt.GetNextGridPosition())) {
                    // This Belt can connect to this Belt Path
                    // Will it cause a loop?
                    if(beltPath.IsGridPositionPartOfBeltPath(belt.GetPreviousGridPosition())) {
                        // Previous Belt Position is ALSO part of this path, meaning that adding this one will create a loop
                    } else {
                        // Previous Belt Position is NOT part of this path, safe to add without causing a loop
                        beltPath.AddBelt(belt);
                        foundMatchingBeltPath = true;
                        break;
                    }
                }
            }

            if(!foundMatchingBeltPath) {
                // Couldn't find a Belt Path for this Belt, create new Belt Path
                BeltPath beltPath = new BeltPath();
                beltPath.AddBelt(belt);
                beltPathList.Add(beltPath);
            }
        }

        // Second Iteration: Merge Belt Paths
        {
            int safety = 0;
            while(TryMergeAnyBeltPath()) {
                // Continue Merging Belt Paths
                safety++;
                if(safety > 1000) break;
            }

            if(safety > 1000) {
                Debug.LogError("######## SAFETY BREAK!");
            }
        }
    }

    bool TryMergeAnyBeltPath() {
        // Tries to merge any belt path, returns true if successful
        for(int i = 0; i < beltPathList.Count; i++) {
            BeltPath beltPathA = beltPathList[i];

            for(int j = 0; j < beltPathList.Count; j++) {
                if(j == i) continue; // Don't try to merge with itself
                BeltPath beltPathB = beltPathList[j];
                ConveyorBelt beltFirstA = beltPathA.GetFirstBelt();
                ConveyorBelt beltLastA = beltPathA.GetLastBelt();
                ConveyorBelt beltFirstB = beltPathB.GetFirstBelt();
                ConveyorBelt beltLastB = beltPathB.GetLastBelt();

                if(beltLastA.GetNextGridPosition() == beltFirstB.GetGridPosition()) {
                    // Next Position on the LastA is the FirstB, connect A to B
                    // Will it cause a loop?
                    if(beltLastB.GetNextGridPosition() == beltFirstA.GetGridPosition()) {
                        // Last on B connects to First on A, this creates a loop, don't do it
                    } else {
                        // Last on B does NOT connect to First on A, safe to connect without making a loop
                        beltPathA.MergeBeltPath(beltPathB);
                        beltPathList.Remove(beltPathB);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public override string ToString() {
        string debugString = "BeltSystem: " + "\n";
        foreach(BeltPath beltPath in beltPathList) {
            debugString += beltPath.ToString() + "\n";
        }
        return debugString;
    }

    /* --------------- BELT PATH --------------- */

    class BeltPath {

        List<ConveyorBelt> beltList;

        public BeltPath() {
            beltList = new List<ConveyorBelt>();
        }

        void RefreshBeltOrder() {
            List<ConveyorBelt> newBeltList = new List<ConveyorBelt>();

            ConveyorBelt firstBelt = GetFirstBelt();
            newBeltList.Add(firstBelt);

            //Debug.Log("firstBelt: " + firstBelt.GetGridPosition());
            ConveyorBelt belt = firstBelt;

            int safety = 0;
            do {
                PlacedObject placedObject = BuildingSystem.Instance.GetGridObject(belt.GetNextGridPosition()).placedObject;
                if(placedObject != null && placedObject is ConveyorBelt) {
                    // Has a Belt in the next position
                    //Debug.Log("Has a Belt in the next position " + belt.GetNextGridPosition());
                    ConveyorBelt nextBelt = placedObject as ConveyorBelt;
                    // Is it part of this path?
                    if(beltList.Contains(nextBelt)) {
                        // Yes it's part of this path
                        //Debug.Log("Yes it's part of this path");
                        newBeltList.Add(nextBelt);
                        belt = nextBelt;
                    } else {
                        // Next is a Belt but not part of this Path
                        //Debug.Log("Next is a Belt but not part of this Path");
                        belt = null;
                    }
                } else {
                    // No object or not a Belt
                    //Debug.Log("No object or not a Belt " + belt.GetNextGridPosition() + " " + placedObject);
                    belt = null;
                }
                safety++;
                if(safety > 1000) break;
            } while(belt != null);

            if(safety > 1000) {
                Debug.LogError("######## SAFETY BREAK!");
            }

            if(beltList.Count != newBeltList.Count) {
                Debug.LogError("beltList.Count != newBeltList.Count \t " + beltList.Count + " != " + newBeltList.Count);
                string errorString = "beltList: ";
                foreach(ConveyorBelt b in beltList) errorString += b.GetGridPosition() + "; ";
                errorString += "\nnewBeltList: ";
                foreach(ConveyorBelt b in newBeltList) errorString += b.GetGridPosition() + "; ";
                Debug.LogError(errorString);
            }

            beltList = newBeltList;
        }

        public void AddBelt(ConveyorBelt belt) {
            beltList.Add(belt);
            //Debug.Log("AddBelt: " + belt.GetGridPosition());
            RefreshBeltOrder();
        }

        public bool IsGridPositionPartOfBeltPath(Vector2Int gridPosition) {
            foreach(ConveyorBelt belt in beltList) {
                if(belt.GetGridPosition() == gridPosition) {
                    return true;
                }
            }
            return false;
        }

        public ConveyorBelt GetFirstBelt() {
            List<ConveyorBelt> tmpBeltList = new List<ConveyorBelt>(beltList);

            for(int i = 0; i < beltList.Count; i++) {
                ConveyorBelt belt = beltList[i];

                PlacedObject placedObject = BuildingSystem.Instance.GetGridObject(belt.GetNextGridPosition()).placedObject;
                if(placedObject != null && placedObject is ConveyorBelt) {
                    // Has a Belt in the next position
                    ConveyorBelt nextBelt = placedObject as ConveyorBelt;
                    // Is it part of this path?
                    if(beltList.Contains(nextBelt)) {
                        // Yes it's part of this path
                        tmpBeltList.Remove(nextBelt);
                    }
                }
            }

            if(tmpBeltList.Count <= 0) {
                Debug.LogError("Something went wrong, there's no more Belts left!");
                return beltList[0];
            }

            return tmpBeltList[0];
        }

        public ConveyorBelt GetLastBelt() {
            List<ConveyorBelt> tmpBeltList = new List<ConveyorBelt>(beltList);

            ConveyorBelt lastBelt = tmpBeltList[0];
            tmpBeltList.RemoveAt(0);

            while(tmpBeltList.Count > 0) {
                PlacedObject placedObject = BuildingSystem.Instance.GetGridObject(lastBelt.GetNextGridPosition()).placedObject;
                if(placedObject != null && placedObject is ConveyorBelt) {
                    // Has a Belt in the next position
                    ConveyorBelt nextBelt = placedObject as ConveyorBelt;
                    // Is it part of this path?
                    if(tmpBeltList.Contains(nextBelt)) {
                        // It is part of this path, continue
                        tmpBeltList.Remove(nextBelt);
                        lastBelt = nextBelt;
                    } else {
                        // Not part of this path, this is the last one
                        break;
                    }
                } else {
                    // No Belt in the next position, this is the last one
                    break;
                }
            }

            return lastBelt;
        }

        public List<ConveyorBelt> GetBeltList() {
            return beltList;
        }

        public void MergeBeltPath(BeltPath beltPathB) {
            foreach(ConveyorBelt belt in beltPathB.beltList) {
                AddBelt(belt);
            }
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

        public override string ToString() {
            string debugString = "BeltPath: ";
            foreach(ConveyorBelt belt in beltList) {
                debugString += belt.GetGridPosition() + "->";
            }
            return debugString;
        }

    }

    /* --------------- BELT DEBUG VISUAL --------------- */

    class DebugVisual {

        List<BeltPathDebugVisual> beltPathDebugVisualList;

        public DebugVisual() {
            Instance.OnBeltAdded += Instance_OnBeltAdded;
            Instance.OnBeltRemoved += Instance_OnBeltRemoved;
            beltPathDebugVisualList = new List<BeltPathDebugVisual>();
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

            Vector2Int gridPosition = beltPath.GetFirstBelt().GetGridPosition();
            Transform beltDebugVisualNodeTransform = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity, pathParent);
            beltDebugVisualNodeTransform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.green;

            if(beltPath.GetBeltList().Count == 1) {
                beltDebugVisualNodeTransform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.cyan;
                pathParent.position += new Vector3(0, .525f, 0);
                return;
            }

            gridPosition = beltPath.GetLastBelt().GetGridPosition();
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