using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestStepUi : MonoBehaviour {
	[SerializeField] private TMP_Text text;
	[SerializeField] private Image checkmark;

	public void Init(string stepDsc) {
		text.text = stepDsc;
	}

	public void Complete() {
		text.text = $"<s>{text.text}</s>";
		checkmark.enabled = true;
	}
}