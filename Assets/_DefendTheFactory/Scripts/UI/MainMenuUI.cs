using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [Header("Menu Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button upgradesButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    
    [Header("Exit Panel Buttons")]
    [SerializeField] private Button exitConfirmationButton;
    [SerializeField] private Button exitCancelButton;

    [Header("Panels")]
    [SerializeField] private RectTransform menuPanel;
    [SerializeField] private RectTransform exitPanel;

    private void Start() {
        SetupButtons();
    }

    private void SetupButtons() {
        SetupMenuPanelButtons();
    }

    private void SetupMenuPanelButtons() {
        //continueButton.onClick.AddListener();
        newGameButton.onClick.AddListener(OnNewGameClicked);
        //upgradesButton.onClick.AddListener();
        //settingsButton.onClick.AddListener();
        exitButton.onClick.AddListener(() => ShowExitPanel());
    }

    private void OnNewGameClicked() {
        SceneLoader.Instance.LoadSceneGroup(1);
    }

    private void ShowExitPanel(bool show = true) {
        menuPanel.gameObject.SetActive(!show);
        exitPanel.gameObject.SetActive(show);

        if (show) {
            exitConfirmationButton.onClick.AddListener(QuitGame);
            exitCancelButton.onClick.AddListener(() => ShowExitPanel(false));
        }
        else {
            exitConfirmationButton.onClick.RemoveAllListeners();
            exitCancelButton.onClick.RemoveAllListeners();
        }
    }

    private void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}