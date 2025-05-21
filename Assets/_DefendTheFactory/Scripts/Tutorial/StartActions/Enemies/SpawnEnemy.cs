using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Start Actions/Enemies/Spawn Enemy")]
public class SpawnEnemy : QuestStartActionSO {
	public override void Execute(TutorialCanvas tutorial) {
		Injector.Resolve<WaveManager>().SpawnTutorialEnemy();
	}
}