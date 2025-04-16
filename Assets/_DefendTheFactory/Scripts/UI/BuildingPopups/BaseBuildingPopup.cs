using UnityEngine;

public abstract class BaseBuildingPopup : MonoBehaviour {
	protected int ticksPerSecond;
	protected int ticksPerMinute;

	private void Awake() {
		ticksPerSecond = Injector.Resolve<TimeTickSystem>().TicksPerSecond();
		ticksPerMinute = ticksPerSecond * 60;
	}

	public void Show(BasePlacedObject placedObject) {
		gameObject.SetActive(true);
		Setup(placedObject);
	}

	public void Close() {
		gameObject.SetActive(false);
		Unsubscribe();
	}

	private void Setup(BasePlacedObject placedObject) {
		SetupStaticData(placedObject);
		SetupDynamicData();
		Subscribe();
	}

	public void ChangeSelectedObject(BasePlacedObject placedObject) {
		Unsubscribe();
		Setup(placedObject);
	}

	protected abstract void SetupStaticData(BasePlacedObject placedObject);
	protected abstract void SetupDynamicData();
	protected abstract void Subscribe();
	protected abstract void Unsubscribe();

	protected string FormatFloatSmart(float value, int decimalPlaces = 1) {
		return Mathf.Approximately(value % 1f, 0f) ? ((int)value).ToString() : value.ToString($"F{decimalPlaces}");
	}
}