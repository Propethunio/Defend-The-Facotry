using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GatheringMachinePopup : BaseBuildingPopup {
    [SerializeField] private Image resourceIcon;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI machineNameText;
    [SerializeField] private TextMeshProUGUI resourceNameText;
    [SerializeField] private TextMeshProUGUI resourceAmountText;
    [SerializeField] private TextMeshProUGUI resourceRatioText;
    [SerializeField] private TextMeshProUGUI resourcesLeftText;
    [SerializeField] private TextMeshProUGUI progressPercentText;
    [SerializeField] private TextMeshProUGUI cycleLengthText;

    private GatheringMachine machine;
    private float targetProgress;
    private float interpolationSpeed;

    private void Update() {
        progressBar.value = Mathf.MoveTowards(progressBar.value, targetProgress, interpolationSpeed * Time.deltaTime);
        progressPercentText.text = (progressBar.value * 100).ToString("0") + "%";
    }

    protected override void SetupStaticData(BasePlacedObject placedObject) {
        machine = placedObject as GatheringMachine;
        float cycleTime = (float)machine.buildableDataSO.ticksForGather / ticksPerSecond;
        interpolationSpeed = 1f / (cycleTime * 0.9f);
        resourceIcon.sprite = machine.buildableDataSO.producedItem.icon;
        machineNameText.text = machine.buildableDataSO.nameString;
        resourceNameText.text = machine.buildableDataSO.producedItem.name;
        resourceRatioText.text = FormatFloatSmart((float)ticksPerMinute / machine.buildableDataSO.ticksForGather) + "/min";
        cycleLengthText.text = FormatFloatSmart(cycleTime) + "s";
    }

    protected override void SetupDynamicData() {
        resourceAmountText.text = machine.storedItemsCount.ToString();
        resourcesLeftText.text = "Total items left: " + machine.GetResourcesInRangeAmount();
        progressBar.value = machine.GetProgressNormalized();
        progressPercentText.text = (progressBar.value * 100).ToString("0") + "%";
        targetProgress = machine.GetTargetProgressNormalized();
    }

    protected override void Subscribe() {
        machine.StoredItemsCountChanged += UpdateStoredItems;
        machine.ResourcesInRangeChanged += UpdateResourcesLeft;
        machine.ProductionTicksChanged += UpdateProgress;
    }

    protected override void Unsubscribe() {
        machine.StoredItemsCountChanged -= UpdateStoredItems;
        machine.ResourcesInRangeChanged -= UpdateResourcesLeft;
        machine.ProductionTicksChanged -= UpdateProgress;
    }

    private void UpdateStoredItems(int amount) {
        resourceAmountText.text = amount.ToString();
    }

    private void UpdateResourcesLeft(int amount) {
        resourcesLeftText.text = "Total items left: " + amount;
    }

    private void UpdateProgress(float progress) {
        if (progress < targetProgress) {
            progressBar.value = 0f;
        }

        targetProgress = progress;
    }
}