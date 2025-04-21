using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Enemies/EnemyWaves")]
public class EnemyWavesSO : ScriptableObject {
    [field: SerializeField] public int dayLength { get; private set; }
    [field: SerializeField] public List<WaveDayData> waveDayData { get; private set; }
}

[Serializable]
public struct WaveDayData {
    [field: SerializeField] public int startSpawnAfter { get; private set; }
    [field: SerializeField] public int chanceToSpawnDay { get; private set; }
    [field: SerializeField] public List<EnemyWeightPair> enemiesDuringDay { get; private set; }
    [field: SerializeField] public int spawnAtNightAmount { get; private set; }
    [field: SerializeField] public int chanceToSpawnNight { get; private set; }
    [field: SerializeField] public List<EnemyWeightPair> enemiesDuringNight { get; private set; }
}

[Serializable]
public struct EnemyWeightPair {
    [field: SerializeField] public EnemySO enemy { get; private set; }
    [field: SerializeField] public int weight { get; private set; }
}