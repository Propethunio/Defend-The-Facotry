using System.Collections.Generic;
using UnityEngine;

public class WaveManager {
    private Vector3 spawnPosition;
    private Transform enemyParentTransform;
    private EnemyLogic enemy;
    public List<Vector2Int> pathPoints { get; private set; } = new List<Vector2Int>();

    public void Init(EnemyLogic enemy) {
        enemyParentTransform = new GameObject("Enemies").transform;
        this.enemy = enemy;
        Injector.Resolve<TimeTickSystem>().OnTick += OnTick;
    }

    ~WaveManager() {
        Injector.Resolve<TimeTickSystem>().OnTick -= OnTick;
    }

    public void SetPath(List<Vector2Int> pathCells) {
        pathPoints = pathCells;
        pathPoints.Reverse();
        pathPoints.Add(pathPoints[^1] + new Vector2Int(1, 0));
    }

    public void SetSpawnPosition(Vector3 position) {
        spawnPosition = position;
    }

    private void OnTick() {
        if (Random.Range(0, 100) > 15) return;

        GameObject.Instantiate(enemy, spawnPosition, Quaternion.identity, enemyParentTransform).Init(Random.Range(0.5f, 2f));
    }
}