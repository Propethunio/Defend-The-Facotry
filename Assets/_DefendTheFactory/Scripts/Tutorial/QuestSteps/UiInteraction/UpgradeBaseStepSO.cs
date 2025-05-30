using UnityEngine;

[CreateAssetMenu(menuName = "Quests/UI Interaction/Upgrade Base")]
public class UpgradeBaseStepSO : QuestStepSO {
    
    private MainBasePopup popup;

    public override void Execute() {
        popup = FindFirstObjectByType<MainBasePopup>(FindObjectsInactive.Include);
        popup.OnUpgrade += CompleteStep;
    }

    protected override void CompleteStep() {
        popup.OnUpgrade -= CompleteStep;
        base.CompleteStep();
    }
}