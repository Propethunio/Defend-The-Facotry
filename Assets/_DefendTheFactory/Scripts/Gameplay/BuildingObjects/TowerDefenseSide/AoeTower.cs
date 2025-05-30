using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeTower : BaseTower<AoeTowerSO> {
	[SerializeField] private Transform towerCenter;
	[SerializeField] private AoeBullet arrowPrefab;
	[SerializeField] private List<Transform> arrowSpawnPoints;
	public float arrowSpeed;

	protected override void Initialize(Vector2Int origin, BuildingDir dir, AoeTowerSO buildableDataSO) {
		BaseDataSet(origin, dir, buildableDataSO);
	}

	protected override IEnumerator AttackCycle() {
		readyToAttack = false;

		while (enemiesInRange.Count > 0) {
			Attack();
			yield return new WaitForSeconds(cooldownTimer);
		}

		readyToAttack = true;
	}

	protected override void Attack() {
		foreach (EnemyLogic enemy in enemiesInRange) {
			enemy.DamageMe(buildableDataSO.damage);
		}

		for (int index = 0; index < arrowSpawnPoints.Count; index++) {
			Transform spawnPoint = arrowSpawnPoints[index];
			AoeBullet arrow = Instantiate(arrowPrefab, spawnPoint.position, Quaternion.identity);
			arrow.Initialize(towerCenter.position - spawnPoint.position, arrowSpeed);
		}
	}
}