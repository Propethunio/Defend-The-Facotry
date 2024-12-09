using UnityEngine;

public class Mouse3D : MonoBehaviour {

    public static Mouse3D Instance { get; private set; }

    [SerializeField] LayerMask mouseColliderLayerMask;

    public static bool TryGetMouseWorldPosition(out Vector3 mousePosition) => Instance.TryGetMouseWorldPosition_Instance(out mousePosition);

    void Awake() {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    bool TryGetMouseWorldPosition_Instance(out Vector3 mousePosition) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, mouseColliderLayerMask)) {
            mousePosition = raycastHit.point;
            return true;
        } else {
            mousePosition = Vector3.zero;
            return false;
        }
    }
}