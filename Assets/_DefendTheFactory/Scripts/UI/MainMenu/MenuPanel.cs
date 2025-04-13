using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour {
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button upgradesButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private RectTransform exitPanel;
    [SerializeField] private TextMeshProUGUI versionText;

    private void Start() {
        versionText.text = Application.version;
    }

    private void OnEnable() {
        SetupButtons();
    }

    private void OnDisable() {
        DisableButtons();
    }

    private void SetupButtons() {
        //continueButton.onClick.AddListener();
        newGameButton.onClick.AddListener(OnNewGameClicked);
        //upgradesButton.onClick.AddListener();
        //settingsButton.onClick.AddListener();
        exitButton.onClick.AddListener(() => ShowPanel(exitPanel));
    }

    private void DisableButtons() {
        continueButton.onClick.RemoveAllListeners();
        newGameButton.onClick.RemoveAllListeners();
        upgradesButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    private void OnNewGameClicked() {
        SceneLoader.Instance.LoadSceneGroup(1);
    }

    private void ShowPanel(RectTransform panel) {
        gameObject.SetActive(false);
        panel.gameObject.SetActive(true);
    }
}