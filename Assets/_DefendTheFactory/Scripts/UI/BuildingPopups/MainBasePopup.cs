using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainBasePopup : BaseBuildingPopup {
	[SerializeField] private Image Res1Image;
	[SerializeField] private Image Res2Image;
	[SerializeField] private Image Res3Image;
	[SerializeField] private TMP_Text Res1NameText;
	[SerializeField] private TMP_Text Res2NameText;
	[SerializeField] private TMP_Text Res3NameText;
	[SerializeField] private TMP_Text Res1AmountText;
	[SerializeField] private TMP_Text Res2AmountText;
	[SerializeField] private TMP_Text Res3AmountText;
	[SerializeField] private Button UpgradeBtn;

	private MainBase machine;

	protected override void SetupStaticData(BasePlacedObject placedObject) {
		machine = placedObject as MainBase;
		Res1Image.sprite = machine.buildableDataSO.UpgradesList[machine.level].cost[0].item.icon;
		Res2Image.sprite = machine.buildableDataSO.UpgradesList[machine.level].cost[1].item.icon;
		Res3Image.sprite = machine.buildableDataSO.UpgradesList[machine.level].cost[2].item.icon;
		Res1NameText.text = machine.buildableDataSO.UpgradesList[machine.level].cost[0].item.name;
		Res2NameText.text = machine.buildableDataSO.UpgradesList[machine.level].cost[1].item.name;
		Res3NameText.text = machine.buildableDataSO.UpgradesList[machine.level].cost[2].item.name;
	}

	protected override void SetupDynamicData() {
		Res1AmountText.text = machine.buildableDataSO.UpgradesList[machine.level].cost[0].amount.ToString();
		Res2AmountText.text = machine.buildableDataSO.UpgradesList[machine.level].cost[1].amount.ToString();
		Res3AmountText.text = machine.buildableDataSO.UpgradesList[machine.level].cost[2].amount.ToString();
	}

	protected override void Subscribe() {
		UpgradeBtn.onClick.AddListener(UpgradeBtnClicked);
	}

	protected override void Unsubscribe() {
		UpgradeBtn.onClick.RemoveAllListeners();
	}

	private void UpgradeBtnClicked() {
		machine.UpgradeLevel();
		ChangeSelectedObject(machine);
	}
}