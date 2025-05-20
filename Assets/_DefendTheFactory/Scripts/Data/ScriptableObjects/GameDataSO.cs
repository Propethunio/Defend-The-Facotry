using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/LevelData")]
public class GameDataSO : ScriptableObject {
	[field: SerializeField] public MapDataSO mapData { get; private set; }
	[field: SerializeField] public List<BaseBuildableObjectSO> factoryBuildingsData { get; private set; }
	[field: SerializeField] public List<BaseBuildableObjectSO> towersData { get; private set; }
}