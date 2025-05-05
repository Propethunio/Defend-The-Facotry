using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleResourceUI : MonoBehaviour {
	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text amountText;

	private ItemsManager itemsManager;
	private ItemSO item;

	public event Action Removed;

	private void OnDestroy() {
		itemsManager.ItemChanged -= OnItemChanged;
		itemsManager.ItemRemoved -= OnItemRemoved;
	}

	public void Init(ItemSO item) {
		itemsManager = Injector.Resolve<ItemsManager>();
		itemsManager.ItemChanged += OnItemChanged;
		itemsManager.ItemRemoved += OnItemRemoved;
		icon.sprite = item.icon;
		this.item = item;
	}

	private void OnItemChanged(ItemSO item, int amount) {
		if (this.item != item) return;

		amountText.text = amount.ToString();
	}

	private void OnItemRemoved(ItemSO item) {
		if (this.item != item) return;

		Removed?.Invoke();
		Destroy(gameObject);
	}
}