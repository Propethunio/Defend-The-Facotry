using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsMenu : MonoBehaviour {
	[SerializeField] private GameObject buildingsMenu;
	[SerializeField] private Button closeMenuButton;
	[SerializeField] private Button factoryButton;
	[SerializeField] private Button towersButton;
	[SerializeField] private Image factoryActiveIcon;
	[SerializeField] private Image towersActiveIcon;
	[SerializeField] private RectTransform dragDropBuildingPanel;
	[SerializeField] private RectTransform factoryButtonsGrid;
	[SerializeField] private RectTransform towersButtonsGrid;
	[SerializeField] private BuildingBtn buildingBtnPrefab;

	private InputManager inputManager;
	private RectTransform mainCanvasRect;

	private void Start() {
		inputManager = Injector.Resolve<InputManager>();
		mainCanvasRect = GetComponent<RectTransform>();
		GridLayoutGroup layoutGroup = factoryButtonsGrid.GetComponent<GridLayoutGroup>();
		dragDropBuildingPanel.sizeDelta = layoutGroup.cellSize;
		GameSetupData gameSetupData = GameSetupData.Instance;
		SetupBuildingButtons(gameSetupData.GetFactoryBuildingsData(), factoryButtonsGrid, layoutGroup.constraintCount);
		SetupBuildingButtons(gameSetupData.GetTowersData(), towersButtonsGrid, layoutGroup.constraintCount);
		
		if (GameSetupData.Instance.IsTutorialLevel()) {
			gameObject.SetActive(false);
		}
		else {
			Subscribe();	
		}
	}

	private void OnDestroy() {
		Unsubscribe();
	}

	private void Subscribe() {
		closeMenuButton.onClick.AddListener(HideBuildingsMenu);
		factoryButton.onClick.AddListener(() => ShowTab(true));
		towersButton.onClick.AddListener(() => ShowTab(false));
		inputManager.BuildingMenuAction += ToggleBuildingsMenu;
	}
	
	private void Unsubscribe() {
		inputManager.BuildingMenuAction -= ToggleBuildingsMenu;
	}

	public void EnableFromTutorial() {
		gameObject.SetActive(true);
		Subscribe();
		towersButton.onClick.RemoveAllListeners();
	}

	public void EnableTowersMenu() {
		towersButton.onClick.AddListener(() => ShowTab(false));
	}
	
	private void ToggleBuildingsMenu() {
		if (buildingsMenu.activeSelf) {
			HideBuildingsMenu();
		}
		else {
			ShowBuildingsMenu();
		}
	}

	private void ShowBuildingsMenu() {
		inputManager.HandleResetBackState();
		inputManager.RegisterBackAction(HideBuildingsMenu);
		buildingsMenu.SetActive(true);
	}

	private void HideBuildingsMenu() {
		inputManager.UnregisterBackAction(HideBuildingsMenu);
		buildingsMenu.SetActive(false);
	}

	private void SetupBuildingButtons(List<BaseBuildableObjectSO> buildings, RectTransform gridTransform, int gridConstraintCount) {
		int count = buildings.Count;

		for (int i = 0; i < count; i++) {
			Instantiate(buildingBtnPrefab, gridTransform).Init(buildings[i], dragDropBuildingPanel, mainCanvasRect);
		}

		int dummyAmount = gridConstraintCount - count % gridConstraintCount;
		if (dummyAmount == gridConstraintCount) return;

		for (int i = 0; i < dummyAmount; i++) {
			Instantiate(buildingBtnPrefab, gridTransform);
		}
	}

	private void ShowTab(bool isFactoryTab) {
		factoryButtonsGrid.gameObject.SetActive(isFactoryTab);
		factoryActiveIcon.enabled = isFactoryTab;
		towersButtonsGrid.gameObject.SetActive(!isFactoryTab);
		towersActiveIcon.enabled = !isFactoryTab;
	}
}