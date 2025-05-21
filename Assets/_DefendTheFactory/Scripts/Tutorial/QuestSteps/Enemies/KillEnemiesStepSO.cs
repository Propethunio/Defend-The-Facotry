using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Enemies/Kill Enemies")]
public class KillEnemiesStepSO : QuestStepSO {
	[SerializeField] private int targetCount;

	private WaveManager waveManager;
	private int currentCount;

	public override void Execute() {
		currentCount = 0;
		waveManager = Injector.Resolve<WaveManager>();
		waveManager.OnEnemyDeath += OnEnemyDeath;
	}

	protected override void CompleteStep() {
		waveManager.OnEnemyDeath -= OnEnemyDeath;
		base.CompleteStep();
	}

	private void OnEnemyDeath(EnemyLogic enemy) {
		currentCount++;
		if (currentCount == targetCount) {
			CompleteStep();
		}
	}
}