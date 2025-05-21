using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Start Actions/Show HUD/Show Clock")]
public class ShowClock : QuestStartActionSO {
	public override void Execute(TutorialCanvas tutorial) {
		tutorial.hudClock.gameObject.SetActive(false);
	}
}