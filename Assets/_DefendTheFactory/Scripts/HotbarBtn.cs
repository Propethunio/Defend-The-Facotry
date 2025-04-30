using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotbarBtn : MonoBehaviour, IDropHandler {
	[SerializeField] private Image highlight;
	[SerializeField] private Image icon;
	[SerializeField] private BaseBuildableObjectSO defaultObject;

	private BaseBuildableObjectSO buildableObject;
	private BuildingSystem buildingSystem;

	public event Action<BaseBuildableObjectSO> OnBtnClick;

	private void Start() {
		buildingSystem = Injector.Resolve<BuildingSystem>();
		GetComponent<Button>().onClick.AddListener(ButtonAction);
		ChangeObject(defaultObject);
	}

	public void OnDrop(PointerEventData eventData) { 
		ChangeObject(eventData.pointerDrag.GetComponent<BuildingBtn>().buildableObject);
	}

	public void ButtonAction() {
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
	}
}