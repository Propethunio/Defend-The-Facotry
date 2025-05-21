using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayNightManager : MonoBehaviour {
	[SerializeField] private Transform sunPivot;
	[SerializeField] private Light sun;
	[SerializeField] private Light moon;
	[SerializeField] private AnimationCurve lightIntensityCurve;
	[SerializeField] private float maxSunIntensity;
	[SerializeField] private float maxMoonIntensity;
	[SerializeField] private float maxShadowStrength;
	[SerializeField] private Color dayAmbientLight;
	[SerializeField] private Color nightAmbientLight;
	[SerializeField] private Volume globalVolume;
	[SerializeField] private Material skyboxMaterial;

	private ColorAdjustments colorAdjustments;
	private WaveManager waveManager;
	private float valueOnCurve;
	private float sunX;
	private float sunY;

	private static readonly int _blend = Shader.PropertyToID("_Blend");

	private void Start() {
		waveManager = Injector.Resolve<WaveManager>();
		globalVolume.profile.TryGet(out colorAdjustments);
		Subscribe();
		
		if (GameSetupData.Instance.IsTutorialLevel()) {
			RotateSun(.7f);
		}
	}

	private void Update() {
		EvaluatePointOnCurve();
		UpdateLightSettings();
		UpdateSkybox();
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
		float targetX = Mathf.Lerp(0f, 180f, percent);
		float targetY = Mathf.Sin(percent * Mathf.PI) * 23.5f;
		DOTween.Kill("SunX");
		DOTween.Kill("SunY");

		DOVirtual.Float(sunX, targetX, 0.5f, x => {
			sunX = x;
			sunPivot.localRotation = Quaternion.Euler(sunX, sunY, 0);
		}).SetEase(Ease.Linear).SetId("SunX");

		DOVirtual.Float(sunY, targetY, 0.5f, y => {
			sunY = y;
			sunPivot.localRotation = Quaternion.Euler(sunX, sunY, 0);
		}).SetEase(Ease.Linear).SetId("SunY");
	}

	private void EvaluatePointOnCurve() {
		valueOnCurve = lightIntensityCurve.Evaluate(Vector3.Dot(sun.transform.forward, Vector3.down));
	}

	private void UpdateLightSettings() {
		sun.intensity = Mathf.Lerp(0, maxSunIntensity, valueOnCurve);
		moon.intensity = Mathf.Lerp(0, maxMoonIntensity, 1 - valueOnCurve);
		colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, valueOnCurve);
		sun.shadowStrength = Mathf.Lerp(0, maxShadowStrength, valueOnCurve);
	}

	private void UpdateSkybox() {
		skyboxMaterial.SetFloat(_blend, Mathf.Lerp(0, 1, valueOnCurve));
	}
}