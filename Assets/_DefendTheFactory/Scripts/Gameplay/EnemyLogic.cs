using System;
using UnityEngine;

public class EnemyLogic : MonoBehaviour {
    public event Action<EnemyLogic> OnDeath;

    private float speed;
    private int health;
    private int shield;
    private bool isFlying;
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
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}