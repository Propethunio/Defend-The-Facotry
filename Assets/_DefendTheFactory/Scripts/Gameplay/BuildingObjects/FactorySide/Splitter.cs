using System;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : LogisticMachine<BaseBuildableObjectSO> {
    private IItemProvider inputMachine;
    private WorldItem newItem;
    private WorldItem itemReadyToGo;
    private Dictionary<LogisticDir, ConveyorBelt> outputBelts = new();
    private Dictionary<LogisticDir, Vector2Int> outputPositions = new();
    private Dictionary<LogisticDir, Splitter> outputSplitters = new();
    private List<Splitter> splittersWaitingForResource = new();

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
        SetupOutputMachines(backPosition, LogisticDir.Straight);
        SetupOutputMachines(leftPosition, LogisticDir.Left);
        SetupOutputMachines(rightPosition, LogisticDir.Right);
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

    private void SetupOutputMachines(Vector2Int position, LogisticDir logisticDir) {
        outputBelts[logisticDir] = null;
        outputSplitters[logisticDir] = null;

        if (!IsPositionValid(position)) return;

        outputPositions[logisticDir] = position;
        Action action = () => HandleGridObjectChange(logisticDir);
        gridArray[position.x, position.y].ObjectChanged += action;
        objectChangedEvents.Add(action, position);

        if (!ShouldSnapBack(position, out ConveyorBelt belt)) {
            Splitter splitter = gridArray[position.x, position.y].placedObject as Splitter;

            if (splitter == null) return;

            outputSplitters[logisticDir] = splitter;
            return;
        }

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
            outputSplitters[dir] = null;
        }
        else {
            outputBelts[dir] = null;
            Splitter splitter = gridArray[position.x, position.y].placedObject as Splitter;
            outputSplitters[dir] = splitter == null ? null : splitter;
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
        if (itemReadyToGo != null && items.Count < 2) {
            items.Add(itemReadyToGo);
            itemReadyToGo = null;
        }

        if (newItem != null && itemReadyToGo == null) {
            itemReadyToGo = newItem;
            newItem = null;
        }

        if (newItem != null || inputMachine == null) return;

        if (inputMachine is Splitter splitter) {
            splitter.NotifySplitter(this);
            return;
        }

        if (!inputMachine.HasItem()) return;

        newItem = inputMachine.GetWorldItem();
        newItem.MoveToPosition(CalculatePositionInsideMachine(inputMachine.GetDir()));
    }

    protected override void OnLateTick() {
        for (int i = 3; i > 0; i--) {
            if (items.Count == 0) break;

            if (outputBelts[logisticDir] == null) {
                if (outputSplitters[logisticDir] != null && splittersWaitingForResource.Contains(outputSplitters[logisticDir])) {
                    outputSplitters[logisticDir].MoveItemAfterNotify(items[0]);
                    items.RemoveAt(0);
                }

                logisticDir = GetNextDir(logisticDir);
                continue;
            }

            if (outputBelts[logisticDir].startItem != null) {
                logisticDir = GetNextDir(logisticDir);
                continue;
            }

            WorldItem worldItem = items[0];
            worldItem.MoveToPosition(CalculatePositionOnBelt(worldItem, outputBelts[logisticDir]));
            outputBelts[logisticDir].SetWorldItem(worldItem);
            items.RemoveAt(0);
            logisticDir = GetNextDir(logisticDir);
        }

        splittersWaitingForResource.Clear();
    }

    public override bool ShouldSnapWithLogisticMachine(Vector2Int logisticMachineOrigin) {
        Vector2Int forwardVector = BuildingSystem.Instance.GetDirForwardVector(dir);

        if (origin - forwardVector == logisticMachineOrigin) return true;

        Vector2Int rightVector = new Vector2Int(forwardVector.y, -forwardVector.x);

        if (origin - rightVector == logisticMachineOrigin) return true;

        return origin + rightVector == logisticMachineOrigin;
    }

    private void NotifySplitter(Splitter splitter) {
        splittersWaitingForResource.Add(splitter);
    }

    private void MoveItemAfterNotify(WorldItem worldItem) {
        newItem = worldItem;
        newItem.MoveToPosition(CalculatePositionInsideMachine(inputMachine.GetDir()));
    }
}