using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class BaseTower<T> : BaseDataPlacedObject<T> where T : BaseTowerSO {
    protected HashSet<EnemyLogic> enemiesInRange = new HashSet<EnemyLogic>();
    protected bool readyToAttack = true;
    protected float cooldownTimer;

    protected override void Initialize(Vector2Int origin, BuildingDir dir, T buildableDataSO) { }

    public override void GridSetupDone() {
        SphereCollider rangeCollider = GetComponent<SphereCollider>();
        Vector2 centerPosition = buildableDataSO.GetCenterPositionForCollider(dir);
        rangeCollider.center = new Vector3(centerPosition.x, rangeCollider.center.y, centerPosition.y);
        rangeCollider.radius = buildableDataSO.range;
        cooldownTimer = 1f / buildableDataSO.attackSpeed;
    }
    
    protected abstract IEnumerator AttackCycle();
    
    protected abstract void Attack();

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

    private void RemoveKilledEnemy(EnemyLogic enemy) {
        enemy.OnDeath -= RemoveKilledEnemy;
        enemiesInRange.Remove(enemy);
    }
}