using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPickingTower : BaseTower<TargetPickingTowerSO> {
    [SerializeField] private Transform rotatePointTransform;

    private EnemyLogic currentTarget;
    private TowerFocusType focusType;
    private Coroutine rotationCoroutine;

    protected override void Initialize(Vector2Int origin, BuildingDir dir, TargetPickingTowerSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    public override void GridSetupDone() {
        base.GridSetupDone();
        Subscribe();
    }

    public override void DestroySelf() {
        Unsubscribe();
        base.DestroySelf();
    }

    protected override void Attack() {
        CalculateTarget();
        currentTarget.DamageMe(buildableDataSO.damage);
    }

    private IEnumerator RotateToTarget() {
        while (currentTarget != null) {
            Vector3 directionToTarget = currentTarget.transform.position - rotatePointTransform.position;
            directionToTarget.y = 0;

            if (directionToTarget != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(-directionToTarget);
                rotatePointTransform.rotation = Quaternion.Slerp(rotatePointTransform.rotation, targetRotation, Time.deltaTime * buildableDataSO.rotationSpeed);
            }

            yield return null;
        }

        rotationCoroutine = null;
    }

    private void Subscribe() {
        Injector.Resolve<TimeTickSystem>().OnMicroTick += CalculateTarget;
    }

    private void Unsubscribe() {
        Injector.Resolve<TimeTickSystem>().OnMicroTick -= CalculateTarget;
    }

    private void CalculateTarget() {
        if (enemiesInRange.Count == 0) {
            currentTarget = null;
            return;
        }

        if (enemiesInRange.Count > 1) {
            CalculateBaseOnFilter();
        }
        else {
            var enumerator = enemiesInRange.GetEnumerator();
            enumerator.MoveNext();
            currentTarget = enumerator.Current;
        }

        if (buildableDataSO.shouldRotate && rotationCoroutine == null) {
            rotationCoroutine = StartCoroutine(RotateToTarget());
        }
    }

    private void CalculateBaseOnFilter() {
        switch (focusType) {
            case TowerFocusType.MostProgressed:
                CalculateEnemyWithMostProgression();
                break;
            case TowerFocusType.MostHp:
                CalculateEnemyWithMostHp();
                break;
            case TowerFocusType.LeastHp:
                CalculateEnemyWithLeastHp();
                break;
            case TowerFocusType.MostShield:
                CalculateEnemyWithMostShield();
                break;
            case TowerFocusType.LeastShield:
                CalculateEnemyWithLeastShield();
                break;
            case TowerFocusType.Fastest:
                CalculateFastestEnemy();
                break;
            case TowerFocusType.Slowest:
                CalculateSlowestEnemy();
                break;
        }
    }

    private void CalculateEnemyWithMostProgression() {
        HashSet<EnemyLogic> enemiesOnMostProgressedIndex = new HashSet<EnemyLogic>();
        int currentMostProgressedIndex = 0;

        foreach (EnemyLogic enemy in enemiesInRange) {
            if (enemy.pathPointIndex > currentMostProgressedIndex) {
                currentMostProgressedIndex = enemy.pathPointIndex;
                enemiesOnMostProgressedIndex.Clear();
                enemiesOnMostProgressedIndex.Add(enemy);
                continue;
            }

            if (enemy.pathPointIndex == currentMostProgressedIndex) {
                enemiesOnMostProgressedIndex.Add(enemy);
            }
        }

        if (enemiesOnMostProgressedIndex.Count == 1) {
            var enumerator = enemiesOnMostProgressedIndex.GetEnumerator();
            enumerator.MoveNext();
            currentTarget = enumerator.Current;
            return;
        }

        float shortestDistanceFromNextIndex = Mathf.Infinity;

        foreach (EnemyLogic enemy in enemiesOnMostProgressedIndex) {
            float distance = enemy.CalculateMyProgress();

            if (distance > shortestDistanceFromNextIndex) continue;

            shortestDistanceFromNextIndex = distance;
            currentTarget = enemy;
        }
    }

    private void CalculateEnemyWithMostHp() {
        int currentMostHp = 0;

        foreach (EnemyLogic enemy in enemiesInRange) {
            int enemyHp = enemy.health;

            if (enemyHp <= currentMostHp) continue;

            currentMostHp = enemyHp;
            currentTarget = enemy;
        }
    }

    private void CalculateEnemyWithLeastHp() {
        int currentLeastHp = int.MaxValue;

        foreach (EnemyLogic enemy in enemiesInRange) {
            int enemyHp = enemy.health;

            if (enemyHp >= currentLeastHp) continue;

            currentLeastHp = enemyHp;
            currentTarget = enemy;
        }
    }

    private void CalculateEnemyWithMostShield() {
        int currentMostShield = 0;

        foreach (EnemyLogic enemy in enemiesInRange) {
            int enemyShield = enemy.shield;

            if (enemyShield <= currentMostShield) continue;

            currentMostShield = enemyShield;
            currentTarget = enemy;
        }
    }

    private void CalculateEnemyWithLeastShield() {
        int currentLeastShield = int.MaxValue;

        foreach (EnemyLogic enemy in enemiesInRange) {
            int enemyShield = enemy.shield;

            if (enemyShield >= currentLeastShield) continue;

            currentLeastShield = enemyShield;
            currentTarget = enemy;
        }
    }

    private void CalculateFastestEnemy() {
        float currentFastest = 0f;

        foreach (EnemyLogic enemy in enemiesInRange) {
            float enemySpeed = enemy.speed;

            if (enemySpeed <= currentFastest) continue;

            currentFastest = enemySpeed;
            currentTarget = enemy;
        }
    }

    private void CalculateSlowestEnemy() {
        float currentSlowest = Mathf.Infinity;

        foreach (EnemyLogic enemy in enemiesInRange) {
            float enemySpeed = enemy.speed;

            if (enemySpeed >= currentSlowest) continue;

            currentSlowest = enemySpeed;
            currentTarget = enemy;
        }
    }
}