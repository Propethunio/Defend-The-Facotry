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
	[SerializeField] private Color dayAmbientLight;
	[SerializeField] private Color nightAmbientLight;
	[SerializeField] private Volume globalVolume;
	[SerializeField] private Material skyboxMaterial;

	private ColorAdjustments colorAdjustments;
	private Tween sunTween;
	private WaveManager waveManager;
	private float sunAngle;
	private float valueOnCurve;

	private static readonly int _blend = Shader.PropertyToID("_Blend");

	private void Start() {
		waveManager = Injector.Resolve<WaveManager>();
		globalVolume.profile.TryGet(out colorAdjustments);
		Subscribe();
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
		Debug.Log(percent);
		if (sunTween != null && sunTween.IsActive()) sunTween.Kill();

		sunTween = DOTween.To(() => sunAngle, x => {
			sunAngle = x;
			sunPivot.localRotation = Quaternion.Euler(sunAngle, 0, 23.5f);
		}, Mathf.Lerp(0f, 180f, percent), 0.5f).SetEase(Ease.Linear);
	}

	private void EvaluatePointOnCurve() {
		valueOnCurve = lightIntensityCurve.Evaluate(Vector3.Dot(sun.transform.forward, Vector3.down));
	}

	private void UpdateLightSettings() {
		sun.intensity = Mathf.Lerp(0, maxSunIntensity, valueOnCurve);
		moon.intensity = Mathf.Lerp(0, maxMoonIntensity, valueOnCurve);
		colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, valueOnCurve);
	}

	private void UpdateSkybox() {
		skyboxMaterial.SetFloat(_blend, Mathf.Lerp(0, 1, valueOnCurve));
	}
}