using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : BaseMenuPanel {
	[Header("Panels")] [SerializeField] private GameObject menuPanel;
	[SerializeField] private GameObject warningPop;

	[Header("Main Buttons")] [SerializeField] private Button resetButton;
	[SerializeField] private Button backButton;
	[SerializeField] private Button applyButton;

	[Header("Warning Buttons")] [SerializeField] private Button warningCancelButton;
	[SerializeField] private Button warningConfirmButton;

	[Header("Text")] [SerializeField] private TextMeshProUGUI warningText;

	private bool changed;

	protected override void SetupButtons() {
		resetButton.onClick.AddListener(OnResetButtonClicked);
		backButton.onClick.AddListener(OnBackButtonClicked);
		applyButton.onClick.AddListener(OnApplyClicked);
	}

	protected override void DisableButtons() {
		resetButton.onClick.RemoveAllListeners();
		backButton.onClick.RemoveAllListeners();
		applyButton.onClick.RemoveAllListeners();
	}

	private void OnApplyClicked() {
		//APPLY AND SAVE
		changed = false;
	}

	private void OnResetButtonClicked() {
		DisableButtons();
		warningPop.SetActive(true);
		warningText.text = "Are you sure you want to reset all settings to default?";
		warningCancelButton.onClick.AddListener(WarningPopCancel);
		warningConfirmButton.onClick.AddListener(ResetWarningOnConfirm);
	}

	private void OnBackButtonClicked() {
		if (changed) {
			DisableButtons();
			warningPop.SetActive(true);
			warningText.text = "You have not applied changes! Are you sure you want to go back?";
			warningCancelButton.onClick.AddListener(WarningPopCancel);
			warningConfirmButton.onClick.AddListener(BackWarningOnConfirm);
		}
		else {
			ShowPanel(menuPanel);
		}
	}

	private void WarningPopCancel() {
		HideWarningPop();
		SetupButtons();
	}

	private void HideWarningPop() {
		warningPop.SetActive(false);
		warningCancelButton.onClick.RemoveAllListeners();
		warningConfirmButton.onClick.RemoveAllListeners();
	}

	private void ResetWarningOnConfirm() {
		HideWarningPop();
		SetupButtons();
		//RESET TO DEFAULT
		// ?? changed = true;
	}

	private void BackWarningOnConfirm() {
		HideWarningPop();
		ShowPanel(menuPanel);
	}
}