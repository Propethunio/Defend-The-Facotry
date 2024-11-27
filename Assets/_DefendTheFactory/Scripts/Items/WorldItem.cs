using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour {

    public static WorldItem Create(Vector2Int gridPosition, ItemSO itemScriptableObject) {
        Transform worldItemTransform = Instantiate(GameAssets.i.pfWorldItem, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
        WorldItem worldItem = worldItemTransform.GetComponent<WorldItem>();
        worldItem.itemSO = itemScriptableObject;

        return worldItem;
    }

    bool hasAlreadyMoved;
    ItemSO itemSO;
    Tween moveTween;

    //void Start() {
     //   transform.Find("ItemVisual").Find("itemSprite").GetComponent<SpriteRenderer>().sprite = itemSO.sprite;
    //}

    public void MoveToGridPosition(Vector2Int gridPosition) {
        if(moveTween != null && moveTween.IsActive()) {
            moveTween.Kill();
        }

        moveTween = transform.DOMove(new Vector3(gridPosition.x, 0, gridPosition.y), 1f).SetEase(Ease.Linear);
    }

    public bool CanMove() {
        return !hasAlreadyMoved;
    }

    public void SetHasAlreadyMoved() {
        hasAlreadyMoved = true;
    }

    public void ResetHasAlreadyMoved() {
        hasAlreadyMoved = false;
    }

    public ItemSO GetItemSO() {
        return itemSO;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}