using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Buildings/Rotate Building")]
public class RotateBuildingStepSO : QuestStepSO {
    private BuildingSystem buildingSystem;

    public override void Execute() {
        buildingSystem = Injector.Resolve<BuildingSystem>();
        buildingSystem.OnRotateObject += CompleteStep;
    }

    protected override void CompleteStep() {
        buildingSystem.OnRotateObject -= CompleteStep;
        base.CompleteStep();
    }
}