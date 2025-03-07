using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleItemUI : MonoBehaviour {

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;

    private ItemSO item;

    private void OnDestroy() {
        ItemsManager.Instance.ItemChanged -= OnItemChanged;
    }

    public void Init(ItemSO item) {
        ItemsManager.Instance.ItemChanged += OnItemChanged;
        icon.sprite = item.icon;
        this.item = item;
    }

    private void OnItemChanged(ItemSO item, int amount) {
        if(this.item != item) return;

        amountText.text = amount.ToString();
    }
}