using System;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : LogisticMachine<BaseBuildableObjectSO> {
    private IItemProvider inputMachine;
    private WorldItem newItem;
    private WorldItem itemReadyToGo;
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

        if (ShouldSnap(position, out IItemProvider itemProvider)) {
            inputMachine = itemProvider;
        }
    }

    private void HandleGridObjectChange(Vector2Int position) {
        inputMachine = ShouldSnap(position, out IItemProvider itemProvider) ? itemProvider : null;
    }

    private void SetupOutputBelt(Vector2Int position, LogisticDir logisticDir) {
        outputBelts[logisticDir] = null;

        if (!IsPositionValid(position)) return;

        outputPositions[logisticDir] = position;
        Action action = () => HandleGridObjectChange(logisticDir);
        gridArray[position.x, position.y].ObjectChanged += action;
        objectChangedEvents.Add(action, position);

        if (!ShouldSnapBack(position, out ConveyorBelt belt)) return;

        outputBelts[logisticDir] = belt;
        belt.SetLogisticMachineAsParent(this);
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
        if (itemReadyToGo != null) {
            itemReadyToGo.DestroySelf();
        }

        if (newItem != null) {
            newItem.DestroySelf();
        }

        base.DestroySelf();
    }

    protected override void OnEarlyTick() {
        if (itemReadyToGo != null) {
            items.Add(itemReadyToGo);
            itemReadyToGo = null;
        }

        if (newItem != null) {
            itemReadyToGo = newItem;
            newItem = null;
        }

        if (items.Count == maxStorage || inputMachine == null || !inputMachine.HasItem()) return;

        newItem = inputMachine.GetWorldItem();
        newItem.MoveToPosition(CalculatePositionInsideMachine(inputMachine.GetDir()));
    }

    protected override void OnLateTick() {
        for (int i = 3; i > 0; i--) {
            if (items.Count == 0) return;

            if (outputBelts[logisticDir] == null || outputBelts[logisticDir].startItem != null) {
                logisticDir = GetNextDir(logisticDir);
                continue;
            }

            WorldItem worldItem = items[0];
            worldItem.MoveToPosition(CalculatePositionOnBelt(worldItem, outputBelts[logisticDir]));
            outputBelts[logisticDir].SetWorldItem(worldItem);
            items.RemoveAt(0);
            logisticDir = GetNextDir(logisticDir);
        }
    }

    public override bool ShouldSnapWithLogisticMachine(Vector2Int logisticMachineOrigin) {
        Vector2Int forwardVector = BuildingSystem.Instance.GetDirForwardVector(dir);

        if (origin - forwardVector == logisticMachineOrigin) return true;

        Vector2Int rightVector = new Vector2Int(forwardVector.y, -forwardVector.x);

        if (origin - rightVector == logisticMachineOrigin) return true;

        return origin + rightVector == logisticMachineOrigin;
    }
}