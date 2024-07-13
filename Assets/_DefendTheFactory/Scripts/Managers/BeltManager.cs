//#define SHOW_DEBUG_VISUAL

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

/*
 * Manages the entire Belt System containing all Belt Paths
 * */
public class BeltManager {

    public static BeltManager Instance { get; private set; }

    public static void Create() {
        new BeltManager();
    }



    public event EventHandler OnBeltAdded;
    public event EventHandler OnBeltRemoved;


    private List<ConveyorBelt> fullBeltList; // Contains full list of all belts in the world

    private List<BeltPath> beltPathList;

    private BeltManager() {
        Instance = this;

        fullBeltList = new List<ConveyorBelt>();

        beltPathList = new List<BeltPath>();

        TimeTickSystem.OnTick += TimeTickSystem_OnTick;

#if SHOW_DEBUG_VISUAL
        new DebugVisual();
#endif
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e) {
        for (int i = 0; i < beltPathList.Count; i++) {
            beltPathList[i].ItemResetHasAlreadyMoved();
        }

        for (int i = 0; i < beltPathList.Count; i++) {
            beltPathList[i].TakeAction();
        }
    }

    public void AddBelt(ConveyorBelt belt) {
        //Debug.Log("## AddBelt: " + belt.GetGridPosition());
        fullBeltList.Add(belt);
        RefreshBeltPathList();

        //Debug.Log(this);
        OnBeltAdded?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveBelt(ConveyorBelt belt) {
        fullBeltList.Remove(belt);
        RefreshBeltPathList();

        //Debug.Log(this);
        OnBeltRemoved?.Invoke(this, EventArgs.Empty);
    }

    private void RefreshBeltPathList() {
        beltPathList.Clear();

        List<ConveyorBelt> beltList = new List<ConveyorBelt>(fullBeltList);

        // First Iteration: Create Belt Paths
        while (beltList.Count > 0) {
            ConveyorBelt belt = beltList[0];
            beltList.RemoveAt(0);

            bool foundMatchingBeltPath = false;
            foreach (BeltPath beltPath in beltPathList) {
                if (beltPath.IsGridPositionPartOfBeltPath(belt.GetNextGridPosition())) {
                    // This Belt can connect to this Belt Path
                    // Will it cause a loop?
                    if (beltPath.IsGridPositionPartOfBeltPath(belt.GetPreviousGridPosition())) {
                        // Previous Belt Position is ALSO part of this path, meaning that adding this one will create a loop
                    } else {
                        // Previous Belt Position is NOT part of this path, safe to add without causing a loop
                        beltPath.AddBelt(belt);
                        foundMatchingBeltPath = true;
                        break;
                    }
                }
            }

            if (!foundMatchingBeltPath) {
                // Couldn't find a Belt Path for this Belt, create new Belt Path
                BeltPath beltPath = new BeltPath();
                beltPath.AddBelt(belt);
                beltPathList.Add(beltPath);
            }
        }

        // Second Iteration: Merge Belt Paths
        {
            int safety = 0;
            while (TryMergeAnyBeltPath()) {
                // Continue Merging Belt Paths
                safety++;
                if (safety > 1000) break;
            }

            if (safety > 1000) {
                Debug.LogError("######## SAFETY BREAK!");
            }
        }
    }

    private bool TryMergeAnyBeltPath() {
        //Debug.Log("#### SKIP MERGE BELT PATHS"); return false;

        // Tries to merge any belt path, returns true if successful
        for (int i = 0; i < beltPathList.Count; i++) {
            BeltPath beltPathA = beltPathList[i];

            for (int j = 0; j < beltPathList.Count; j++) {
                if (j == i) continue; // Don't try to merge with itself
                BeltPath beltPathB = beltPathList[j];

                ConveyorBelt beltFirstA = beltPathA.GetFirstBelt();
                ConveyorBelt beltLastA = beltPathA.GetLastBelt();
                ConveyorBelt beltFirstB = beltPathB.GetFirstBelt();
                ConveyorBelt beltLastB = beltPathB.GetLastBelt();

                if (beltLastA.GetNextGridPosition() == beltFirstB.GetGridPosition()) {
                    // Next Position on the LastA is the FirstB, connect A to B
                    // Will it cause a loop?
                    if (beltLastB.GetNextGridPosition() == beltFirstA.GetGridPosition()) {
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
        foreach (BeltPath beltPath in beltPathList) {
            debugString += beltPath.ToString() + "\n";
        }
        return debugString;
    }





    /*
     * Generates a Debug Visual for the Belt System
     * */
    private class DebugVisual {

        private List<BeltPathDebugVisual> beltPathDebugVisualList;

        public DebugVisual() {
            BeltManager.Instance.OnBeltAdded += Instance_OnBeltAdded;
            BeltManager.Instance.OnBeltRemoved += Instance_OnBeltRemoved;

            beltPathDebugVisualList = new List<BeltPathDebugVisual>();
            RefreshVisual();
        }

        private void Instance_OnBeltAdded(object sender, EventArgs e) {
            RefreshVisual();
        }

        private void Instance_OnBeltRemoved(object sender, EventArgs e) {
            RefreshVisual();
        }

        private void RefreshVisual() {
            foreach (BeltPathDebugVisual beltPathDebugVisual in beltPathDebugVisualList) {
                beltPathDebugVisual.DestroySelf();
            }

            beltPathDebugVisualList.Clear();

            foreach (BeltPath beltPath in BeltManager.Instance.beltPathList) {
                beltPathDebugVisualList.Add(new BeltPathDebugVisual(beltPath));
            }
        }


        private class BeltPathDebugVisual {

            private List<Transform> transformList;

            public BeltPathDebugVisual(BeltPath beltPath) {
                transformList = new List<Transform>();

                Vector2Int gridPosition = beltPath.GetFirstBelt().GetGridPosition();
                Transform beltDebugVisualNodeTransform = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, BuildingSystem.Instance.GetWorldPosition(gridPosition) + CodeMonkey.Utils.UtilsClass.GetRandomDirXZ() * .1f, Quaternion.identity);
                transformList.Add(beltDebugVisualNodeTransform);

                beltDebugVisualNodeTransform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.green;

                if (beltPath.GetBeltList().Count == 1) {
                    // Only has a single belt
                    // Show in purple and break
                    beltDebugVisualNodeTransform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.magenta;
                    return;
                }

                gridPosition = beltPath.GetLastBelt().GetGridPosition();
                beltDebugVisualNodeTransform = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, BuildingSystem.Instance.GetWorldPosition(gridPosition) + CodeMonkey.Utils.UtilsClass.GetRandomDirXZ() * .1f, Quaternion.identity);
                transformList.Add(beltDebugVisualNodeTransform);
                beltDebugVisualNodeTransform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.red;

                for (int i=0; i<beltPath.GetBeltList().Count - 1; i++) {
                    ConveyorBelt belt = beltPath.GetBeltList()[i];
                    ConveyorBelt nextBelt = beltPath.GetBeltList()[i + 1];
                    gridPosition = belt.GetGridPosition();
                    Vector2Int nextGridPosition = nextBelt.GetGridPosition();

                    beltDebugVisualNodeTransform = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualNode, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                    transformList.Add(beltDebugVisualNodeTransform);
                    beltDebugVisualNodeTransform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.white;

                    beltDebugVisualNodeTransform = GameObject.Instantiate(GameAssets.i.pfBeltDebugVisualLine, BuildingSystem.Instance.GetWorldPosition(gridPosition) + new Vector3(1, 0, 1) * .5f, Quaternion.identity);
                    transformList.Add(beltDebugVisualNodeTransform);
                    beltDebugVisualNodeTransform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.white;

                    Vector3 dirToNextBelt = (BuildingSystem.Instance.GetWorldPosition(nextGridPosition) - BuildingSystem.Instance.GetWorldPosition(gridPosition)).normalized;
                    beltDebugVisualNodeTransform.eulerAngles = new Vector3(0, -CodeMonkey.Utils.UtilsClass.GetAngleFromVectorFloat3D(dirToNextBelt), 0);
                }
            }

            public void DestroySelf() {
                foreach (Transform transform in transformList) {
                    GameObject.Destroy(transform.gameObject);
                }
            }

        }

    }










    /*
     * Manages a single Belt Path, start to finish with all Conveyour Belts
     * */
    private class BeltPath {

        private List<ConveyorBelt> beltList;

        public BeltPath() {
            beltList = new List<ConveyorBelt>();
        }

        private void RefreshBeltOrder() {
            List <ConveyorBelt> newBeltList = new List<ConveyorBelt>();

            ConveyorBelt firstBelt = GetFirstBelt();
            newBeltList.Add(firstBelt);

            //Debug.Log("firstBelt: " + firstBelt.GetGridPosition());
            ConveyorBelt belt = firstBelt;

            int safety = 0;
            do {
                PlacedObject placedObject = BuildingSystem.Instance.GetGridObject(belt.GetNextGridPosition()).placedObject;
                if (placedObject != null && placedObject is ConveyorBelt) {
                    // Has a Belt in the next position
                    //Debug.Log("Has a Belt in the next position " + belt.GetNextGridPosition());
                    ConveyorBelt nextBelt = placedObject as ConveyorBelt;
                    // Is it part of this path?
                    if (beltList.Contains(nextBelt)) {
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
                if (safety > 1000) break;
            } while (belt != null);

            if (safety > 1000) {
                Debug.LogError("######## SAFETY BREAK!");
            }

            if (beltList.Count != newBeltList.Count) {
                Debug.LogError("beltList.Count != newBeltList.Count \t " + beltList.Count + " != " + newBeltList.Count);
                string errorString = "beltList: ";
                foreach (ConveyorBelt b in beltList) errorString += b.GetGridPosition() + "; ";
                errorString += "\nnewBeltList: ";
                foreach (ConveyorBelt b in newBeltList) errorString += b.GetGridPosition() + "; ";
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
            foreach (ConveyorBelt belt in beltList) {
                if (belt.GetGridPosition() == gridPosition) {
                    return true;
                }
            }
            return false;
        }

        public ConveyorBelt GetFirstBelt() {
            List<ConveyorBelt> tmpBeltList = new List<ConveyorBelt>(beltList);

            for (int i = 0; i < beltList.Count; i++) {
                ConveyorBelt belt = beltList[i];

                PlacedObject placedObject = BuildingSystem.Instance.GetGridObject(belt.GetNextGridPosition()).placedObject;
                if (placedObject != null && placedObject is ConveyorBelt) {
                    // Has a Belt in the next position
                    ConveyorBelt nextBelt = placedObject as ConveyorBelt;
                    // Is it part of this path?
                    if (beltList.Contains(nextBelt)) {
                        // Yes it's part of this path
                        tmpBeltList.Remove(nextBelt);
                    }
                }
            }

            if (tmpBeltList.Count <= 0) {
                Debug.LogError("Something went wrong, there's no more Belts left!");
                return beltList[0];
            }

            return tmpBeltList[0];
        }

        public ConveyorBelt GetLastBelt() {
            List<ConveyorBelt> tmpBeltList = new List<ConveyorBelt>(beltList);

            ConveyorBelt lastBelt = tmpBeltList[0];
            tmpBeltList.RemoveAt(0);

            while (tmpBeltList.Count > 0) {
                PlacedObject placedObject = BuildingSystem.Instance.GetGridObject(lastBelt.GetNextGridPosition()).placedObject;
                if (placedObject != null && placedObject is ConveyorBelt) {
                    // Has a Belt in the next position
                    ConveyorBelt nextBelt = placedObject as ConveyorBelt;
                    // Is it part of this path?
                    if (tmpBeltList.Contains(nextBelt)) {
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
            foreach (ConveyorBelt belt in beltPathB.beltList) {
                AddBelt(belt);
            }
        }

        public void TakeAction() {
            for (int i = beltList.Count - 1; i >= 0; i--) {
                ConveyorBelt belt = beltList[i];
                belt.TakeAction();
            }
        }

        public void ItemResetHasAlreadyMoved() {
            for (int i = beltList.Count - 1; i >= 0; i--) {
                ConveyorBelt belt = beltList[i];
                belt.ItemResetHasAlreadyMoved();
            }
        }

        public override string ToString() {
            string debugString = "BeltPath: ";
            foreach (ConveyorBelt belt in beltList) {
                debugString += belt.GetGridPosition() + "->";
            }
            return debugString;
        }

    }

}
