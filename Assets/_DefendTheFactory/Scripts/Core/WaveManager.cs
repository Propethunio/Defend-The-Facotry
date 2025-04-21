using System.Collections.Generic;
using UnityEngine;

public class WaveManager {
    public List<Vector2Int> pathPoints { get; private set; } = new List<Vector2Int>();

    private EnemyWavesSO wavesData;
    private WaveDayData currentDayData;
    private int ticksAmount;
    private bool isNight;
    private int spawnWieghtCombined;
    private Vector3 spawnPosition;
    private Transform enemyParentTransform;
    private HashSet<EnemyLogic> nightEnemiesList;
    private int daysSurvived;

    public void Init() {
        enemyParentTransform = new GameObject("Enemies").transform;
        Injector.Resolve<TimeTickSystem>().OnTick += OnTick;
    }

    ~WaveManager() {
        Injector.Resolve<TimeTickSystem>().OnTick -= OnTick;
    }

    public void SetEnemiesData(EnemyWavesSO enemiesData) {
        wavesData = enemiesData;
        currentDayData = wavesData.waveDayData[0];

        int enemiesCount = currentDayData.enemiesDuringDay.Count;

        for(int i = 0; i < enemiesCount; i++) {
            spawnWieghtCombined += currentDayData.enemiesDuringDay[i].weight;
        }
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
        if(!isNight) {
            HandleDayTick();
        } else {
            HandleNightTick();
        }
    }

    private void HandleDayTick() {
        ticksAmount++;
        if(ticksAmount == wavesData.dayLength) {
            isNight = true;
            ticksAmount = 0;
            spawnWieghtCombined = 0;

            int enemiesCount = currentDayData.enemiesDuringNight.Count;

            for(int i = 0; i < enemiesCount; i++) {
                spawnWieghtCombined += currentDayData.enemiesDuringNight[i].weight;
            }

            return;
        }

        if(ticksAmount <= currentDayData.startSpawnAfter) return;

        if(Random.Range(0, 100) < currentDayData.chanceToSpawnDay) {
            int roll = Random.Range(0, spawnWieghtCombined);
            int cumulative = 0;
            int enemiesCount = currentDayData.enemiesDuringDay.Count;

            for (int i = 0; i < enemiesCount; i++) {
                var enemyWithWeight = currentDayData.enemiesDuringDay[i];
                cumulative += enemyWithWeight.weight;

                if(roll < cumulative) {
                    GameObject.Instantiate(enemyWithWeight.enemy.modelPrefab, spawnPosition, Quaternion.identity, enemyParentTransform).GetComponent<EnemyLogic>().Init(enemyWithWeight.enemy);
                }
            }
        }
    }

    private void HandleNightTick() {
        if(ticksAmount == currentDayData.spawnAtNightAmount) {
            isNight = false;
            ticksAmount = 0;
            daysSurvived++;
            currentDayData = wavesData.waveDayData[daysSurvived];
            spawnWieghtCombined = 0;

            int enemiesCount = currentDayData.enemiesDuringDay.Count;

            for(int i = 0; i < enemiesCount; i++) {
                spawnWieghtCombined += currentDayData.enemiesDuringDay[i].weight;
            }
            return;
        }

        if(Random.Range(0, 100) < currentDayData.chanceToSpawnNight) {
            int roll = Random.Range(0, spawnWieghtCombined);
            int cumulative = 0;
            int enemiesCount = currentDayData.enemiesDuringNight.Count;

            for(int i = 0; i < enemiesCount; i++) {
                var enemyWithWeight = currentDayData.enemiesDuringNight[i];
                cumulative += enemyWithWeight.weight;

                if(roll < cumulative) {
                    EnemyLogic enemy = GameObject.Instantiate(enemyWithWeight.enemy.modelPrefab, spawnPosition, Quaternion.identity, enemyParentTransform).GetComponent<EnemyLogic>();
                    enemy.Init(enemyWithWeight.enemy);

                    //DAJ EVENT NA JEGO SMIERC ZEBY SIE ODEJMOWAL :D

                    nightEnemiesList.Add(enemy);
                }
            }
        }
    }
}