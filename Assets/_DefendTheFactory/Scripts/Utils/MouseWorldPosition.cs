using UnityEngine;

public class MouseWorldPosition : DependencyMonoBehaviour<MouseWorldPosition> {
    private Camera cam;

    private void Start() {
        cam = Camera.main;
    }

    public bool TryGetMouseWorldPosition(out Vector3 mousePosition) {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, 1 << 3)) {
            mousePosition = raycastHit.point;
            return true;
        }
        else {
            mousePosition = Vector3.zero;
            return false;
        }
    }
}