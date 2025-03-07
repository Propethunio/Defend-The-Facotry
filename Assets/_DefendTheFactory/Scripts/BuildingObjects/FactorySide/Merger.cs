using System;
using System.Collections.Generic;
using UnityEngine;

public class Merger : LogisticMachine<BaseBuildableObjectSO> {
    private ConveyorBelt outputBelt;
    private List<WorldItem> newItems = new();
    private Dictionary<LogisticDir, ConveyorBelt> inputBelts = new();
    private Dictionary<LogisticDir, Vector2Int> inputPositions = new();

    public override void Initialize(Vector2Int origin, BuildingDir dir, BaseBuildableObjectSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    public override void GridSetupDone() {
        base.GridSetupDone();
        Vector2Int forwardVector = BuildingSystem.Instance.GetDirForwardVector(dir);
        Vector2Int nextPosition = origin + forwardVector;
        Vector2Int backPosition = origin - forwardVector;
        Vector2Int rightVector = new Vector2Int(forwardVector.y, -forwardVector.x);
        Vector2Int rightPosition = origin + rightVector;
        Vector2Int leftPosition = origin - rightVector;

        SetupInputBelt(backPosition, LogisticDir.Straight);
        SetupInputBelt(leftPosition, LogisticDir.Left);
        SetupInputBelt(rightPosition, LogisticDir.Right);
        SetupOutputBelt(nextPosition);
    }

    private void SetupInputBelt(Vector2Int position, LogisticDir logisticDir) {
        inputBelts[logisticDir] = null;

        if(!IsPositionValid(position)) return;

        inputPositions[logisticDir] = position;
        Action action = () => HandleGridObjectChange(logisticDir);
        gridArray[position.x, position.y].ObjectChanged += action;
        objectChangedEvents.Add(action, position);

        if(ShouldSnap(position, out ConveyorBelt belt)) {
            inputBelts[logisticDir] = belt;
        }
    }

    private void HandleGridObjectChange(LogisticDir dir) {
        Vector2Int position = inputPositions[dir];

        if(ShouldSnap(position, out ConveyorBelt belt)) {
            inputBelts[dir] = belt;
        } else {
            inputBelts[dir] = null;
        }
    }

    private void SetupOutputBelt(Vector2Int position) {
        if(!IsPositionValid(position)) return;

        Action action = () => HandleGridObjectChange(position);
        gridArray[position.x, position.y].ObjectChanged += action;
        objectChangedEvents.Add(action, position);

        if(ShouldSnapBack(position, out ConveyorBelt belt)) {
            outputBelt = belt;
        }
    }

    private void HandleGridObjectChange(Vector2Int position) {
        if(ShouldSnapBack(position, out ConveyorBelt belt)) {
            outputBelt = belt;
        } else {
            outputBelt = null;
        }
    }

    public override void DestroySelf() {
        foreach(WorldItem item in newItems) {
            item.DestroySelf();
        }
        base.DestroySelf();
    }

    protected override void OnEarlyTick() {
        if(newItems.Count > 0) {
            items.AddRange(newItems);
            newItems.Clear();
        }

        if(items.Count == maxStorage) return;

        for(int i = 3; i > 0; i--) {

            if(inputBelts[logisticDir] == null || inputBelts[logisticDir].endItem == null) {
                logisticDir = GetNextDir(logisticDir);
                continue;
            }

            inputBelts[logisticDir].endItem.MoveToPosition(origin);
            newItems.Add(inputBelts[logisticDir].endItem);
            inputBelts[logisticDir].ResetWorldItem();
            logisticDir = GetNextDir(logisticDir);

            if(items.Count + newItems.Count == maxStorage) return;
        }
    }

    protected override void OnLateTick() {
        if(items.Count == 0 || outputBelt == null || outputBelt.startItem != null) return;

        WorldItem worldItem = items[0];
        worldItem.MoveToPosition(outputBelt.origin);
        outputBelt.SetWorldItem(worldItem);
        items.RemoveAt(0);
    }
}