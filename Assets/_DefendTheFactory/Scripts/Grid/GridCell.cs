public class GridCell {

    public PlacedObject placedObject { get; private set; }
    int x;
    int y;

    public GridCell(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void SetPlacedObject(PlacedObject placedObject) {
        this.placedObject = placedObject;
        BuildingSystem.Instance.grid.TriggerGridObjectChanged(x, y);
    }

    public void ClearPlacedObject() {
        placedObject = null;
        BuildingSystem.Instance.grid.TriggerGridObjectChanged(x, y);
    }
}