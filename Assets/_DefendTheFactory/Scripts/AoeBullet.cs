using UnityEngine;

public class AoeBullet : MonoBehaviour {
	private Vector3 moveDirection;
	private float speed;
	private float distanceTraveled;
	private const float maxDistance = 1f;

	public void Initialize(Vector3 direction, float moveSpeed) {
		moveDirection = direction.normalized;
		speed = moveSpeed;
		transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
	}

	private void Update() {
		float step = speed * Time.deltaTime;
		transform.position += moveDirection * step;
		distanceTraveled += step;

		if (distanceTraveled >= maxDistance) {
			Destroy(gameObject);
		}
	}
}