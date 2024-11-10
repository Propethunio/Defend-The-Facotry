using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour {

    public static WorldItem Create(Vector2Int gridPosition, ItemSO itemScriptableObject) {
        Transform worldItemTransform = Instantiate(GameAssets.i.pfWorldItem, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

        WorldItem worldItem = worldItemTransform.GetComponent<WorldItem>();
        worldItem.SetGridPosition(gridPosition);
        worldItem.itemSO = itemScriptableObject;

        return worldItem;
    }

    private Vector2Int gridPosition;
    private bool hasAlreadyMoved;
    private ItemSO itemSO;

    private void Start() {
        transform.Find("ItemVisual").Find("itemSprite").GetComponent<SpriteRenderer>().sprite = itemSO.sprite;
    }

    private void Update() {
        transform.position = Vector3.Lerp(transform.position, BuildingSystem.Instance.GetWorldPosition(gridPosition), Time.deltaTime * 10f);
    }

    public void SetGridPosition(Vector2Int gridPosition) {
        this.gridPosition = gridPosition;
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
