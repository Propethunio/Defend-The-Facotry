using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Resources/Collect Resource")]
public class CollectResourceStepSO : QuestStepSO {
	[SerializeField] private ItemSO itemToCollect;
	[SerializeField] private int targetCount;

	private ItemsManager itemsManager;
	private int currentCount;

	public override void Execute() {
		currentCount = 0;
		itemsManager = Injector.Resolve<ItemsManager>();
		itemsManager.ItemAdded += OnItemAdded;
	}

	protected override void CompleteStep() {
		itemsManager.ItemAdded -= OnItemAdded;
		base.CompleteStep();
	}

	private void OnItemAdded(ItemSO addedItem, int count) {
		if (addedItem != itemToCollect) return;

		currentCount += count;
		if (currentCount >= targetCount) {
			CompleteStep();
		}
	}
}