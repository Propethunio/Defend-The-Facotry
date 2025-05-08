using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Camera/Zoom Camera")]
public class ZoomCameraStepSO : QuestStepSO {
    private CameraFollowTarget cameraFollowTarget;

    public override void Execute() {
        cameraFollowTarget = Injector.Resolve<CameraFollowTarget>();
        cameraFollowTarget.cameraZoom += CompleteStep;
    }

    protected override void CompleteStep() {
        cameraFollowTarget.cameraZoom -= CompleteStep;
        base.CompleteStep();
    }
}