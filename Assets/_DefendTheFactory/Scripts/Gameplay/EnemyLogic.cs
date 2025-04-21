using System;
using UnityEngine;

public class EnemyLogic : MonoBehaviour {
    public event Action<EnemyLogic> OnDeath;

    [SerializeField] EnemyBar statBars;

    public bool isFlying { get; private set; }
    public int pathPointIndex { get; private set; }
    public int health { get; private set; }
    public int shield { get; private set; }
    public float speed { get; private set; }

    private Vector3 nextPoint;
    private WaveManager waveManager;
    private int pathPointsCount;
    private int maxHealth;
    private int maxShield;

    public void Init(EnemySO data) {
        waveManager = Injector.Resolve<WaveManager>();
        pathPointsCount = waveManager.pathPoints.Count;
        maxHealth = data.health;
        health = maxHealth;
        maxShield = data.shield;
        shield = maxShield;
        speed = data.speed;
        isFlying = data.isFlying;
        SetNextPoint();
    }

    private void Update() {
        if (Vector3.Distance(transform.position, nextPoint) < 0.05f) {
            if (pathPointIndex == pathPointsCount) {
                DamageBase();
                return;
            }

            SetNextPoint();
        }

        Move();
    }

    private void Move() {
        transform.position = Vector3.MoveTowards(transform.position, nextPoint, speed * Time.deltaTime);
    }

    private void SetNextPoint() {
        Vector2Int nextPoint2D = waveManager.pathPoints[pathPointIndex];
        nextPoint = new Vector3(nextPoint2D.x + 0.5f, transform.position.y, nextPoint2D.y + 0.5f);
        pathPointIndex++;
    }

    private void DamageBase() {
        DestroyMe();
    }

    private void DestroyMe() {
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    public void DamageMe(int damage) {
        health -= damage;

        if (health <= 0) {
            DestroyMe();
        } else {
            statBars.SetHealthBar((float)health / maxHealth);
        }
    }

    public float CalculateMyProgress() {
        return Vector3.Distance(transform.position, nextPoint);
    }
}