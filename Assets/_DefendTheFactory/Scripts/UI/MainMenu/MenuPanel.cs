using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : BaseMenuPanel {
    [Header("Panels")] [SerializeField] private GameObject newGameWarningPanel;
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject exitPanel;

    [Header("Buttons")] [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button upgradesButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("Text")] [SerializeField] private TextMeshProUGUI versionText;

    private void Start() {
        versionText.text = Application.version;
    }

    protected override void SetupButtons() {
        //continueButton.onClick.AddListener();
        newGameButton.onClick.AddListener(() => ShowPanel(newGamePanel));
        //upgradesButton.onClick.AddListener(() => ShowPanel(upgradesPanel));
        settingsButton.onClick.AddListener(() => ShowPanel(settingsPanel));
        exitButton.onClick.AddListener(() => ShowPanel(exitPanel));
    }

    protected override void DisableButtons() {
        continueButton.onClick.RemoveAllListeners();
        newGameButton.onClick.RemoveAllListeners();
        upgradesButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }
}