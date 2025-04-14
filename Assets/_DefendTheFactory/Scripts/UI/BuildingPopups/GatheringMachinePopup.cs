using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GatheringMachinePopup : BaseBuildingPopup {
    [SerializeField] private Image resourceIcon;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI machineNameText;
    [SerializeField] private TextMeshProUGUI resourceNameText;
    [SerializeField] private TextMeshProUGUI resourceRatioText;
    [SerializeField] private TextMeshProUGUI resourcesLeftText;
    [SerializeField] private TextMeshProUGUI progressPercentText;
    [SerializeField] private TextMeshProUGUI cycleLengthText;

    private GatheringMachine machine;
    private int ticksPerSecond;
    private int ticksPerMinute;

    private void Start() {
        ticksPerSecond = Injector.Resolve<TimeTickSystem>().TicksPerSecond();
        ticksPerMinute = ticksPerSecond * 60;
    }

    public override void Setup(BasePlacedObject placedObject) {
        machine = placedObject as GatheringMachine;
        SetupStaticData();
    }

    private void SetupStaticData() {
        machineNameText.text = machine.buildableDataSO.nameString;
        resourceIcon.sprite = machine.buildableDataSO.producedItem.icon;
        resourceNameText.text = machine.buildableDataSO.producedItem.name;
        resourceRatioText.text = ((float)ticksPerMinute / machine.buildableDataSO.ticksForGather).ToString("0.0");
        cycleLengthText.text = (machine.buildableDataSO.ticksForGather / ticksPerSecond).ToString("0.0");
    }
}