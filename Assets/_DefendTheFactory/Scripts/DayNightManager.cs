using DG.Tweening;
using UnityEngine;

public class DayNightManager : MonoBehaviour {
	[SerializeField] private Light sun;
	[SerializeField] private Light moon;
	[SerializeField] private AnimationCurve lightIntensityCurve;
	[SerializeField] private float maxSunIntensity;
	[SerializeField] private float maxMoonIntensity;

	private Tween sunTween;
	private WaveManager waveManager;
	private float sunAngle;

	private void Start() {
		waveManager = Injector.Resolve<WaveManager>();
		Subscribe();
	}

	private void OnDestroy() {
		Unsubscribe();
	}

	private void Subscribe() {
		waveManager.OnDayTick += RotateSun;
	}

	private void Unsubscribe() {
		waveManager.OnDayTick -= RotateSun;
	}

	private void RotateSun(float percent) {
		if (sunTween != null && sunTween.IsActive()) sunTween.Kill();

		sunTween = DOTween.To(() => sun.transform.rotation.eulerAngles.x, x => sun.transform.rotation = Quaternion.AngleAxis(x, Vector3.right), Mathf.Lerp(0f, 180f, percent), 0.5f).SetEase(Ease.Linear);
	}

	private void UpdateLightSettings() {
		float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
		sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensityCurve.Evaluate(dotProduct));
	}
}