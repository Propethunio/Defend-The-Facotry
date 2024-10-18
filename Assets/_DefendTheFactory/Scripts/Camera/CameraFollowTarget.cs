using Cinemachine;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour {

    [SerializeField] CinemachineVirtualCamera cinemachineCamera;
    [SerializeField] bool moveOnEdge;
    [SerializeField] int edgeScrollSize;
    [SerializeField] float moveSpeed;
    [SerializeField] float moveSpeedOnDrag;
    [SerializeField] float rotateSpeed;
    [SerializeField] float rotateSpeedOnDrag;
    [SerializeField] float zoomSpeed;
    [SerializeField] float zoomScrollWheelClamp;
    [SerializeField] float zoomLerpSpeed;
    [SerializeField] Vector2 zoomRange;
    [SerializeField] AnimationCurve zoomRotationCurve;

    Transform target;
    InputManager inputManager;
    CinemachineTransposer transposer;
    Vector3 followOffset;
    Vector2 lastMousePosMove;
    float lastMousePosRotate;
    float screenRightScroll;
    float screenTopScroll;
    float zoomAmount;
    bool moveOnHoldActive;
    bool rotateOnHoldActive;

    void Start() {
        target = transform;
        inputManager = InputManager.Instance;
        transposer = cinemachineCamera.GetCinemachineComponent<CinemachineTransposer>();
        followOffset = transposer.m_FollowOffset;
        zoomAmount = followOffset.y;
        followOffset.z = zoomRotationCurve.Evaluate(zoomAmount);
        transposer.m_FollowOffset = followOffset;
        CalculateScrollBounds();
        ObserveEvents();
    }

    void Update() {
        MoveCamera();
        RotateCamera();
        ZoomCamera();
    }

    void CalculateScrollBounds() {
        screenRightScroll = Screen.width - edgeScrollSize;
        screenTopScroll = Screen.height - edgeScrollSize;
    }

    void ObserveEvents() {
        inputManager.rightClickPerformed += InputManager_rightClickPerformed;
        inputManager.rightClickCanceled += InputManager_rightClickCanceled;
        inputManager.scrollClickPerformed += InputManager_scrollClickPerformed;
        inputManager.scrollClickCanceled += InputManager_scrollClickCanceled;
    }

    void InputManager_rightClickPerformed() {
        if(rotateOnHoldActive) return;
        moveOnHoldActive = true;
        lastMousePosMove = inputManager.mousePos;
    }

    void InputManager_rightClickCanceled() {
        moveOnHoldActive = false;
    }

    void InputManager_scrollClickPerformed() {
        if(moveOnHoldActive) return;
        rotateOnHoldActive = true;
        lastMousePosRotate = inputManager.mousePos.x;
    }

    void InputManager_scrollClickCanceled() {
        rotateOnHoldActive = false;
    }

    void MoveCamera() {
        Vector2 inputDir = Vector2.zero;

        if(moveOnHoldActive) {
            Vector2 newMousePosMove = inputManager.mousePos;
            Vector2 mouseMove = newMousePosMove - lastMousePosMove;
            inputDir.x = Mathf.Clamp(-mouseMove.x, -moveSpeedOnDrag, moveSpeedOnDrag);
            inputDir.y = Mathf.Clamp(-mouseMove.y, -moveSpeedOnDrag, moveSpeedOnDrag);

            lastMousePosMove = newMousePosMove;
        } else {
            inputDir = inputManager.moveDir;

            if(inputDir == Vector2.zero && moveOnEdge && !rotateOnHoldActive && Application.isFocused) {
                Vector2 mousePos = inputManager.mousePos;
                if(mousePos.x < edgeScrollSize) {
                    inputDir.x = -1f;
                } else if(mousePos.x > screenRightScroll) {
                    inputDir.x = 1f;
                }

                if(mousePos.y < edgeScrollSize) {
                    inputDir.y = -1f;
                } else if(mousePos.y > screenTopScroll) {
                    inputDir.y = 1f;
                }
            }
        }

        if(inputDir != Vector2.zero) {
            Vector3 moveDir = target.forward * inputDir.y + target.right * inputDir.x;
            target.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }

    void RotateCamera() {
        float rotateDir = 0f;

        if(rotateOnHoldActive) {
            float newMousePosRotate = inputManager.mousePos.x;
            rotateDir = Mathf.Clamp(newMousePosRotate - lastMousePosRotate, -rotateSpeedOnDrag, rotateSpeedOnDrag);
            lastMousePosRotate = newMousePosRotate;
        } else {
            rotateDir = inputManager.rotationDir;
        }

        if(rotateDir != 0f) {
            target.Rotate(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
        }
    }

    void ZoomCamera() {
        float zoomDir = Mathf.Clamp(inputManager.zoomDir, -zoomScrollWheelClamp, zoomScrollWheelClamp);
        zoomAmount -= zoomDir * zoomSpeed * Time.deltaTime;
        zoomAmount = Mathf.Clamp(zoomAmount, zoomRange.x, zoomRange.y);
        followOffset.y = zoomAmount;
        followOffset.z = zoomRotationCurve.Evaluate(zoomAmount);
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, followOffset, Time.deltaTime * zoomLerpSpeed);
    }
}