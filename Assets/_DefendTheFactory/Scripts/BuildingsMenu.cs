using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsMenu : MonoBehaviour {
	[SerializeField] private Button factoryButton;
	[SerializeField] private Button towersButton;
	[SerializeField] private Image factoryActiveIcon;
	[SerializeField] private Image towersActiveIcon;
	[SerializeField] private RectTransform dragDropBuildingPanel;
	[SerializeField] private RectTransform factoryButtonsGrid;
	[SerializeField] private RectTransform towersButtonsGrid;
	[SerializeField] private BuildingBtn buildingBtnPrefab;
	[SerializeField] private List<BaseBuildableObjectSO> factoryData;
	[SerializeField] private List<BaseBuildableObjectSO> towersData;

	private RectTransform mainCanvasRect;

	private void Start() {
		mainCanvasRect = GetComponent<RectTransform>();
		GridLayoutGroup layoutGroup = factoryButtonsGrid.GetComponent<GridLayoutGroup>();
		dragDropBuildingPanel.sizeDelta = layoutGroup.cellSize;
		SetupBuildingButtons(factoryData, factoryButtonsGrid, layoutGroup.constraintCount);
		SetupBuildingButtons(towersData, towersButtonsGrid, layoutGroup.constraintCount);
		Subscribe();
	}

	private void Subscribe() {
		factoryButton.onClick.AddListener(() => ShowTab(true));
		towersButton.onClick.AddListener(() => ShowTab(false));
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