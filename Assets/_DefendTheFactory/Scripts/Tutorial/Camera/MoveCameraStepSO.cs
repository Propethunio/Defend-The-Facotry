using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Move Camera")]
public class MoveCameraStepSO : QuestStepSO {
	private CameraFollowTarget cameraFollowTarget;

	public override void Execute() {
		cameraFollowTarget = Injector.Resolve<CameraFollowTarget>();
		cameraFollowTarget.cameraMove += CompleteStep;
	}

	protected override void CompleteStep() {
		cameraFollowTarget.cameraMove -= CompleteStep;
		base.CompleteStep();
	}
}