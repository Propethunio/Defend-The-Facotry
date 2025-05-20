using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupData : Singleton<GameSetupData> {
	[SerializeField] private List<GameDataSO> LevelsDataList;
	
	private GameDataSO selectedLevelData;
	private SceneLoader sceneLoader;
	
	protected override void Awake() {
		base.Awake();
		DontDestroyOnLoad(gameObject);
	}

	private void Start() {
		sceneLoader = SceneLoader.Instance;
	}

	private void OnDestroy() {
		sceneLoader.OnSceneGroupLoaded -= OnGameLoaded;
	}

	public void StartLevel(int index) {
		selectedLevelData = LevelsDataList[index];
		sceneLoader.OnSceneGroupLoaded += OnGameLoaded;
		sceneLoader.LoadSceneGroup(1);
	}

	public MapDataSO GetLevelData() {
		return selectedLevelData.mapData;
	}

	public List<BaseBuildableObjectSO> GetFactoryBuildingsData() {
		return selectedLevelData.factoryBuildingsData;
	}
	
	public List<BaseBuildableObjectSO> GetTowersData() {
		return selectedLevelData.towersData;
	}

	public bool IsTutorialLevel() {
		return selectedLevelData.isTutorialLevel;
	}

	private void OnGameLoaded() {
		Destroy(gameObject);
	}
}