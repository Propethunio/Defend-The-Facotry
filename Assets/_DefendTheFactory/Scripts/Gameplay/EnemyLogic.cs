using System;
using UnityEngine;

public class EnemyLogic : Flyweight {
	public event Action<EnemyLogic> OnDeath;

	[SerializeField] private EnemyBar statBars;

	public bool isFlying { get; private set; }
	public int pathPointIndex { get; private set; }
	public int health { get; private set; }
	public int shield { get; private set; }
	public float speed { get; private set; }

	private Quaternion targetRotation;
	private Vector3 nextPoint;
	private WaveManager waveManager;
	private FlyweightFactory factory;
	private int pathPointsCount;
	private int maxHealth;
	private int maxShield;
	private int damage;

	public void SetData(EnemySO data) {
		factory = Injector.Resolve<FlyweightFactory>();
		waveManager = Injector.Resolve<WaveManager>();
		pathPointsCount = waveManager.pathPoints.Count;
		maxHealth = data.health;
		maxShield = data.shield;
		speed = data.speed;
		isFlying = data.isFlying;
		damage = data.damage;
	}

	public void Init() {
		pathPointIndex = 0;
		health = maxHealth;
		shield = maxShield;
		SetNextPoint();
		SetDirection();
		transform.rotation = targetRotation;
	}

	private void Update() {
		if (Vector3.Distance(transform.position, nextPoint) < 0.05f) {
			if (pathPointIndex == pathPointsCount) {
				DamageBase();
				return;
			}

			SetNextPoint();
			SetDirection();
		}

		Rotate();
		Move();
	}

	private void Rotate() {
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
	}

	private void Move() {
		transform.position = Vector3.MoveTowards(transform.position, nextPoint, speed * Time.deltaTime);
	}

	private void SetNextPoint() {
		Vector2Int nextPoint2D = waveManager.pathPoints[pathPointIndex];
		nextPoint = new Vector3(nextPoint2D.x + 0.5f, transform.position.y, nextPoint2D.y + 0.5f);
		pathPointIndex++;
	}

	private void SetDirection() {
		Vector3 rotateDirection = transform.position - nextPoint;
		rotateDirection.y = 0;
		targetRotation = Quaternion.LookRotation(rotateDirection);
	}

	private void DamageBase() {
		Injector.Resolve<HealthManager>().Damage(damage);
		DestroyMe();
	}

	private void DestroyMe() {
		OnDeath?.Invoke(this);
		factory.ReturnToPool(this);
	}

	public void DamageMe(int damage) {
		health -= damage;

		if (health <= 0) {
			DestroyMe();
		}
		else {
			statBars.SetHealthBar((float)health / maxHealth);
		}
	}

	public float CalculateMyProgress() {
		return Vector3.Distance(transform.position, nextPoint);
	}
}