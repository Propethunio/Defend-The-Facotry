using UnityEngine;

[CreateAssetMenu(menuName = "Quests/UI Interaction/Open Building Popup")]
public class OpenBuildingPopupStepSO : QuestStepSO {
	[SerializeField] private BuildingPopupEnum buildingPopupToOpen;

	private BuildingPopupManager buildingPopupManager;

	public override void Execute() {
		buildingPopupManager = Injector.Resolve<BuildingPopupManager>();
		buildingPopupManager.OnBuildingPopupOpened += OnBuildingPopupOpened;
	}

	protected override void CompleteStep() {
		buildingPopupManager.OnBuildingPopupOpened -= OnBuildingPopupOpened;
		base.CompleteStep();
	}

	private void OnBuildingPopupOpened(BuildingPopupEnum openedPopup) {
		if (openedPopup != buildingPopupToOpen) return;

		CompleteStep();
	}
}