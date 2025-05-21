using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour {
	[SerializeField] private RectTransform clockContainer;
	[SerializeField] private RectTransform waveIcon;
	[SerializeField] private RectTransform dayNightSlide;
	[SerializeField] private float skullOffset;
	[SerializeField] private Image progressBar;
	[SerializeField] private TMP_Text progressText;
	[SerializeField] private List<ClockGear> gears;

	private WaveManager waveManager;
	private float dayNightSlideSize;
	private float dayNightSlideX;

	private void Start() {
		waveManager = Injector.Resolve<WaveManager>();
		dayNightSlideSize = dayNightSlide.sizeDelta.y;
		dayNightSlideX = dayNightSlide.anchoredPosition.x;
		
		if (GameSetupData.Instance.IsTutorialLevel()) {
			StartTutorial();
		}
		else {
			Subscribe();	
		}
	}

	private void OnDestroy() {
		Unsubscribe();
	}

	private void Subscribe() {
		waveManager.OnDayStart += OnDayStart;
		waveManager.OnDayTick += SetDayTimeProgress;
	}

	private void Unsubscribe() {
		waveManager.OnDayStart -= OnDayStart;
		waveManager.OnDayTick -= SetDayTimeProgress;
	}

	private void StartTutorial() {
		SetDayTimeProgress(.7f);
		gameObject.SetActive(false);
	}
	
	private void SetDayTimeProgress(float progress) {
		progressBar.fillAmount = progress;
		dayNightSlide.anchoredPosition = new Vector2(dayNightSlideX, progress * dayNightSlideSize);
		UpdateGears(progress);
	}

	private void UpdateGears(float timeRatio) {
		int totalGears = gears.Count;
		int activeGears = Mathf.CeilToInt(timeRatio * totalGears);

		for (int i = 0; i < totalGears; i++) {
			gears[i].ToggleHighlight(i < activeGears);
		}
	}

	private void OnDayStart() {
		CalculateWaveIconPosition();
		SetCurrentDay();
	}

	private void CalculateWaveIconPosition() {
		float timeRatio = (float)waveManager.GetSpawnStartTime() / waveManager.GetDayLength();
		float angleRadians = (timeRatio * -360f + 90f) * Mathf.Deg2Rad;
		float radius = clockContainer.rect.width / 2 + skullOffset;
		waveIcon.position = new Vector2(clockContainer.position.x + radius * Mathf.Cos(angleRadians), clockContainer.position.y + radius * Mathf.Sin(angleRadians));
	}

	private void SetCurrentDay() {
		progressText.text = (waveManager.GetDaysSurvived() + 1).ToString();
	}
}