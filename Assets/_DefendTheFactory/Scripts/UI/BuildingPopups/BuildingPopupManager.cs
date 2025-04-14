using UnityEngine;

public class BuildingPopupManager : DependencyMonoBehaviour<BuildingPopupManager> {
    [SerializeField] private GatheringMachinePopup gatheringMachinePopup;

    private BaseBuildingPopup currentBuildingPopup;

    public void ShowBuildingPopup<T>(BaseDataPlacedObject<T> placedObject) where T : BaseBuildableObjectSO {
        if (currentBuildingPopup != null) {
            currentBuildingPopup.Close();
        }

        switch (placedObject.buildableDataSO.buildingPopupType) {
            default:
            case BuildingPopupEnum.None:
                currentBuildingPopup = null;
                return;
            case BuildingPopupEnum.GatheringMachine:
                currentBuildingPopup = gatheringMachinePopup;
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

        currentBuildingPopup.Setup(placedObject);
        currentBuildingPopup.Show();
    }
}