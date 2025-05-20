using System.Collections.Generic;
using UnityEngine;

public class GameSetupData : Singleton<GameSetupData> {
	[SerializeField] private List<GameDataSO> LevelsDataList;

	private GameDataSO selectedLevelData;
	private SceneLoader sceneLoader;
	private bool isTutorialLevelSelected;

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
		isTutorialLevelSelected = index == 0;
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
		return isTutorialLevelSelected;
	}

	private void OnGameLoaded() {
		Destroy(gameObject);
	}
}