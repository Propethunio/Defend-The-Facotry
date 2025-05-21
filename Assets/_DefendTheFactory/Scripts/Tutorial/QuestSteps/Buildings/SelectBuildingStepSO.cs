using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Buildings/Select Building")]
public class SelectBuildingStepSO : QuestStepSO {
    [SerializeField] private BaseBuildableObjectSO buildingToSelect;
    
    private BuildingSystem buildingSystem;

    public override void Execute() {
        buildingSystem = Injector.Resolve<BuildingSystem>();
        buildingSystem.OnObjectSelected += OnBuildingSelected;
    }

    protected override void CompleteStep() {
        buildingSystem.OnObjectSelected -= OnBuildingSelected;
        base.CompleteStep();
    }

    private void OnBuildingSelected(BaseBuildableObjectSO selected) {
        if(selected != buildingToSelect) return;
        
        CompleteStep();
    }
}