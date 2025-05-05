using UnityEngine;

public class LookAtCamera : MonoBehaviour {
	private Transform camTransform;

	private void Start() {
		camTransform = Camera.main.transform;
	}

	private void LateUpdate() {
		transform.forward = camTransform.forward;
	}
}