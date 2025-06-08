using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		var enemiesCopy = enemiesInRange.ToList();

		foreach (EnemyLogic enemy in enemiesCopy) {
			enemy.DamageMe(buildableDataSO.damage);
		}

		for (int index = 0; index < arrowSpawnPoints.Count; index++) {
			Transform spawnPoint = arrowSpawnPoints[index];
			AoeBullet arrow = Instantiate(arrowPrefab, spawnPoint.position, Quaternion.identity);
			arrow.Initialize(spawnPoint.position - towerCenter.position, arrowSpeed);
		}
	}
}