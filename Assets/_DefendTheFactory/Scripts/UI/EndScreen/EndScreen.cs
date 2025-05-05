using System;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour {
	[SerializeField] private GameObject endScreen;
	[SerializeField] private Button playAgainButton;
	[SerializeField] private Button mainMenuButton;

	private HealthManager healthManager;
	
	private void Start() {
		healthManager = Injector.Resolve<HealthManager>();
		Subscribe();
	}

	private void OnDestroy() {
		Unsubscribe();
	}

	private void Subscribe() {
		playAgainButton.onClick.AddListener(ReloadGame);
		mainMenuButton.onClick.AddListener(LoadMainMenu);
		healthManager.GameOver += OnGameEnd;
	}

	private void Unsubscribe() {
		healthManager.GameOver -= OnGameEnd;
	}

	private void OnGameEnd() {
		endScreen.gameObject.SetActive(true);
	}

	private void ReloadGame() {
		SceneLoader.Instance.LoadSceneGroup(1);
	}

	private void LoadMainMenu() {
		SceneLoader.Instance.LoadSceneGroup(0);
	}
}