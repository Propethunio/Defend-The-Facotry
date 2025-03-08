using UnityEngine;

public class Mouse3D : MonoBehaviour {
    [SerializeField] private LayerMask mouseColliderLayerMask;

    private static Mouse3D Instance;
    private Camera cam;

    public static bool TryGetMouseWorldPosition(out Vector3 mousePosition) => Instance.TryGetMouseWorldPosition_Instance(out mousePosition);

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() {
        cam = Camera.main;
    }

    private bool TryGetMouseWorldPosition_Instance(out Vector3 mousePosition) {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, mouseColliderLayerMask)) {
            mousePosition = raycastHit.point;
            return true;
        }
        else {
            mousePosition = Vector3.zero;
            return false;
        }
    }
}