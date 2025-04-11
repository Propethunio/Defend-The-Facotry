using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class BaseTower<T> : BaseDataPlacedObject<T> where T : BaseTowerSO {
    protected List<EnemyLogic> enemiesInRange = new List<EnemyLogic>();
    private bool readyToAttack = true;
    private float cooldownTimer;

    protected override void Initialize(Vector2Int origin, BuildingDir dir, T buildableDataSO) { }

    public override void GridSetupDone() {
        SphereCollider rangeCollider = GetComponent<SphereCollider>();
        Vector2 centerPosition = buildableDataSO.GetCenterPositionForCollider(dir);
        rangeCollider.center = new Vector3(centerPosition.x, rangeCollider.center.y, centerPosition.y);
        rangeCollider.radius = buildableDataSO.range;
        cooldownTimer = 1f / buildableDataSO.attackSpeed;
    }

    private void OnTriggerEnter(Collider other) {
        EnemyLogic enemy = other.gameObject.GetComponent<EnemyLogic>();

        if (!buildableDataSO.canTargetFlying && enemy.isFlying) return;

        enemiesInRange.Add(enemy);
        enemy.OnDeath += RemoveKilledEnemy;

        if (!readyToAttack) return;

        StartCoroutine(AttackCycle());
    }

    private void OnTriggerExit(Collider other) {
        EnemyLogic enemy = other.gameObject.GetComponent<EnemyLogic>();
        enemy.OnDeath -= RemoveKilledEnemy;
        enemiesInRange.Remove(enemy);
    }

    private IEnumerator AttackCycle() {
        readyToAttack = false;

        while (enemiesInRange.Count > 0) {
            Attack();
            yield return new WaitForSeconds(cooldownTimer);
        }

        readyToAttack = true;
    }

    private void RemoveKilledEnemy(EnemyLogic enemy) {
        enemy.OnDeath -= RemoveKilledEnemy;
        enemiesInRange.Remove(enemy);
    }

    protected abstract void Attack();
}