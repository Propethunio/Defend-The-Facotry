using UnityEngine;

public abstract class BaseMenuPanel : MonoBehaviour {
    private void OnEnable() {
        SetupButtons();
    }

    private void OnDisable() {
        DisableButtons();
    }

    protected abstract void SetupButtons();
    protected abstract void DisableButtons();
    
    protected void ShowPanel(GameObject panel) {
        gameObject.SetActive(false);
        panel.SetActive(true);
    }
}