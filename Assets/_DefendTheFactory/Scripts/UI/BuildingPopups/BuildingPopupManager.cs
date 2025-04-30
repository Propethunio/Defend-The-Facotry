using UnityEngine;

public class BuildingPopupManager : DependencyMonoBehaviour<BuildingPopupManager> {
	[SerializeField] private GatheringMachinePopup gatheringMachinePopup;
	[SerializeField] private ConstructorPopup constructorPopup;

	private BaseBuildingPopup currentBuildingPopup;
	private BasePlacedObject currentSelectedObject;
	private InputManager inputManager;

	private void Start() {
		inputManager = Injector.Resolve<InputManager>();
	}

	private void OnDestroy() {
		inputManager.RightClickPerformedAction -= CloseActivePopup;
	}

	private void CloseActivePopup() {
		if (currentBuildingPopup != null) {
			currentBuildingPopup.Close();
			currentBuildingPopup = null;
		}

		currentSelectedObject = null;
		inputManager.UnregisterBackAction(CloseActivePopup);
		inputManager.RightClickPerformedAction -= CloseActivePopup;
	}

	public void ShowBuildingPopup<T>(BaseDataPlacedObject<T> placedObject) where T : BaseBuildableObjectSO {
		if (currentSelectedObject == placedObject) return;

		currentSelectedObject = placedObject;
		BaseBuildingPopup popupToShow = GetPopupType(placedObject.buildableDataSO.buildingPopupType);

		if (popupToShow == null) {
			CloseActivePopup();
			return;
		}

		if (popupToShow == currentBuildingPopup) {
			currentBuildingPopup.ChangeSelectedObject(placedObject);
			return;
		}

		if (currentBuildingPopup != null) {
			currentBuildingPopup.Close();
		}
		else {
			inputManager.RegisterBackAction(CloseActivePopup);
			inputManager.RightClickPerformedAction += CloseActivePopup;
		}
		
		currentBuildingPopup = popupToShow;
		currentBuildingPopup.Show(placedObject);
	}

	private BaseBuildingPopup GetPopupType(BuildingPopupEnum buildingPopupType) {
		switch (buildingPopupType) {
			default:
			case BuildingPopupEnum.None: return null;
			case BuildingPopupEnum.GatheringMachine: return gatheringMachinePopup;
			case BuildingPopupEnum.Constructor: return constructorPopup;
		}
	}
}