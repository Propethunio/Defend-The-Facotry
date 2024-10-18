using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeMonkey.Utils;

public class StorageUI : MonoBehaviour {

    public static StorageUI Instance { get; private set; }


    private Storage storage;

    private void Awake() {
        Instance = this;

        transform.Find("CloseBtn").GetComponent<Button_UI>().ClickFunc = () => {
            Hide();
        };

        Hide();
    }

    private void UpdateItemList() {
        Transform itemContainer = transform.Find("ItemContainer");
        Transform itemTemplate = itemContainer.Find("Template");
        itemTemplate.gameObject.SetActive(false);

        // Destory old transforms
        foreach (Transform transform in itemContainer) {
            if (transform != itemTemplate) {
                Destroy(transform.gameObject);
            }
        }

        ItemStackList itemStackList = storage.GetItemStackList();

        foreach (ItemStack itemStack in itemStackList.GetItemStackList()) {
            Transform itemTransform = Instantiate(itemTemplate, itemContainer);
            itemTransform.gameObject.SetActive(true);

            itemTransform.Find("Icon").GetComponent<Image>().sprite = itemStack.itemSO.sprite;
            itemTransform.Find("Text").GetComponent<TextMeshProUGUI>().text = itemStack.amount.ToString();
        }
    }

    private void Storage_OnItemStorageCountChanged(object sender, System.EventArgs e) {
        UpdateItemList();
    }

    public void Show(Storage storage) {
        gameObject.SetActive(true);

        if (this.storage != null) {
            this.storage.OnItemStorageCountChanged -= Storage_OnItemStorageCountChanged;
        }

        this.storage = storage;

        if (storage != null) {
            storage.OnItemStorageCountChanged += Storage_OnItemStorageCountChanged;
            UpdateItemList();
        }
    }

    public void Hide() {
        gameObject.SetActive(false);

        if (this.storage != null) {
            this.storage.OnItemStorageCountChanged -= Storage_OnItemStorageCountChanged;
        }

        storage = null;
    }

}
