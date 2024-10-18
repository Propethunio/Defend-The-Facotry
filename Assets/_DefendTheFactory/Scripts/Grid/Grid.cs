using System;

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

    public void TriggerGridObjectChanged(int x, int y) {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }
}