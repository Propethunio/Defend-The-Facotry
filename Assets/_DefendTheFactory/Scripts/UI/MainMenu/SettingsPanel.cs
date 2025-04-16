using System.Collections.Generic;
using System.Linq;
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

	[Header("Options")] [SerializeField] private TMP_Dropdown resolutionDropdown;
	[SerializeField] private TMP_Dropdown qualityDropdown;
	[SerializeField] private Toggle fullscreenToggle;
	[SerializeField] private Slider scaleSlider;
	[SerializeField] private TMP_InputField scaleInput;
	[SerializeField] private TMP_Dropdown languageDropdown;

	private bool changed;
	private int qualityLevel;
	private Resolution[] resolutions;
	private HashSet<string> allowedRatios = new HashSet<string> { "16:9", "16:10", "4:3", "5:4", "21:9", "32:9" };

	private void SetupOptions() {
		resolutionDropdown.onValueChanged.AddListener(SetResolution);
		qualityDropdown.onValueChanged.AddListener(SetQualityLevel);
		fullscreenToggle.onValueChanged.AddListener(SetFullScreen);
		scaleSlider.onValueChanged.AddListener(SetScaleFromSlider);
		scaleInput.onValueChanged.AddListener(SetScaleFromInputField);
		languageDropdown.onValueChanged.AddListener(SetLanguage);
	}

	private void DisableOptions() {
		resolutionDropdown.onValueChanged.RemoveAllListeners();
		qualityDropdown.onValueChanged.RemoveAllListeners();
		fullscreenToggle.onValueChanged.RemoveAllListeners();
		scaleSlider.onValueChanged.RemoveAllListeners();
		scaleInput.onValueChanged.RemoveAllListeners();
		languageDropdown.onValueChanged.RemoveAllListeners();
	}

	private void SetResolution(int index) {
		string selected = resolutionDropdown.options[index].text;

		if (selected.StartsWith("--")) return;

		string[] parts = selected.Split('x');
		if (parts.Length < 2) return;

		if (int.TryParse(parts[0].Trim(), out int width) && int.TryParse(parts[1].Trim(), out int height)) {
			Screen.SetResolution(width, height, Screen.fullScreen);
		}
	}

	private void SetQualityLevel(int qualityIndex) {
		qualityLevel = qualityIndex;
		changed = true;
	}

	private void SetFullScreen(bool fullScreen) {
		Screen.fullScreen = fullScreen;
	}

	private void SetScaleFromSlider(float scale) {
		changed = true;
	}

	private void SetScaleFromInputField(string input) {
		changed = true;
	}

	private void SetLanguage(int languageIndex) { }

	private void OnApplyClicked() {
		QualitySettings.SetQualityLevel(qualityLevel);
		changed = false;
	}

	private void Start() {
		SetupResolutions();
	}

	protected override void SetupButtons() {
		resetButton.onClick.AddListener(OnResetButtonClicked);
		backButton.onClick.AddListener(OnBackButtonClicked);
		applyButton.onClick.AddListener(OnApplyClicked);
		SetupOptions();
	}

	protected override void DisableButtons() {
		resetButton.onClick.RemoveAllListeners();
		backButton.onClick.RemoveAllListeners();
		applyButton.onClick.RemoveAllListeners();
		DisableOptions();
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

	private void SetupResolutions() {
		resolutionDropdown.ClearOptions();

		resolutions = Screen.resolutions;
		Dictionary<string, List<(int width, int height)>> groupedResolutions = new Dictionary<string, List<(int, int)>>();
		HashSet<string> usedResolutions = new HashSet<string>();
		List<string> options = new List<string>();
		int currentResolutionIndex = 0;
		int indexCounter = 0;

		// Get current screen aspect ratio
		string currentAspect = GetAspectRatio(Screen.width, Screen.height);

		for (int i = 0; i < resolutions.Length; i++) {
			int width = resolutions[i].width;
			int height = resolutions[i].height;

			// Skip tiny resolutions
			if (width < 640 || height < 480) continue;

			string aspectRatio = GetAspectRatio(width, height);
			if (!allowedRatios.Contains(aspectRatio)) continue;

			string key = width + "x" + height;
			if (usedResolutions.Contains(key)) continue;
			usedResolutions.Add(key);

			if (!groupedResolutions.ContainsKey(aspectRatio)) {
				groupedResolutions[aspectRatio] = new List<(int, int)>();
			}

			groupedResolutions[aspectRatio].Add((width, height));
		}

		// Order aspect groups: current one first
		var orderedAspects = groupedResolutions.Keys.OrderBy(r => r != currentAspect) // current aspect ratio first
			.ThenBy(r => r) // others alphabetically
			.ToList();

		foreach (var aspect in orderedAspects) {
			options.Add($"<i><color=#888888>-- {aspect} --</color></i>");

			var sortedGroup = groupedResolutions[aspect].OrderBy(r => r.width).ThenBy(r => r.height).ToList();

			foreach (var res in sortedGroup) {
				string option = $"{res.width} x {res.height}";
				options.Add(option);

				if (res.width == Screen.width && res.height == Screen.height) {
					currentResolutionIndex = indexCounter + 1; // offset for label
				}

				indexCounter++;
			}

			indexCounter++; // account for label itself
		}

		resolutionDropdown.AddOptions(options);
		resolutionDropdown.value = currentResolutionIndex;
		resolutionDropdown.RefreshShownValue();
	}


	private string GetAspectRatio(int width, int height) {
		int gcd = GetGCD(width, height);
		return $"{width / gcd}:{height / gcd}";
	}

	private int GetGCD(int a, int b) {
		while (b != 0) {
			int temp = b;
			b = a % b;
			a = temp;
		}
		return a;
	}
}