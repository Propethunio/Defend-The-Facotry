using System;
using UnityEngine;
using UnityEngine.UI;

public class HotbarBtn : MonoBehaviour {
	[SerializeField] private Image highlight;
	[SerializeField] private Image icon;
	[SerializeField] private BaseBuildableObjectSO defaultObject;

	private BaseBuildableObjectSO buildableObject;
	private BuildingSystem buildingSystem;

	public event Action OnBtnClick;

	private void Start() {
		buildingSystem = Injector.Resolve<BuildingSystem>();
		GetComponent<Button>().onClick.AddListener(OnClick);
		ChangeObject(defaultObject);
	}

	private void OnClick() {
		OnBtnClick?.Invoke();
		ButtonAction();
	}

	public void ButtonAction() {
		if (buildableObject == null) {
			buildingSystem.DisableBuildingSystem();
			return;
		}

		SetHighlight(true);
		buildingSystem.SetSelectedPlacedObject(buildableObject);
	}

	public void SetHighlight(bool isActive) {
		highlight.enabled = isActive;
	}

	public void ChangeObject(BaseBuildableObjectSO obj) {
		buildableObject = obj;
		icon.sprite = buildableObject.icon;
		icon.enabled = true;
	}
}