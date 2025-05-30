using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssemblerPopup : BaseBuildingPopup {
	[SerializeField] private Image recipeOneIcon;
	[SerializeField] private Image recipeOneHighlight;
	[SerializeField] private Image recipeTwoIcon;
	[SerializeField] private Image recipeTwoHighlight;
	[SerializeField] private Image inputIcon;
	[SerializeField] private Image input2Icon;
	[SerializeField] private Image outputIcon;
	[SerializeField] private GameObject recipeTwoLock;
	[SerializeField] private Slider progressBar;
	[SerializeField] private Button recipeOneButton;
	[SerializeField] private Button recipeTwoButton;
	[SerializeField] private TextMeshProUGUI machineNameText;
	[SerializeField] private TextMeshProUGUI inputResourceText;
	[SerializeField] private TextMeshProUGUI input2ResourceText;
	[SerializeField] private TextMeshProUGUI inputResourceAmountText;
	[SerializeField] private TextMeshProUGUI input2ResourceAmountText;
	[SerializeField] private TextMeshProUGUI inputResourceRatioText;
	[SerializeField] private TextMeshProUGUI input2ResourceRatioText;
	[SerializeField] private TextMeshProUGUI outputResourceText;
	[SerializeField] private TextMeshProUGUI outputResourceAmountText;
	[SerializeField] private TextMeshProUGUI outputResourceRatioText;
	[SerializeField] private TextMeshProUGUI progressPercentText;
	[SerializeField] private TextMeshProUGUI cycleLengthText;

    private Assembler machine;
	private float targetProgress;
	private float interpolationSpeed;
	private int currentRecipeIndex;

	private void Update() {
		progressBar.value = Mathf.MoveTowards(progressBar.value, targetProgress, interpolationSpeed * Time.deltaTime);
		progressPercentText.text = (progressBar.value * 100).ToString("0") + "%";
	}

	protected override void SetupStaticData(BasePlacedObject placedObject) {
		machine = placedObject as Assembler;
		float cycleTime = (float)machine.currentRecipe.craftingTicks / ticksPerSecond;
		interpolationSpeed = 1f / (cycleTime * 0.9f);
		recipeOneIcon.sprite = machine.buildableDataSO.itemRecipeList[0].outputItemList[0].item.icon;
		recipeTwoIcon.sprite = machine.buildableDataSO.itemRecipeList[1].outputItemList[0].item.icon;
		inputIcon.sprite = machine.currentRecipe.inputItemList[0].item.icon;
		input2Icon.sprite = machine.currentRecipe.inputItemList[1].item.icon;
		outputIcon.sprite = machine.currentRecipe.outputItemList[0].item.icon;

		if (machine.currentRecipe == machine.buildableDataSO.itemRecipeList[0]) {
			recipeOneHighlight.enabled = true;
			recipeTwoHighlight.enabled = false;
            currentRecipeIndex = 0;
        }
		else {
			recipeOneHighlight.enabled = false;
			recipeTwoHighlight.enabled = true;
			currentRecipeIndex = 1;
		}

		machineNameText.text = machine.buildableDataSO.nameString;
		inputResourceText.text = machine.currentRecipe.inputItemList[0].amount + " x " + machine.currentRecipe.inputItemList[0].item.name;
		input2ResourceText.text = machine.currentRecipe.inputItemList[1].amount + " x " + machine.currentRecipe.inputItemList[1].item.name;
		outputResourceText.text = machine.currentRecipe.outputItemList[0].amount + " x " + machine.currentRecipe.outputItemList[0].item.name;
		inputResourceRatioText.text = FormatFloatSmart((float)ticksPerMinute / machine.currentRecipe.craftingTicks * machine.currentRecipe.inputItemList[0].amount) + "/min";
		inputResourceRatioText.text = FormatFloatSmart((float)ticksPerMinute / machine.currentRecipe.craftingTicks * machine.currentRecipe.inputItemList[1].amount) + "/min";
		outputResourceRatioText.text = FormatFloatSmart((float)ticksPerMinute / machine.currentRecipe.craftingTicks * machine.currentRecipe.outputItemList[0].amount) + "/min";
		cycleLengthText.text = FormatFloatSmart(cycleTime) + "s";
	}

	protected override void SetupDynamicData() {
		inputResourceAmountText.text = machine.storedInputItems[machine.currentRecipe.inputItemList[0].item].ToString();
		input2ResourceAmountText.text = machine.storedInputItems[machine.currentRecipe.inputItemList[1].item].ToString();
		outputResourceAmountText.text = machine.storedOutputItems.ToString();
		progressBar.value = machine.GetProgressNormalized();
		progressPercentText.text = (progressBar.value * 100).ToString("0") + "%";
		targetProgress = machine.GetTargetProgressNormalized();
	}

	protected override void Subscribe() {
		machine.StoredInputItemsCountChanged += UpdateStoredInputItems;
		machine.StoredOutputItemsCountChanged += UpdateStoredOutputItems;
		machine.ProductionTicksChanged += UpdateProgress;
		machine.BuildingUpgraded += UnlockRecipe;
		recipeOneButton.onClick.AddListener(() => ChangeMachineRecipe(0));
		if(machine.buildingLevel == 2) {
            recipeTwoButton.onClick.AddListener(() => ChangeMachineRecipe(1));
            recipeTwoLock.SetActive(false);
        } else {
			recipeTwoLock.SetActive(true);
		}
	}

	protected override void Unsubscribe() {
		machine.StoredInputItemsCountChanged -= UpdateStoredInputItems;
		machine.StoredOutputItemsCountChanged -= UpdateStoredOutputItems;
		machine.ProductionTicksChanged -= UpdateProgress;
        machine.BuildingUpgraded -= UnlockRecipe;
        recipeOneButton.onClick.RemoveAllListeners();
		recipeTwoButton.onClick.RemoveAllListeners();
	}

	private void ChangeMachineRecipe(int recipeIndex) {
		if(currentRecipeIndex == recipeIndex) return;
		
		machine.SetupRecipe(recipeIndex);
		SetupStaticData(machine);
		SetupDynamicData();
	}
	
	private void UpdateStoredInputItems(int amount) {
		inputResourceAmountText.text = machine.storedInputItems[machine.currentRecipe.inputItemList[0].item].ToString();
		input2ResourceAmountText.text = machine.storedInputItems[machine.currentRecipe.inputItemList[1].item].ToString();
	}

	private void UpdateStoredOutputItems(int amount) {
		outputResourceAmountText.text = machine.storedOutputItems.ToString();
	}

	private void UpdateProgress(float progress) {
		if (progress < targetProgress) {
			progressBar.value = 0f;
		}

		targetProgress = progress;
	}

	private void UnlockRecipe() {
        recipeTwoButton.onClick.AddListener(() => ChangeMachineRecipe(1));
        recipeTwoLock.SetActive(false);
    }
}