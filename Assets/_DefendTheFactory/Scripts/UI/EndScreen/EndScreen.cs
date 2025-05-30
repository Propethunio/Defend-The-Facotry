using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour {
	[SerializeField] private GameObject endScreen;
	[SerializeField] private TMP_Text endText;
	[SerializeField] private Button playAgainButton;
	[SerializeField] private Button mainMenuButton;

	private HealthManager healthManager;
	private GameManager gameManager;
	
	private void Start() {
		healthManager = Injector.Resolve<HealthManager>();
		gameManager = Injector.Resolve<GameManager>();
		Subscribe();
	}

	private void OnDestroy() {
		Unsubscribe();
	}

	private void Subscribe() {
		playAgainButton.onClick.AddListener(ReloadGame);
		mainMenuButton.onClick.AddListener(LoadMainMenu);
		healthManager.GameOver += OnGameLost;
		gameManager.OnGameWon += OnGameWon;
	}

	private void Unsubscribe() {
		healthManager.GameOver -= OnGameLost;
		gameManager.OnGameWon -= OnGameWon;
	}

	private void OnGameWon() {
		playAgainButton.gameObject.SetActive(false);
		endText.text = "VICTORY!";
		endScreen.gameObject.SetActive(true);
	}
	
	private void OnGameLost() {
		endScreen.gameObject.SetActive(true);
	}

	private void ReloadGame() {
		SceneLoader.Instance.LoadSceneGroup(1);
	}

	private void LoadMainMenu() {
		SceneLoader.Instance.LoadSceneGroup(0);
	}
}