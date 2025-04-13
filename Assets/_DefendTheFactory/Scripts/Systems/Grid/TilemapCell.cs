public class TilemapCell {
    public TilemapSprite tilemapSprite { get; private set; }
    private int x;
    private int y;

    public TilemapCell(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void SetTilemapSprite(TilemapSprite tilemapSprite) {
        this.tilemapSprite = tilemapSprite;
        Injector.Resolve<TilemapVisual>().grid.TriggerGridObjectChanged(x, y);
    }
}