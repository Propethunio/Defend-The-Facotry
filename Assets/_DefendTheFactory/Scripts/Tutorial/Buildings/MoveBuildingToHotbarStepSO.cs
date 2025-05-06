using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Move Building To Hotbar")]
public class MoveBuildingToHotbarStepSO : QuestStepSO {
	[SerializeField] private BaseBuildableObjectSO buildableObjectToSet;

	private List<HotbarBtn> hotbar;
	private int btnCount;

	public override void Execute() {
		hotbar = FindFirstObjectByType<Hotbar>().GetHotbarBtns();
		btnCount = hotbar.Count;
		
		for (int index = 0; index < btnCount; index++) {
			HotbarBtn hotbarBtn = hotbar[index];
			hotbarBtn.OnOjbectChanged += OnObjectChanged;
		}
	}

	protected override void CompleteStep() {
		for (int index = 0; index < btnCount; index++) {
			HotbarBtn hotbarBtn = hotbar[index];
			hotbarBtn.OnOjbectChanged -= OnObjectChanged;
		}

		base.CompleteStep();
	}

	private void OnObjectChanged(BaseBuildableObjectSO newBuildableObject) {
		if (buildableObjectToSet != newBuildableObject) return;

		CompleteStep();
	}
}