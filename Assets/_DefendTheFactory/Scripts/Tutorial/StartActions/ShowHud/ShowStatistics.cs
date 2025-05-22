using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Start Actions/Show HUD/Show Statistics")]
public class ShowStatistics : QuestStartActionSO {
	public override void Execute(TutorialCanvas tutorial) {
		tutorial.hudStatistics.gameObject.SetActive(true);
	}
}