using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Move Building To Hotbar")]
public class MoveBuildingToHotbarStepSO : QuestStepSO {
	[SerializeField] private BaseBuildableObjectSO buildableObjectToSet;

	private Hotbar hotbar;

	public override void Execute() {
		hotbar = FindFirstObjectByType<Hotbar>();

		for (int index = 0; index < hotbar.hotbarBtns.Count; index++) {
			HotbarBtn hotbarBtn = hotbar.hotbarBtns[index];
			hotbarBtn.OnOjbectChanged += OnObjectChanged;
		}
	}

	protected override void CompleteStep() {
		for (int index = 0; index < hotbar.hotbarBtns.Count; index++) {
			HotbarBtn hotbarBtn = hotbar.hotbarBtns[index];
			hotbarBtn.OnOjbectChanged += OnObjectChanged;
		}

		base.CompleteStep();
	}

	private void OnObjectChanged(BaseBuildableObjectSO newBuildableObject) {
		if (buildableObjectToSet != newBuildableObject) return;

		CompleteStep();
	}
}