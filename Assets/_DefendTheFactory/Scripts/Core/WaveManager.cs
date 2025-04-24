using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager {
	public List<Vector2Int> pathPoints { get; private set; } = new List<Vector2Int>();

	private EnemyWavesSO wavesData;
	private WaveDayData currentDayData;
	private int ticksAmount;
	private bool isNight;
	private int spawnWeightCombined;
	private Vector3 spawnPosition;
	private HashSet<EnemyLogic> nightEnemiesList = new HashSet<EnemyLogic>();
	private int daysSurvived;
	private FlyweightFactory factory;

	public event Action<bool> OnNightActive;
	public event Action<int> OnWaveTick;
	
	public void Init() {
		factory = Injector.Resolve<FlyweightFactory>();
		Injector.Resolve<TimeTickSystem>().OnTick += OnTick;
	}

	~WaveManager() {
		Injector.Resolve<TimeTickSystem>().OnTick -= OnTick;
	}

	public void SetEnemiesData(EnemyWavesSO enemiesData) {
		wavesData = enemiesData;
		currentDayData = wavesData.waveDayData[0];

		int enemiesCount = currentDayData.enemiesDuringDay.Count;

		for (int i = 0; i < enemiesCount; i++) {
			spawnWeightCombined += currentDayData.enemiesDuringDay[i].weight;
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
		if (!isNight) {
			HandleDayTick();
		}
		else {
			HandleNightTick();
		}
	}

	private void HandleDayTick() {
		ticksAmount++;
		OnWaveTick?.Invoke((wavesData.dayLength - ticksAmount) / 2);
		if (ticksAmount == wavesData.dayLength) {
			ChangeDayIntoNight();
		}

		if (ticksAmount <= currentDayData.startSpawnAfter || Random.Range(0, 100) >= currentDayData.chanceToSpawnDay) return;

		SpawnEnemyDay();
	}

	private void ChangeDayIntoNight() {
		isNight = true;
		OnNightActive?.Invoke(isNight);
		ticksAmount = 0;
		spawnWeightCombined = 0;
		int enemiesCount = currentDayData.enemiesDuringNight.Count;

		for (int i = 0; i < enemiesCount; i++) {
			spawnWeightCombined += currentDayData.enemiesDuringNight[i].weight;
		}
	}

	private void SpawnEnemyDay() {
		int roll = Random.Range(0, spawnWeightCombined);
		int cumulative = 0;
		int enemiesCount = currentDayData.enemiesDuringDay.Count;

		for (int i = 0; i < enemiesCount; i++) {
			EnemyWeightPair enemyWithWeight = currentDayData.enemiesDuringDay[i];
			cumulative += enemyWithWeight.weight;

			if (roll >= cumulative) continue;

			EnemyLogic enemy = factory.Spawn(enemyWithWeight.enemy) as EnemyLogic;
			enemy.transform.position = spawnPosition;
			enemy.transform.rotation = Quaternion.identity;
			enemy.Init();
		}
	}

	private void HandleNightTick() {
		if (ticksAmount == currentDayData.spawnAtNightAmount) {
			if (nightEnemiesList.Count == 0) {
				ChangeNightIntoDay();
			}
			return;
		}

		if (Random.Range(0, 100) < currentDayData.chanceToSpawnNight) {
			SpawnEnemyNight();
		}
	}

	private void ChangeNightIntoDay() {
		isNight = false;
		OnNightActive?.Invoke(isNight);
		ticksAmount = 0;
		spawnWeightCombined = 0;
		daysSurvived++;
		currentDayData = wavesData.waveDayData[daysSurvived];
		int enemiesCount = currentDayData.enemiesDuringDay.Count;

		for (int i = 0; i < enemiesCount; i++) {
			spawnWeightCombined += currentDayData.enemiesDuringDay[i].weight;
		}
	}

	private void SpawnEnemyNight() {
		int roll = Random.Range(0, spawnWeightCombined);
		int cumulative = 0;
		int enemiesCount = currentDayData.enemiesDuringNight.Count;

		for (int i = 0; i < enemiesCount; i++) {
			EnemyWeightPair enemyWithWeight = currentDayData.enemiesDuringNight[i];
			cumulative += enemyWithWeight.weight;

			if (roll >= cumulative) continue;

			EnemyLogic enemy = factory.Spawn(enemyWithWeight.enemy) as EnemyLogic;
			enemy.transform.position = spawnPosition;
			enemy.transform.rotation = Quaternion.identity;
			enemy.Init();
			enemy.OnDeath += RemoveEnemyFromNightList;
			nightEnemiesList.Add(enemy);
			ticksAmount++;
			OnWaveTick?.Invoke(ticksAmount);
		}
	}

	private void RemoveEnemyFromNightList(EnemyLogic enemy) {
		enemy.OnDeath -= RemoveEnemyFromNightList;
		nightEnemiesList.Remove(enemy);
	}
}