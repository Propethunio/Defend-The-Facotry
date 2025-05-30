using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour {
	[SerializeField] private GameObject menuContainer;
	[SerializeField] private Button toggleMenuButton;
	[SerializeField] private Button continueGameButton;
	[SerializeField] private Button backToMainMenuButton;
	
	private InputManager inputManager;

	private void Start() {
		inputManager = Injector.Resolve<InputManager>();
		Subscribe();
	}

	private void OnDestroy() {
		Unsubscribe();
	}

	private void Subscribe() {
		toggleMenuButton.onClick.AddListener(ToggleMenu);
		continueGameButton.onClick.AddListener(HideBuildingsMenu);
		backToMainMenuButton.onClick.AddListener(BackToMenu);
		inputManager.OpenMenuAction += ShowBuildingsMenu;
	}

	private void Unsubscribe() {
		inputManager.OpenMenuAction -= ShowBuildingsMenu;
	}

	private void ToggleMenu() {
		if (menuContainer.activeSelf) {
			HideBuildingsMenu();
		}
		else {
			ShowBuildingsMenu();
		}
	}
	
	private void ShowBuildingsMenu() {
		inputManager.HandleResetBackState();
		inputManager.RegisterBackAction(HideBuildingsMenu);
		inputManager.ToggleMenuInput(true);
		menuContainer.SetActive(true);
	}

	private void HideBuildingsMenu() {
		inputManager.UnregisterBackAction(HideBuildingsMenu);
		inputManager.ToggleMenuInput(false);
		menuContainer.SetActive(false);
	}
	
	private void BackToMenu() {
		SceneLoader.Instance.LoadSceneGroup(0);
	}
}