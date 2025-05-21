using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Start Actions/Enemies/Start Wave Ticking")]
public class StartWaveTicking : QuestStartActionSO {
	public override void Execute(TutorialCanvas tutorial) {
		Injector.Resolve<WaveManager>().StartTickingTutorial();
	}
}