using UnityEngine;
using UnityEngine.UI;

public class ExitPanel : MonoBehaviour {
    [SerializeField] private Button exitConfirmationButton;
    [SerializeField] private Button exitCancelButton;
    [SerializeField] private RectTransform menuPanel;

    private void OnEnable() {
        SetupButtons();
    }

    private void OnDisable() {
        DisableButtons();
    }

    private void SetupButtons() {
        exitConfirmationButton.onClick.AddListener(QuitGame);
        exitCancelButton.onClick.AddListener(() => ShowPanel(menuPanel));
    }

    private void DisableButtons() {
        exitConfirmationButton.onClick.RemoveAllListeners();
        exitCancelButton.onClick.RemoveAllListeners();
    }

    private void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowPanel(RectTransform panel) {
        gameObject.SetActive(false);
        panel.gameObject.SetActive(true);
    }
}