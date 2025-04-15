using UnityEngine;

public class BuildingPopupManager : DependencyMonoBehaviour<BuildingPopupManager> {
    [SerializeField] private GatheringMachinePopup gatheringMachinePopup;

    private BaseBuildingPopup currentBuildingPopup;
    private BasePlacedObject currentSelectedObject;

    private void Start() {
        Injector.Resolve<InputManager>().rightClickPerformedAction += CloseActivePopup;
    }

    private void OnDestroy() {
        Injector.Resolve<InputManager>().rightClickPerformedAction -= CloseActivePopup;
    }

    private void CloseActivePopup() {
        if (currentBuildingPopup == null) return;

        currentBuildingPopup.Close();
        currentBuildingPopup = null;
        currentSelectedObject = null;
    }

    public void ShowBuildingPopup<T>(BaseDataPlacedObject<T> placedObject) where T : BaseBuildableObjectSO {
        if (currentSelectedObject == placedObject) return;

        currentSelectedObject = placedObject;
        BaseBuildingPopup popupToShow = null;

        switch (placedObject.buildableDataSO.buildingPopupType) {
            default:
            case BuildingPopupEnum.None:
                currentBuildingPopup.Close();
                currentBuildingPopup = null;
                return;
            case BuildingPopupEnum.GatheringMachine:
                popupToShow = gatheringMachinePopup;
                break;
            case BuildingPopupEnum.Constructor:
                break;
            case BuildingPopupEnum.MainBase:
                break;
            case BuildingPopupEnum.TowerTargetPicking:
                break;
            case BuildingPopupEnum.TowerAOE:
                break;
        }

        if (popupToShow == currentBuildingPopup) {
            currentBuildingPopup.ChangeSelectedObject(placedObject);
            return;
        }

        if (currentBuildingPopup != null) {
            currentBuildingPopup.Close();
        }

        currentBuildingPopup = popupToShow;
        currentBuildingPopup.Show(placedObject);
    }
}