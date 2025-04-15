using Unity.Cinemachine;
using UnityEngine;

public class CameraFollowTarget : DependencyMonoBehaviour<CameraFollowTarget> {
    [SerializeField] private CinemachineFollow cinemachineFollow;

    [Header("Screen Edge")] [SerializeField]
    private bool moveOnEdge;

    [SerializeField] private int edgeScrollSize;
    [Header("Movement")] [SerializeField] private float moveSpeed;
    [SerializeField] private float moveSpeedOnDrag;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float rotateSpeedOnDrag;
    [Header("Zoom")] [SerializeField] private float zoomSpeed;
    [SerializeField] private float zoomScrollWheelClamp;
    [SerializeField] private float zoomLerpSpeed;
    [SerializeField] private Vector2 zoomRange;
    [SerializeField] private AnimationCurve zoomRotationCurve;

    [Header("Map Border")] [SerializeField]
    private float mapBorderSmoothPadding;

    [SerializeField] private float mapBorderHardPadding;
    [SerializeField] private float lerpSpeedOutsideBorder;

    private Transform target;
    private InputManager inputManager;
    private Vector3 followOffset;
    private Vector2 lastMousePosMove;
    private float lastMousePosRotate;
    private float screenRightScroll;
    private float screenTopScroll;
    private float zoomAmount;
    private bool moveOnHoldActive;
    private bool rotateOnHoldActive;
    private Vector2Int mapSize;
    private float dynamicPadding;

    private void Start() {
        target = transform;
        inputManager = Injector.Resolve<InputManager>();
        followOffset = cinemachineFollow.FollowOffset;
        zoomAmount = followOffset.y;
        followOffset.z = zoomRotationCurve.Evaluate(zoomAmount);
        cinemachineFollow.FollowOffset = followOffset;
        CalculateScrollBounds();
        CalculateDynamicPadding();
        SubscribeEvents();
    }

    private void Update() {
        RotateCamera();
        ZoomCamera();
        MoveCamera();
    }

    private bool IsOutsideBorders(Vector3 position) {
        return position.x < dynamicPadding || position.x > mapSize.x - dynamicPadding || position.z < dynamicPadding || position.z > mapSize.y - dynamicPadding;
    }

    private void SmoothClampCameraPosition() {
        Vector3 targetPosition = target.position;

        if (targetPosition.x < dynamicPadding) {
            targetPosition.x = Mathf.Lerp(targetPosition.x, dynamicPadding, Time.deltaTime * lerpSpeedOutsideBorder);
        }
        else if (targetPosition.x > mapSize.x - dynamicPadding) {
            targetPosition.x = Mathf.Lerp(targetPosition.x, mapSize.x - dynamicPadding, Time.deltaTime * lerpSpeedOutsideBorder);
        }

        if (targetPosition.z < dynamicPadding) {
            targetPosition.z = Mathf.Lerp(targetPosition.z, dynamicPadding, Time.deltaTime * lerpSpeedOutsideBorder);
        }
        else if (targetPosition.z > mapSize.y - dynamicPadding) {
            targetPosition.z = Mathf.Lerp(targetPosition.z, mapSize.y - dynamicPadding, Time.deltaTime * lerpSpeedOutsideBorder);
        }

        target.position = targetPosition;
    }

    public void SetCameraPosition(Vector2 position) {
        target.position = new Vector3(position.x, target.position.y, position.y);
    }

    public void SetCameraPosition(Vector3 position) {
        target.position = new Vector3(position.x, target.position.y, position.y);
    }

    public void SetMapSize(Vector2Int size) {
        mapSize = size;
    }

    private void CalculateScrollBounds() {
        screenRightScroll = Screen.width - edgeScrollSize;
        screenTopScroll = Screen.height - edgeScrollSize;
    }

    private void SubscribeEvents() {
        inputManager.rightClickPerformedAction += InputManager_rightClickPerformed;
        inputManager.rightClickCanceledAction += InputManager_rightClickCanceled;
        inputManager.scrollClickPerformedAction += InputManager_scrollClickPerformed;
        inputManager.scrollClickCanceledAction += InputManager_scrollClickCanceled;
    }

    private void InputManager_rightClickPerformed() {
        if (rotateOnHoldActive) return;

        moveOnHoldActive = true;
        lastMousePosMove = inputManager.mousePos;
    }

    private void InputManager_rightClickCanceled() {
        moveOnHoldActive = false;
    }

    private void InputManager_scrollClickPerformed() {
        if (moveOnHoldActive) return;

        rotateOnHoldActive = true;
        lastMousePosRotate = inputManager.mousePos.x;
    }

    private void InputManager_scrollClickCanceled() {
        rotateOnHoldActive = false;
    }

