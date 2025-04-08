using System;
using System.Collections.Generic;
using UnityEngine;

public class Merger : LogisticMachine<BaseBuildableObjectSO> {
    private ConveyorBelt outputBelt;
    private List<WorldItem> newItems = new();
    private List<WorldItem> itemsReadyToGo = new();
    private Dictionary<LogisticDir, IItemProvider> inputMachines = new();
    private Dictionary<LogisticDir, Vector2Int> inputPositions = new();

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

        SetupInput(backPosition, LogisticDir.Straight);
        SetupInput(leftPosition, LogisticDir.Left);
        SetupInput(rightPosition, LogisticDir.Right);
        SetupOutputBelt(nextPosition);
    }

    private void SetupInput(Vector2Int position, LogisticDir logisticDir) {
        inputMachines[logisticDir] = null;

        if (!IsPositionValid(position)) return;

        inputPositions[logisticDir] = position;
        Action action = () => HandleGridObjectChange(logisticDir);
        gridArray[position.x, position.y].ObjectChanged += action;
        objectChangedEvents.Add(action, position);

        if (ShouldSnap(position, out IItemProvider itemProvider)) {
            inputMachines[logisticDir] = itemProvider;
        }
    }

    public override bool IsOnOutputCell(Vector2Int position) {
        return BuildingSystem.Instance.GetDirForwardVector(dir) + origin == position;
    }

    private void HandleGridObjectChange(LogisticDir dir) {
        Vector2Int position = inputPositions[dir];

        if (ShouldSnap(position, out IItemProvider itemProvider)) {
            inputMachines[dir] = itemProvider;
        }
        else {
            inputMachines[dir] = null;
        }
    }

    private void SetupOutputBelt(Vector2Int position) {
        if (!IsPositionValid(position)) return;

        Action action = () => HandleGridObjectChange(position);
        gridArray[position.x, position.y].ObjectChanged += action;
        objectChangedEvents.Add(action, position);

        if (!ShouldSnapBack(position, out ConveyorBelt belt)) return;

        outputBelt = belt;
        belt.SetLogisticMachineAsParent(this);
    }

    private void HandleGridObjectChange(Vector2Int position) {
        outputBelt = ShouldSnapBack(position, out ConveyorBelt belt) ? belt : null;
    }

    public override void DestroySelf() {
        int indexCount = newItems.Count;

        for (int i = 0; i < indexCount; i++) {
            newItems[i].DestroySelf();
        }

        indexCount = itemsReadyToGo.Count;

        for (int i = 0; i < indexCount; i++) {
            itemsReadyToGo[i].DestroySelf();
        }

        base.DestroySelf();
    }

    protected override void OnEarlyTick() {
        if (itemsReadyToGo.Count > 0) {
            items.AddRange(itemsReadyToGo);
            itemsReadyToGo.Clear();
        }

        if (newItems.Count > 0) {
            itemsReadyToGo.AddRange(newItems);
            newItems.Clear();
        }

        int storedItems = items.Count + itemsReadyToGo.Count;

        if (storedItems == maxStorage) return;

        for (int i = 3; i > 0; i--) {
            if (inputMachines[logisticDir] == null || !inputMachines[logisticDir].HasItem()) {
                logisticDir = GetNextDir(logisticDir);
                continue;
            }

            WorldItem item = inputMachines[logisticDir].GetWorldItem();
            item.MoveToPosition(CalculatePositionInsideMachine(inputMachines[logisticDir].GetDir()));
            newItems.Add(item);
            logisticDir = GetNextDir(logisticDir);
            storedItems++;

            if (storedItems == maxStorage) return;
        }
    }

    protected override void OnLateTick() {
        if (items.Count == 0 || outputBelt == null || outputBelt.startItem != null) return;

        WorldItem worldItem = items[0];
        worldItem.MoveToPosition(CalculatePositionOnBelt(worldItem, outputBelt));
        outputBelt.SetWorldItem(worldItem);
        items.RemoveAt(0);
    }

    public override bool ShouldSnapWithLogisticMachine(Vector2Int logisticMachineOrigin) {
        return origin + BuildingSystem.Instance.GetDirForwardVector(dir) == logisticMachineOrigin;
    }
}