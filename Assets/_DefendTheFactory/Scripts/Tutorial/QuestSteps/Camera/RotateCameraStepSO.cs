using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Camera/Rotate Camera")]
public class RotateCameraStepSO : QuestStepSO {
    private CameraFollowTarget cameraFollowTarget;

    public override void Execute() {
        cameraFollowTarget = Injector.Resolve<CameraFollowTarget>();
        cameraFollowTarget.cameraRotate += CompleteStep;
    }

    protected override void CompleteStep() {
        cameraFollowTarget.cameraRotate -= CompleteStep;
        base.CompleteStep();
    }
}