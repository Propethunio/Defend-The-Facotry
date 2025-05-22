using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Enemies/Survive Night")]
public class SurviveNightSO : QuestStepSO {
	private WaveManager waveManager;

	public override void Execute() {
		waveManager = Injector.Resolve<WaveManager>();
		waveManager.OnDayStart += CompleteStep;
	}

	protected override void CompleteStep() {
		waveManager.OnDayStart -= CompleteStep;
		base.CompleteStep();
	}
}