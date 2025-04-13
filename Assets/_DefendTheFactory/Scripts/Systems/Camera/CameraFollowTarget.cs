using Unity.Cinemachine;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour {
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private bool moveOnEdge;
    [SerializeField] private int edgeScrollSize;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveSpeedOnDrag;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float rotateSpeedOnDrag;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float zoomScrollWheelClamp;
    [SerializeField] private float zoomLerpSpeed;
    [SerializeField] private Vector2 zoomRange;
    [SerializeField] private AnimationCurve zoomRotationCurve;

    private Transform target;
    private InputManager inputManager;
    private CinemachineFollow cinemachineFollow;
    private Vector3 followOffset;
    private Vector2 lastMousePosMove;
    private float lastMousePosRotate;
    private float screenRightScroll;
    private float screenTopScroll;
    private float zoomAmount;
    private bool moveOnHoldActive;
    private bool rotateOnHoldActive;

    private void Start() {
        target = transform;
        inputManager = Injector.Resolve<InputManager>();
        cinemachineFollow = cinemachineCamera.GetComponent<CinemachineFollow>();
        followOffset = cinemachineFollow.FollowOffset;
        zoomAmount = followOffset.y;
        followOffset.z = zoomRotationCurve.Evaluate(zoomAmount);
        cinemachineFollow.FollowOffset = followOffset;
        CalculateScrollBounds();
        SubscribeEvents();
    }

    private void Update() {
        MoveCamera();
        RotateCamera();
        ZoomCamera();
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

        if (inputDir == Vector2.zero) return;

        Vector3 moveDir = target.forward * inputDir.y + target.right * inputDir.x;
        target.position += moveSpeed * Time.deltaTime * moveDir;
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
    }
}