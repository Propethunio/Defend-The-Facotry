using UnityEngine;

public class MinerVisualRange : MonoBehaviour {

    [SerializeField] private float range;
    [SerializeField] private Color discColor;
    [SerializeField] private Color circleColor;

    private void OnDrawGizmos() {
        Gizmos.color = discColor;

        // Draw a filled disc (use Handles for better visualization in Scene view)
#if UNITY_EDITOR
        UnityEditor.Handles.color = discColor;
        UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, range);
#endif
    }
}