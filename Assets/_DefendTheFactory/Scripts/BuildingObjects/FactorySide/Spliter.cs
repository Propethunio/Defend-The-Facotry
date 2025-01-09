using System.Collections.Generic;
using UnityEngine;

public class Spliter : LogisticMachine {

    int suckedItems;
    List<WorldItem> items = new();
    [SerializeField] ItemSO itemToCreate;
    LogisticDir logisticDir = LogisticDir.Straight;

    public override void GridSetupDone() {
        buildingSystem = BuildingSystem.Instance;
        Subscribe();
    }

    public override void OnEarlyTick() {
        if(suckedItems > 0) {
            currentStorage += suckedItems;
            suckedItems = 0;
            items.Clear();
        }

        if(currentStorage > maxStorage) return;

        Vector2Int forwardVector = buildingSystem.GetDirForwardVector(dir);
        Vector2Int nextPosition = origin + forwardVector;

        GridCell[,] gridArray = buildingSystem.grid.gridArray;

        if(ShouldSnap(gridArray, nextPosition, out ConveyorBelt belt)) {
            if(belt.worldItem != null) {
                belt.worldItem.MoveToGridPosition(origin);
                items.Add(belt.worldItem);
                belt.ResetWorldItem();
                suckedItems++;
            }
        }
    }

    public override void OnTick() {

    }

    public override void OnLateTick() {
        if(currentStorage == 0) return;

        Vector2Int forwardVector = buildingSystem.GetDirForwardVector(dir);

        GridCell[,] gridArray = buildingSystem.grid.gridArray;
        Vector2Int backPosition = origin - forwardVector;

        Vector2Int rightVector = new Vector2Int(forwardVector.y, -forwardVector.x);
        Vector2Int rightPosition = origin + rightVector;
        Vector2Int leftPosition = origin - rightVector;



        for(int i = 3; i > 0; i--) {
            Vector2Int operationDir = new();

            switch(logisticDir) {
                case LogisticDir.Straight:
                    operationDir = backPosition;
                    break;
                case LogisticDir.Left:
                    operationDir = leftPosition;
                    break;
                case LogisticDir.Right:
                    operationDir = rightPosition;
                    break;
            }

            if(ShouldSnapBack(gridArray, operationDir, out ConveyorBelt belt)) {
                if(belt.worldItem == null) {
                    WorldItem worldItem = WorldItem.Create(origin, itemToCreate);
                    worldItem.MoveToGridPosition(operationDir);
                    belt.TrySetWorldItem(worldItem);
                    currentStorage--;
                }
            }

            logisticDir = GetNextDir(logisticDir);
            if(currentStorage == 0) {
                break;
            }
        }
    }

    bool ShouldSnap(GridCell[,] gridArray, Vector2Int position, out ConveyorBelt belt) {
        belt = null;

        if(!IsPositionValid(gridArray, position)) return false;

        belt = gridArray[position.x, position.y].placedObject as ConveyorBelt;
        return belt != null && belt.nextPosition == origin;
    }

    bool ShouldSnapBack(GridCell[,] gridArray, Vector2Int position, out ConveyorBelt belt) {
        belt = null;

        if(!IsPositionValid(gridArray, position)) return false;

        belt = gridArray[position.x, position.y].placedObject as ConveyorBelt;
        return belt != null && belt.previousPosition == origin;
    }

    bool IsPositionValid(GridCell[,] gridArray, Vector2Int position) {
        return position.x >= 0 && position.x < gridArray.GetLength(0) && position.y >= 0 && position.y < gridArray.GetLength(1);
    }
}
