using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotbarBtn : MonoBehaviour, IDropHandler {
	[SerializeField] private Image highlight;
	[SerializeField] private Image icon;
	[SerializeField] private BaseBuildableObjectSO defaultObject;

	private Hotbar hotbar;
	private BaseBuildableObjectSO buildableObject;
	private BuildingSystem buildingSystem;
	
	public event Action<BaseBuildableObjectSO> OnBtnClick, OnOjbectChanged;

	private void Start() {
		buildingSystem = Injector.Resolve<BuildingSystem>();
		GetComponent<Button>().onClick.AddListener(ButtonAction);

		if (defaultObject == null) return;
		ChangeObject(defaultObject);
	}

	public void Init(Hotbar hotbar) {
		this.hotbar = hotbar;
	}

	public void OnDrop(PointerEventData eventData) {
		ChangeObject(eventData.pointerDrag.GetComponent<BuildingBtn>().buildableObject);
	}

	public void ButtonAction() {
		if (hotbar.IsBtnClickBlocked()) return;

		OnBtnClick?.Invoke(buildableObject);

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

	private void ChangeObject(BaseBuildableObjectSO obj) {
		buildableObject = obj;
		icon.sprite = buildableObject.icon;
		icon.enabled = true;
		OnOjbectChanged?.Invoke(buildableObject);
	}
}