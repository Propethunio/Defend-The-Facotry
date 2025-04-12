using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
[DisallowMultipleComponent]
public class ResponsiveCanvasScaler : MonoBehaviour {
    [SerializeField] private float uiScale = 1f;

    private CanvasScaler canvasScaler;
    private Vector2 lastResolution;

    private void Awake() {
        canvasScaler = GetComponent<CanvasScaler>();
        lastResolution = new Vector2(Screen.width, Screen.height);
        ApplyScale();
    }

    private void Update() {
        if (Mathf.Approximately(lastResolution.x, Screen.width) && Mathf.Approximately(lastResolution.y, Screen.height)) return;

        lastResolution = new Vector2(Screen.width, Screen.height);
        ApplyScale();
    }

    private void ApplyScale() {
        canvasScaler.scaleFactor = lastResolution.y / 1080f * uiScale;
    }

    public void SetUIScale(float scale) {
        uiScale = scale;
        ApplyScale();
    }
}