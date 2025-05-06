using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Open Building Menu")]
public class OpenBuildingMenuStepSO : QuestStepSO {
	private InputManager inputManager;

	public override void Execute() {
		inputManager = Injector.Resolve<InputManager>();
		inputManager.BuildingMenuAction += CompleteStep;
	}

	protected override void CompleteStep() {
		inputManager.BuildingMenuAction -= CompleteStep;
		base.CompleteStep();
	}
}