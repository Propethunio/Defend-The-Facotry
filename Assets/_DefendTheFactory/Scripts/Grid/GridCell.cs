using System;

public class GridCell {
    public event Action ObjectChanged;

    public bool isPathCell{ get; private set; }
    public BasePlacedObject placedObject { get; private set; }

    public void SetPlacedObject(BasePlacedObject placedObject) {
        this.placedObject = placedObject;
        ObjectChanged?.Invoke();
    }

    public void ClearPlacedObject() {
        placedObject = null;
        ObjectChanged?.Invoke();
    }

    public void MarkPathCell() {
        isPathCell = true;
    }
    
    public void UnmarkPathCell() {
        isPathCell = false;
    }
}