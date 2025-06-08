using System;
using System.Collections.Generic;
using TMPro;
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
	[SerializeField] private RectTransform infoContainer;
	[SerializeField] private TMP_Text buildingNameText;
	[SerializeField] private TMP_Text buildingDscText;
	[SerializeField] private RectTransform recipesPanel;
	[SerializeField] private RectTransform recipesIconsPanel;
	[SerializeField] private RectTransform costPanel;
	[SerializeField] private RecipeIcon recipeIconPrefab;
	[SerializeField] private RecipeIcon costIconPrefab;

	private InputManager inputManager;
	private RectTransform mainCanvasRect;
	private BuildingBtn currentBuildingBtn;

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
		infoContainer.gameObject.SetActive(false);

		if (currentBuildingBtn == null) return;
		
		currentBuildingBtn.ToggleHighlight(false);
		currentBuildingBtn = null;
	}

	private void HideBuildingsMenu() {
		inputManager.UnregisterBackAction(HideBuildingsMenu);
		buildingsMenu.SetActive(false);
	}

	private void SetupBuildingButtons(List<BaseBuildableObjectSO> buildings, RectTransform gridTransform, int gridConstraintCount) {
		int count = buildings.Count;

		for (int i = 0; i < count; i++) {
			BuildingBtn btn = Instantiate(buildingBtnPrefab, gridTransform);
			btn.Init(buildings[i], dragDropBuildingPanel, mainCanvasRect);
			btn.onBtnClick += OnBtnClicked;
		}

		int dummyAmount = gridConstraintCount - count % gridConstraintCount;
		if (dummyAmount == gridConstraintCount) return;

		for (int i = 0; i < dummyAmount; i++) {
			Instantiate(buildingBtnPrefab, gridTransform);
		}
	}

	private void OnBtnClicked(BaseBuildableObjectSO data, BuildingBtn btn) {
		if (currentBuildingBtn == btn) return;

		ToggleBtnsHighlights(btn);
		SetInfoPanelData(data);
	}

	private void ToggleBtnsHighlights(BuildingBtn btn) {
		if (currentBuildingBtn != null) {
			currentBuildingBtn.ToggleHighlight(false);
		}

		btn.ToggleHighlight(true);
		currentBuildingBtn = btn;
	}

	private void SetInfoPanelData(BaseBuildableObjectSO data) {
		buildingNameText.text = data.nameString;
		buildingDscText.text = data.description;

		foreach (Transform child in recipesIconsPanel.transform) {
			Destroy(child.gameObject);
		}

		if (data is GatheringMachineSO gatheringMachine) {
			Instantiate(recipeIconPrefab, recipesIconsPanel).SetIcon(gatheringMachine.producedItem.icon);
			recipesPanel.gameObject.SetActive(true);
		}
		else if (data is ConstructorSO constructor) {
			foreach (SimpleItemRecipeSO recipe in constructor.itemRecipeList) {
				Instantiate(recipeIconPrefab, recipesIconsPanel).SetIcon(recipe.outputItem.item.icon);
			}
			recipesPanel.gameObject.SetActive(true);
		}
		else if (data is AssemblerSO assembler) {
			foreach (ItemRecipeSO recipe in assembler.itemRecipeList) {
				Instantiate(recipeIconPrefab, recipesIconsPanel).SetIcon(recipe.outputItemList[0].item.icon);
			}
			recipesPanel.gameObject.SetActive(true);
		}
		else {
			recipesPanel.gameObject.SetActive(false);
		}

		foreach (Transform child in costPanel.transform) {
			Destroy(child.gameObject);
		}

		for (int index = 0; index < data.cost.Count; index++) {
			ItemIntPair cost = data.cost[index];
			Instantiate(costIconPrefab, costPanel).SetCost(cost.item.icon, cost.amount);
		}
		
		infoContainer.gameObject.SetActive(true);
	}

	private void ShowTab(bool isFactoryTab) {
		factoryButtonsGrid.gameObject.SetActive(isFactoryTab);
		factoryActiveIcon.enabled = isFactoryTab;
		towersButtonsGrid.gameObject.SetActive(!isFactoryTab);
		towersActiveIcon.enabled = !isFactoryTab;
	}
}