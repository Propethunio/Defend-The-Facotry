using UnityEngine;
using UnityEngine.UI;

public class NewGamePanel : BaseMenuPanel {
	[Header("Panels")] [SerializeField] private GameObject menuPanel;

	[Header("Main Buttons")] [SerializeField] private Button startButton;
	[SerializeField] private Button backButton;

	[Header("Level Buttons")] [SerializeField] private Button tutorialButton;
	[SerializeField] private Button level1Button;

	private Image tutorialImage;
	private Image level1Image;
	private int selectedLevelIndex;
	private Color notSelectedColor;
	private Color selectedColor;

	private void Awake() {
		tutorialImage = tutorialButton.GetComponent<Image>();
		level1Image = level1Button.GetComponent<Image>();
		notSelectedColor = tutorialImage.color;
		selectedColor = notSelectedColor;
		selectedColor.a = 1f;
	}
	
	protected override void OnEnable() {
		base.OnEnable();
		selectedLevelIndex = -1;
		tutorialImage.color = notSelectedColor;
		level1Image.color = notSelectedColor;
	}

	protected override void SetupButtons() {
		startButton.onClick.AddListener(OnStartButtonClicked);
		backButton.onClick.AddListener(OnBackButtonClicked);
		tutorialButton.onClick.AddListener(() => OnLevelButtonClicked(0));
		level1Button.onClick.AddListener(() => OnLevelButtonClicked(1));
	}

	protected override void DisableButtons() {
		startButton.onClick.RemoveAllListeners();
		backButton.onClick.RemoveAllListeners();
		tutorialButton.onClick.RemoveAllListeners();
		level1Button.onClick.RemoveAllListeners();
	}

	private void OnStartButtonClicked() {
		if (selectedLevelIndex == -1) return;
		
		GameSetupData.Instance.StartLevel(selectedLevelIndex);
	}
	
	private void OnBackButtonClicked() {
		ShowPanel(menuPanel);
	}

	private void OnLevelButtonClicked(int index) {
		selectedLevelIndex = index;

		if (index == 0) {
			tutorialImage.color = selectedColor;
			level1Image.color = notSelectedColor;
		}
		else {
			tutorialImage.color = notSelectedColor;
			level1Image.color = selectedColor;
		}
	}
}