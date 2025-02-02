using System;

public class GridCell {

    public Action ObjectChanged;

    public PlacedObject placedObject { get; private set; }
    int x;
    int y;

    public GridCell(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void SetPlacedObject(PlacedObject placedObject) {
        this.placedObject = placedObject;
        ObjectChanged?.Invoke();
        BuildingSystem.Instance.grid.TriggerGridObjectChanged(x, y);
    }

    public void ClearPlacedObject() {
        placedObject = null;
        ObjectChanged?.Invoke();
        BuildingSystem.Instance.grid.TriggerGridObjectChanged(x, y);
    }
}