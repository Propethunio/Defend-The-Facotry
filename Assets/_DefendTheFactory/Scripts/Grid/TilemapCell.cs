public class TilemapCell {

    public TilemapSprite tilemapSprite { get; private set; }
    int x;
    int y;

    public TilemapCell(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void SetTilemapSprite(TilemapSprite tilemapSprite) {
        this.tilemapSprite = tilemapSprite;
        TilemapVisual.Instance.grid.TriggerGridObjectChanged(x, y);
    }
}