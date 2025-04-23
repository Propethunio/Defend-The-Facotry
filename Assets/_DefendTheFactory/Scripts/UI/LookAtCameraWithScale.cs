using UnityEngine;

public class LookAtCameraWithScale : MonoBehaviour {
	[SerializeField, Range(1, 5)] private float scaleFactor;

	private Transform camTransform;
	private float lowPoint;
	private float maxDistance;

	private void Start() {
		camTransform = Camera.main.transform;
		Vector2 zoomRange = Injector.Resolve<CameraFollowTarget>().GetZoomRange();
		lowPoint = zoomRange.x;
		maxDistance = zoomRange.y;
	}

	private void LateUpdate() {
		ScaleBasedOnCameraY();
		LookAtCamera();
	}

	private void ScaleBasedOnCameraY() {
		float camPosY = camTransform.position.y;
		float t = Mathf.InverseLerp(lowPoint, maxDistance, camPosY);
		float scaleMultiplier = Mathf.Lerp(1f, scaleFactor, t);
		transform.localScale = Vector3.one * scaleMultiplier;
	}

	private void LookAtCamera() {
		transform.forward = camTransform.forward;
	}
}