using System;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : LogisticMachine<BaseBuildableObjectSO> {
    private ConveyorBelt inputBelt;
    private WorldItem newItem;
    private Dictionary<LogisticDir, ConveyorBelt> outputBelts = new();
    private Dictionary<LogisticDir, Vector2Int> outputPositions = new();

    protected override void Initialize(Vector2Int origin, BuildingDir dir, BaseBuildableObjectSO buildableDataSO) {
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

        SetupInputBelt(nextPosition);
        SetupOutputBelt(backPosition, LogisticDir.Straight);
        SetupOutputBelt(leftPosition, LogisticDir.Left);
        SetupOutputBelt(rightPosition, LogisticDir.Right);
    }

    private void SetupInputBelt(Vector2Int position) {
        if (!IsPositionValid(position)) return;

        Action action = () => HandleGridObjectChange(position);
        gridArray[position.x, position.y].ObjectChanged += action;
        objectChangedEvents.Add(action, position);

        if (ShouldSnap(position, out ConveyorBelt belt)) {
            inputBelt = belt;
        }
    }

    private void HandleGridObjectChange(Vector2Int position) {
        inputBelt = ShouldSnap(position, out ConveyorBelt belt) ? belt : null;
    }

    private void SetupOutputBelt(Vector2Int position, LogisticDir logisticDir) {
        outputBelts[logisticDir] = null;

        if (!IsPositionValid(position)) return;

        outputPositions[logisticDir] = position;
        Action action = () => HandleGridObjectChange(logisticDir);
        gridArray[position.x, position.y].ObjectChanged += action;
        objectChangedEvents.Add(action, position);

        if (ShouldSnapBack(position, out ConveyorBelt belt)) {
            outputBelts[logisticDir] = belt;
        }
    }

    public override bool IsOnOutputCell(Vector2Int position) {
        foreach (var kvp in outputPositions) {
            if (kvp.Value == position) {
                return true;
            }
        }

        return false;
    }

    private void HandleGridObjectChange(LogisticDir dir) {
        Vector2Int position = outputPositions[dir];

        if (ShouldSnapBack(position, out ConveyorBelt belt)) {
            outputBelts[dir] = belt;
        }
        else {
            outputBelts[dir] = null;
        }
    }

    public override void DestroySelf() {
        if (newItem != null) {
            newItem.DestroySelf();
        }

        base.DestroySelf();
    }

    protected override void OnEarlyTick() {
        if (newItem != null) {
            items.Add(newItem);
            newItem = null;
        }

        if (items.Count == maxStorage || inputBelt == null || inputBelt.endItem == null) return;

        newItem = inputBelt.endItem;
        inputBelt.ResetWorldItem();
        newItem.MoveToPosition(origin);
    }

    protected override void OnLateTick() {
        for (int i = 3; i > 0; i--) {
            if (items.Count == 0) return;

            if (outputBelts[logisticDir] == null || outputBelts[logisticDir].startItem != null) {
                logisticDir = GetNextDir(logisticDir);
                continue;
            }

            WorldItem worldItem = items[0];
            worldItem.MoveToPosition(outputBelts[logisticDir].origin);
            outputBelts[logisticDir].SetWorldItem(worldItem);
            items.RemoveAt(0);
            logisticDir = GetNextDir(logisticDir);
        }
    }
}