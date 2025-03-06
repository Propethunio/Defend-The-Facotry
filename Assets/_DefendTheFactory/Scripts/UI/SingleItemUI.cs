using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleItemUI : MonoBehaviour {

    [SerializeField] Image icon;
    [SerializeField] TMP_Text amountText;

    ItemSO item;

    void OnDestroy() {
        ItemsManager.Instance.ItemChanged -= OnItemChanged;
    }

    public void Init(ItemSO item) {
        ItemsManager.Instance.ItemChanged += OnItemChanged;
        icon.sprite = item.icon;
        this.item = item;
    }

    void OnItemChanged(ItemSO item, int amount) {
        if(this.item != item) return;

        amountText.text = amount.ToString();
    }
}