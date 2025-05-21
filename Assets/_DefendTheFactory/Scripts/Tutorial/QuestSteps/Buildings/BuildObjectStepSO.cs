using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Buildings/Build Object")]
public class BuildObjectStepSO : QuestStepSO {
	[SerializeField] private BaseBuildableObjectSO objectToBuild;
	[SerializeField] private int count;

	private int currentCount;
	private BuildingSystem buildingSystem;

	public override void Execute() {
		currentCount = 0;
		buildingSystem = Injector.Resolve<BuildingSystem>();
		buildingSystem.OnObjectPlaced += OnObjectPlaced;
	}

	protected override void CompleteStep() {
		buildingSystem.OnObjectPlaced -= OnObjectPlaced;
		base.CompleteStep();
	}

	private void OnObjectPlaced(BaseBuildableObjectSO obj) {
		if (objectToBuild != obj) return;

		currentCount++;
		if (currentCount == count) {
			CompleteStep();
		}
	}
}