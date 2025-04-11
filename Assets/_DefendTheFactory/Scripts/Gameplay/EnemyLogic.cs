using System;
using UnityEngine;

public class EnemyLogic : MonoBehaviour {
    public event Action<EnemyLogic> OnDeath;

    public bool isFlying { get; private set; }

    private float speed;
    private float health = 10f;
    private int shield;
    private int damage;
    private Vector3 nextPoint;
    private int pathPointIndex;
    private WaveManager waveManager;
    private int pathPointsCount;

    public void Init(float speed) {
        waveManager = WaveManager.Instance;
        pathPointsCount = waveManager.pathPoints.Count;
        this.speed = speed;
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

    public void DamageMe(float damage) {
        health -= damage;

        if (health <= 0) {
            DestroyMe();
        }
    }

    private void DestroyMe() {
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}