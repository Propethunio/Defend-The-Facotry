using System;

public class GridCell {

    public event Action ObjectChanged;

    public BasePlacedObject placedObject { get; private set; }

    public void SetPlacedObject(BasePlacedObject placedObject) {
        this.placedObject = placedObject;
        ObjectChanged?.Invoke();
    }

    public void ClearPlacedObject() {
        placedObject = null;
        ObjectChanged?.Invoke();
    }
}