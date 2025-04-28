using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostResourceUI : MonoBehaviour {
	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text amountText;

	private static readonly Color32 positiveColor = new Color32(0xDF, 0xDF, 0xDF, 0xFF);
	private static readonly Color32 negativeColor = new Color32(0xF5, 0x58, 0x58, 0xFF);

	private ItemsManager itemsManager;
	private ItemSO item;
	private int requiredAmount;

	private void OnDestroy() {
		itemsManager.ItemChanged -= OnItemChanged;
	}

	public void Init(ItemIntPair itemCostPair) {
		itemsManager = Injector.Resolve<ItemsManager>();
		itemsManager.ItemChanged += OnItemChanged;
		item = itemCostPair.item;
		requiredAmount = itemCostPair.amount;
		icon.sprite = item.icon;
		SetupText();
	}

	private void OnItemChanged(ItemSO item, int amount) {
		if (this.item != item) return;

		SetupText();
	}

	private void SetupText() {
		int currentAmount = itemsManager.GetAmount(item);
		amountText.text = currentAmount + "/" + requiredAmount;
		amountText.color = currentAmount < requiredAmount ? negativeColor : positiveColor;
	}
}