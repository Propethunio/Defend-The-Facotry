using UnityEngine;
using UnityEngine.UI;

public class ExitPanel : BaseMenuPanel {
    [SerializeField] private GameObject menuPanel;

    [Header("Buttons")] [SerializeField] private Button exitConfirmationButton;
    [SerializeField] private Button exitCancelButton;

    protected override void SetupButtons() {
        exitConfirmationButton.onClick.AddListener(QuitGame);
        exitCancelButton.onClick.AddListener(() => ShowPanel(menuPanel));
    }

    protected override void DisableButtons() {
        exitConfirmationButton.onClick.RemoveAllListeners();
        exitCancelButton.onClick.RemoveAllListeners();
    }

    private void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }
}