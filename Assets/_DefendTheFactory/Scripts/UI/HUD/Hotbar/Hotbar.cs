using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour {
	[SerializeField] private GameObject buildingsMenu;
	[SerializeField] private CostResourceUI costResourcePrefab;
	[SerializeField] private RectTransform costPanel;
	[SerializeField] private List<HotbarBtn> hotbarBtns;

	private int btnsCount;

	private void Start() {
		btnsCount = hotbarBtns.Count;
		Subscribe();
	}

	private void OnDestroy() {
		Unsubscribe();
	}

	private void Subscribe() {
		Injector.Resolve<InputManager>().HotbarAction += OnHotbarAction;
		Injector.Resolve<BuildingSystem>().OnSystemDisabled += DisableHighlights;

		for (int i = 0; i < btnsCount; i++) {
			hotbarBtns[i].Init(this);
			hotbarBtns[i].OnBtnClick += OnBtnClick;
		}
	}

	private void Unsubscribe() {
		Injector.Resolve<InputManager>().HotbarAction -= OnHotbarAction;
		Injector.Resolve<BuildingSystem>().OnSystemDisabled -= DisableHighlights;

		for (int i = 0; i < btnsCount; i++) {
			hotbarBtns[i].OnBtnClick -= OnBtnClick;
		}
	}

	public bool IsBtnClickBlocked() {
		return buildingsMenu.activeSelf;
	}
	
	private void OnBtnClick(BaseBuildableObjectSO buildableObject) {
		DisableHighlights();
		SetupCostPanel(buildableObject);
	}

	private void DisableHighlights() {
		for (int i = 0; i < btnsCount; i++) {
			hotbarBtns[i].SetHighlight(false);
		}

		costPanel.gameObject.SetActive(false);
	}

	private void OnHotbarAction(int index) {
		DisableHighlights();
		hotbarBtns[index].ButtonAction();
	}

	private void SetupCostPanel(BaseBuildableObjectSO buildableObject) {
		if (buildableObject == null) return;

		foreach (RectTransform child in costPanel) {
			Destroy(child.gameObject);
		}

		int costItemsCounts = buildableObject.cost.Count;
		for (int i = 0; i < costItemsCounts; i++) {
			ItemIntPair itemCostPair = buildableObject.cost[i];
			Instantiate(costResourcePrefab, costPanel).Init(itemCostPair);
		}

		costPanel.gameObject.SetActive(true);
	}
}