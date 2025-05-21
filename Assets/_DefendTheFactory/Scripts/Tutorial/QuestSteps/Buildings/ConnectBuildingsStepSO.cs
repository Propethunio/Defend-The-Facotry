using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Buildings/Connect Buildings")]
public class ConnectBuildingsStepSO : QuestStepSO {
    [SerializeField] private BaseBuildableObjectSO buildingOne;
    [SerializeField] private BaseBuildableObjectSO buildingTwo;

    private BeltManager beltManager;

    public override void Execute() {
        beltManager = Injector.Resolve<BeltManager>();
        beltManager.OnBeltAdded += CheckIfBuildingsConnected;
        beltManager.OnBeltRemoved += CheckIfBuildingsConnected;
        CheckIfBuildingsConnected();
    }

    protected override void CompleteStep() {
        beltManager.OnBeltAdded -= CheckIfBuildingsConnected;
        beltManager.OnBeltRemoved -= CheckIfBuildingsConnected;
        base.CompleteStep();
    }

    private void CheckIfBuildingsConnected() {
        if(!beltManager.AreBuildingsConnected(buildingOne, buildingTwo)) return;
        
        CompleteStep();
    }
}