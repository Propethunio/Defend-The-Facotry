using System;
using UnityEngine;

public class Grid<GridObject> {

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }

    public int width { get; private set; }
    public int height { get; private set; }
    public GridObject[,] gridArray { get; private set; }

    public Grid(int width, int height, Func<Grid<GridObject>, int, int, GridObject> createGridObject) {
        this.width = width;
        this.height = height;
        gridArray = new GridObject[width, height];

        for(int x = 0; x < gridArray.GetLength(0); x++) {
            for(int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, 0, y);
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt(worldPosition.x);
        y = Mathf.FloorToInt(worldPosition.y);
    }

    public void SetGridObject(int x, int y, GridObject value) {
        if(x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
            TriggerGridObjectChanged(x, y);
        }
    }

    public void TriggerGridObjectChanged(int x, int y) {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public void SetGridObject(Vector3 worldPosition, GridObject value) {
        GetXZ(worldPosition, out int x, out int y);
        SetGridObject(x, y, value);
    }

    public GridObject GetGridObject(int x, int y) {
        if(x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else {
            return default(GridObject);
        }
    }

    public GridObject GetGridObject(Vector3 worldPosition) {
        GetXZ(worldPosition, out int x, out int y);
        return GetGridObject(x, y);
    }

    public bool IsValidGridPosition(Vector2Int gridPosition) {
        int x = gridPosition.x;
        int y = gridPosition.y;

        if(x >= 0 && y >= 0 && x < width && y < height) {
            return true;
        } else {
            return false;
        }
    }

    public bool IsValidGridPositionWithPadding(Vector2Int gridPosition) {
        Vector2Int padding = new Vector2Int(2, 2);
        int x = gridPosition.x;
        int y = gridPosition.y;

        if(x >= padding.x && y >= padding.y && x < width - padding.x && y < height - padding.y) {
            return true;
        } else {
            return false;
        }
    }
}