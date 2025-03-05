using UnityEngine;

public class MinerVisualRange : MonoBehaviour {

    [SerializeField] private Transform centerPosition;
    [SerializeField] private GatheringMachineSO _data;
    [SerializeField] private Color _discColor;
    [SerializeField] private Color _circleColor;

    private void OnDrawGizmos() {
        Gizmos.color = _discColor;

        // Draw a filled disc (use Handles for better visualization in Scene view)
#if UNITY_EDITOR
        UnityEditor.Handles.color = _discColor;
        UnityEditor.Handles.DrawSolidDisc(centerPosition.position, Vector3.up, _data.resourceSearchRange);
#endif
    }
}