using UnityEngine;

public class TowerVisualRange : MonoBehaviour {
    [SerializeField] private BaseTowerSO _data;
    [SerializeField] private int segments = 64;
    [SerializeField] private Color circleColor = Color.red;

    private LineRenderer lineRenderer;

    private void Awake() {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = circleColor;
        lineRenderer.endColor = circleColor;
    }

    private void Start() {
        DrawCircle();
    }

    private void DrawCircle() {
        float radius = _data.range;
        Vector3[] points = new Vector3[segments];

        for (int i = 0; i < segments; i++) {
            float angle = i * Mathf.PI * 2f / segments;
            points[i] = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
        }

        lineRenderer.SetPositions(points);
    }
}
