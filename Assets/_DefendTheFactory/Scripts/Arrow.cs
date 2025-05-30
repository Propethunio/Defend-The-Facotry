using UnityEngine;

public class Arrow : MonoBehaviour {
	private EnemyLogic target;
	private int damage;
	private float speed;
	private float hitThreshold = 0.1f;

	public void Initialize(EnemyLogic target, int damage, float speed) {
		this.target = target;
		this.damage = damage;
		this.speed = speed;
	}

	private void Update() {
		if (target == null || target.isDead) {
			Destroy(gameObject);
			return;
		}

		Vector3 hitPoint = target.transform.position + new Vector3(0f, .25f, 0f);
		Vector3 direction = (hitPoint - transform.position).normalized;
		transform.position += speed * Time.deltaTime * direction;

		transform.rotation = Quaternion.LookRotation(direction);

		float distance = Vector3.Distance(transform.position, hitPoint);

		if (!(distance <= hitThreshold)) return;

		target.DamageMe(damage);
		Destroy(gameObject);
	}
}