    private void MoveCamera() {
        Vector2 inputDir = Vector2.zero;

        if (moveOnHoldActive) {
            Vector2 newMousePosMove = inputManager.mousePos;
            Vector2 mouseMove = newMousePosMove - lastMousePosMove;
            inputDir.x = Mathf.Clamp(-mouseMove.x, -moveSpeedOnDrag, moveSpeedOnDrag);
            inputDir.y = Mathf.Clamp(-mouseMove.y, -moveSpeedOnDrag, moveSpeedOnDrag);
            lastMousePosMove = newMousePosMove;
        }
        else {
            inputDir = inputManager.moveDir;

            if (inputDir == Vector2.zero && moveOnEdge && !rotateOnHoldActive && Application.isFocused) {
                Vector2 mousePos = inputManager.mousePos;

                if (mousePos.x < edgeScrollSize) {
                    inputDir.x = -1f;
                }
                else if (mousePos.x > screenRightScroll) {
                    inputDir.x = 1f;
                }

                if (mousePos.y < edgeScrollSize) {
                    inputDir.y = -1f;
                }
                else if (mousePos.y > screenTopScroll) {
                    inputDir.y = 1f;
                }
            }
        }

        if (inputDir == Vector2.zero) {
            if (IsOutsideBorders(target.position)) {
                SmoothClampCameraPosition();
            }
            else {
                return;
            }
        }

        Vector3 moveDir = target.forward * inputDir.y + target.right * inputDir.x;
        Vector3 newPosition = target.position + moveSpeed * Time.deltaTime * moveDir;

        if (IsOutsideBorders(newPosition)) {
            if (IsOutsideBorders(target.position)) {
                newPosition.x = Mathf.Clamp(newPosition.x, Mathf.Min(dynamicPadding, target.position.x), Mathf.Max(mapSize.x - dynamicPadding, target.position.x));
                newPosition.z = Mathf.Clamp(newPosition.z, Mathf.Min(dynamicPadding, target.position.z), Mathf.Max(mapSize.y - dynamicPadding, target.position.z));
                target.position = newPosition;
                SmoothClampCameraPosition();
            }
            else {
                newPosition.x = Mathf.Clamp(newPosition.x, dynamicPadding, mapSize.x - dynamicPadding);
                newPosition.z = Mathf.Clamp(newPosition.z, dynamicPadding, mapSize.y - dynamicPadding);
                target.position = newPosition;
            }
        }
        else {
            target.position = newPosition;
        }
    }

    private void RotateCamera() {
        float rotateDir = 0f;

        if (rotateOnHoldActive) {
            float newMousePosRotate = inputManager.mousePos.x;
            rotateDir = Mathf.Clamp(newMousePosRotate - lastMousePosRotate, -rotateSpeedOnDrag, rotateSpeedOnDrag);
            lastMousePosRotate = newMousePosRotate;
        }
        else {
            rotateDir = inputManager.rotationDir;
        }

        if (rotateDir != 0f) {
            target.Rotate(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
        }
    }

    private void ZoomCamera() {
        float zoomDir = Mathf.Clamp(inputManager.zoomDir, -zoomScrollWheelClamp, zoomScrollWheelClamp);
        zoomAmount -= zoomDir * zoomSpeed * Time.deltaTime;
        zoomAmount = Mathf.Clamp(zoomAmount, zoomRange.x, zoomRange.y);
        followOffset.y = zoomAmount;
        followOffset.z = zoomRotationCurve.Evaluate(zoomAmount);
        cinemachineFollow.FollowOffset = Vector3.Lerp(cinemachineFollow.FollowOffset, followOffset, Time.deltaTime * zoomLerpSpeed);

        if (zoomDir == 0f) return;

        CalculateDynamicPadding();
    }

    private void CalculateDynamicPadding() {
        float paddingScale = Mathf.InverseLerp(zoomRange.x, zoomRange.y, zoomAmount);
        dynamicPadding = mapBorderSmoothPadding * paddingScale + mapBorderHardPadding;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if (mapSize == Vector2.zero) return;

        Gizmos.color = Color.red;
        Vector3 center = new Vector3(mapSize.x / 2f, 0f, mapSize.y / 2f);
        Vector3 size = new Vector3(mapSize.x, 0.1f, mapSize.y);
        Gizmos.DrawWireCube(center, size);
        Gizmos.color = Color.yellow;
        Vector3 paddedSoftSize = new Vector3(mapSize.x - 2 * (mapBorderSmoothPadding + mapBorderHardPadding), 0.1f, mapSize.y - 2 * (mapBorderSmoothPadding + mapBorderHardPadding));
        Gizmos.DrawWireCube(center, paddedSoftSize);
        Gizmos.color = Color.blue;
        Vector3 paddedHardSize = new Vector3(mapSize.x - 2 * mapBorderHardPadding, 0.1f, mapSize.y - 2 * mapBorderHardPadding);
        Gizmos.DrawWireCube(center, paddedHardSize);
        Gizmos.color = Color.cyan;
        Vector3 dynamicPaddedSize = new Vector3(mapSize.x - 2 * dynamicPadding, 0.1f, mapSize.y - 2 * dynamicPadding);
        Gizmos.DrawWireCube(center, dynamicPaddedSize);
    }
#endif
}