using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : PlacedObject {

    private enum State {
        Cooldown,
        WaitingForItemToGrab,
        MovingToDropItem,
        DroppingItem,
    }

    private Vector2Int grabPosition;
    private Vector2Int dropPosition;
    private WorldItem holdingItem;
    private ItemSO grabFilterItemSO;
    private float timer;
    private string textString = "";
    private State state;



    protected override void Setup() {
        grabPosition = origin;
        dropPosition = origin;

        state = State.Cooldown;

        transform.Find("GrabberVisual").Find("ArrowGrab").gameObject.SetActive(false);
        transform.Find("GrabberVisual").Find("ArrowDrop").gameObject.SetActive(false);

        grabFilterItemSO = GameAssets.i.itemSO_Refs.any;
    }

    public override string ToString() {
        return textString;
    }


    private void Update() {
        switch (state) {
            default:
            case State.Cooldown:
                timer -= Time.deltaTime;
                if (timer <= 0f) {
                    state = State.WaitingForItemToGrab;
                }
                break;
            case State.WaitingForItemToGrab:
                PlacedObject grabPlacedObject = BuildingSystem.Instance.GetGridObject(grabPosition).placedObject;
                PlacedObject dropPlacedObject = BuildingSystem.Instance.GetGridObject(dropPosition).placedObject;

                if (grabPlacedObject != null && dropPlacedObject != null) {
                    // Objects exist on both places

                    // Type of object that can be dropped
                    ItemSO[] dropFilterItemSO = new ItemSO[] { GameAssets.i.itemSO_Refs.none };

                    if (dropPlacedObject is IItemStorage) {
                        dropFilterItemSO = (dropPlacedObject as IItemStorage).GetItemSOThatCanStore();
                    }
                    if (dropPlacedObject is IWorldItemSlot) {
                        dropFilterItemSO = (dropPlacedObject as IWorldItemSlot).GetItemSOThatCanStore();
                    }

                    // Combine Drop and Grab filters
                    dropFilterItemSO = ItemSO.GetCombinedFilter(new ItemSO[] { grabFilterItemSO }, dropFilterItemSO);

                    if (ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.none, dropFilterItemSO)) {
                        // Cannot drop any item, so dont grab anything
                        break;
                    }

                    // Is Grab PlacedObject a Item Storage?
                    if (grabPlacedObject is IItemStorage) {
                        IItemStorage itemStorage = grabPlacedObject as IItemStorage;
                        if (itemStorage.TryGetStoredItem(dropFilterItemSO, out ItemSO itemScriptableObject)) {
                            // ## DEBUG
                            textString = Time.realtimeSinceStartup.ToString("F2");
                            TriggerGridObjectChanged();
                            // ## DEBUG

                            holdingItem = WorldItem.Create(grabPosition, itemScriptableObject);
                            holdingItem.MoveToGridPosition(grabPosition);

                            state = State.MovingToDropItem;
                            float TIME_TO_DROP_ITEM = .5f;
                            timer = TIME_TO_DROP_ITEM;
                        }
                    }

                    // Is Grab PlacedObject a WorldItemSlot?
                    if (grabPlacedObject is IWorldItemSlot) {
                        IWorldItemSlot worldItemSlot = grabPlacedObject as IWorldItemSlot;
                        if (worldItemSlot.TryGetWorldItem(dropFilterItemSO, out holdingItem)) {
                            holdingItem.MoveToGridPosition(grabPosition);

                            state = State.MovingToDropItem;
                            float TIME_TO_DROP_ITEM = .5f;
                            timer = TIME_TO_DROP_ITEM;
                        }
                    }
                }
                break;
            case State.MovingToDropItem:
                timer -= Time.deltaTime;
                if (timer <= 0f) {
                    state = State.DroppingItem;
                }
                break;
            case State.DroppingItem:
                dropPlacedObject = BuildingSystem.Instance.GetGridObject(dropPosition).placedObject;
                // Does it have a place to drop the item?
                if (dropPlacedObject != null) {
                    // Is it a World Item Slot?
                    if (dropPlacedObject is IWorldItemSlot) {
                        IWorldItemSlot worldItemSlot = dropPlacedObject as IWorldItemSlot;
                        // Try to Set World Item
                        if (worldItemSlot.TrySetWorldItem(holdingItem)) {
                            // It worked, drop item
                            holdingItem.MoveToGridPosition(worldItemSlot.GetGridPosition());
                            holdingItem = null;

                            state = State.Cooldown;
                            float COOLDOWN_TIME = .2f;
                            timer = COOLDOWN_TIME;
                        } else {
                            // Cannot drop, slot must be full
                            // Continue trying...
                        }
                    }

                    // Is it a Item Storage?
                    if (dropPlacedObject is IItemStorage) {
                        IItemStorage itemStorage = dropPlacedObject as IItemStorage;
                        // Try to Set World Item
                        if (itemStorage.TryStoreItem(holdingItem.GetItemSO())) {
                            // It worked, drop item, destroy world item
                            holdingItem.DestroySelf();
                            holdingItem = null;

                            state = State.Cooldown;
                            float COOLDOWN_TIME = .2f;
                            timer = COOLDOWN_TIME;
                        } else {
                            // Cannot drop, storage must be full
                            // Continue trying...
                        }
                    }
                }
                break;
        }
    }

    public ItemSO GetGrabFilterItemSO() {
        return grabFilterItemSO;
    }

    public void SetGrabFilterItemSO(ItemSO grabFilterItemSO) {
        this.grabFilterItemSO = grabFilterItemSO;
    }

}

