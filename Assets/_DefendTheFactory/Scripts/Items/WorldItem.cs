using DG.Tweening;
using UnityEngine;

public class WorldItem : MonoBehaviour {

    bool hasAlreadyMoved;
    public ItemSO itemSO { get; private set; }
    Tween moveTween;

    public static WorldItem Create(Vector2Int gridPosition, ItemSO itemScriptableObject) {
        Transform worldItemTransform = Instantiate(itemScriptableObject.prefab.transform, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
        WorldItem worldItem = worldItemTransform.GetComponent<WorldItem>();
        worldItem.itemSO = itemScriptableObject;

        return worldItem;
    }

    public void MoveToGridPosition(Vector2Int gridPosition) {
        if(moveTween != null && moveTween.IsActive()) {
            moveTween.Kill();
        }

        moveTween = transform.DOMove(new Vector3(gridPosition.x, 0, gridPosition.y), 1f).SetEase(Ease.Linear);
    }


    public void DestroySelf() {
        Destroy(gameObject);
    }
}