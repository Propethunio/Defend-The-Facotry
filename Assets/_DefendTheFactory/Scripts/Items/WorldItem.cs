using DG.Tweening;
using UnityEngine;

public class WorldItem : MonoBehaviour {

    public ItemSO itemSO { get; private set; }

    Tween moveTween;

    public static WorldItem Create(Vector2Int gridPosition, BuildingDir dir, ItemSO itemScriptableObject) {
        Vector3 worldPosition = new Vector3(gridPosition.x, 0.31f, gridPosition.y);
        Quaternion rotation = Quaternion.identity;

        switch(dir) {
            case BuildingDir.Down:
                worldPosition += new Vector3(0.5f, 0, 0.75f);
                rotation = Quaternion.Euler(0, 180, 0);
                break;
            case BuildingDir.Left:
                worldPosition += new Vector3(0.75f, 0, 0.5f);
                rotation = Quaternion.Euler(0, 270, 0);
                break;
            case BuildingDir.Up:
                worldPosition += new Vector3(0.5f, 0, 0.25f);
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            case BuildingDir.Right:
                worldPosition += new Vector3(0.25f, 0, 0.5f);
                rotation = Quaternion.Euler(0, 90, 0);
                break;
        }

        WorldItem worldItem = Instantiate(itemScriptableObject.prefab, worldPosition, rotation).AddComponent<WorldItem>();
        worldItem.itemSO = itemScriptableObject;

        return worldItem;
    }

    public void MoveToPosition(Vector2 worldPosition) {
        if(moveTween != null && moveTween.IsActive()) {
            moveTween.Kill();
        }

        moveTween = transform.DOMove(new Vector3(worldPosition.x, transform.position.y, worldPosition.y), .5f).SetEase(Ease.Linear);
    }

    public void MoveToPositionCurved(Vector2 worldPosition) {
        if(moveTween != null && moveTween.IsActive()) {
            moveTween.Kill();
        }

        // Define the curved path points
        Vector3 startPosition = transform.position;
        Vector3 controlPoint = new Vector3(Mathf.Floor(startPosition.x) + .5f, startPosition.y, Mathf.Floor(startPosition.z) + .5f);
        Vector3 endPosition = new Vector3(worldPosition.x, startPosition.y, worldPosition.y);

        // Create a curved path using CatmullRom or a custom curve
        moveTween = transform.DOPath(new Vector3[] { startPosition, controlPoint, endPosition }, .5f, PathType.CatmullRom)
            .SetEase(Ease.Linear).SetLookAt(0.01f); // Smooth easing for curve motion
    }

    public void DestroySelf() {
        if(moveTween != null && moveTween.IsActive()) {
            moveTween.Kill();
        }

        Destroy(gameObject);
    }
}