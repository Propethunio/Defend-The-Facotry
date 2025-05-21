using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Start Actions/Show HUD/Enable Towers")]
public class EnableTowers : QuestStartActionSO {
	public override void Execute(TutorialCanvas tutorial) {
		tutorial.hudBuildingsMenu.EnableTowersMenu();
	}
}