using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeIcon : MonoBehaviour {
	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text amountText;

	public void SetIcon(Sprite sprite) {
		icon.sprite = sprite;
	}

	public void SetCost(Sprite sprite, int amount) {
		icon.sprite = sprite;
		amountText.text = amount.ToString();
	}
}