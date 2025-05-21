using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Start Actions/Show HUD/Show Hotbar")]
public class ShowHotbar : QuestStartActionSO {
	public override void Execute(TutorialCanvas tutorial) {
		tutorial.hudBuildingsMenu.EnableFromTutorial();
	}
}