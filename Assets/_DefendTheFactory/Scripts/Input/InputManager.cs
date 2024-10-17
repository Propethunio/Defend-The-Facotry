using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {

    public static InputManager Instance;

    public Vector2 mousePos { get; private set; }
    public Vector2 moveDir { get; private set; }
    public float rotationDir { get; private set; }
    public float zoomDir { get; private set; }

    public event Action leftClickPerformed, leftClickCanceled, rightClickPerformed, rightClickCanceled, scrollClickPerformed, scrollClickCanceled;
    public event Action puseAction, timeNormalAction, timeFastAction, timeExtraFastAction;
    public event Action buildingMenuAction, buildingRotationAction;

    InputMap input;

    void Awake() {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        input = new InputMap();
        EnableGameInput();
    }

    public void EnableMenuInput() {
        DisableEvents();
        input.Disable();
        input.MenuInput.Enable();
        SetMenuEvents();
    }

    public void EnableGameInput() {
        DisableEvents();
        input.Disable();
        input.GameInput.Enable();
        SetGameEvents();
    }

    void SetMenuEvents() {

    }

    void SetGameEvents() {
        input.GameInput.CameraMovement.performed += CameraMovement_performed;
        input.GameInput.CameraMovement.canceled += CameraMovement_canceled;
        input.GameInput.CameraRotation.performed += CameraRotation_performed;
        input.GameInput.CameraRotation.canceled += CameraRotation_canceled;
        input.GameInput.CameraZoom.performed += CameraZoom_performed;
        input.GameInput.CameraZoom.canceled += CameraZoom_canceled;
        input.GameInput.PointerPosition.performed += PointerPosition_performed;
        input.GameInput.LeftClick.performed += LeftClick_performed;
        input.GameInput.LeftClick.canceled += LeftClick_canceled;
        input.GameInput.RightClick.performed += RightClick_performed;
        input.GameInput.RightClick.canceled += RightClick_canceled;
        input.GameInput.ScrollClick.performed += ScrollClick_performed;
        input.GameInput.ScrollClick.canceled += ScrollClick_canceled;
    }

    void DisableEvents() {
        input.GameInput.CameraMovement.performed -= CameraMovement_performed;
        input.GameInput.CameraMovement.canceled -= CameraMovement_canceled;
        input.GameInput.CameraRotation.performed -= CameraRotation_performed;
        input.GameInput.CameraRotation.canceled -= CameraRotation_canceled;
        input.GameInput.CameraZoom.performed -= CameraZoom_performed;
        input.GameInput.CameraZoom.canceled -= CameraZoom_canceled;
        input.GameInput.PointerPosition.performed -= PointerPosition_performed;
        input.GameInput.LeftClick.performed -= LeftClick_performed;
        input.GameInput.LeftClick.canceled -= LeftClick_canceled;
        input.GameInput.RightClick.performed -= RightClick_performed;
        input.GameInput.RightClick.canceled -= RightClick_canceled;
        input.GameInput.ScrollClick.performed -= ScrollClick_performed;
        input.GameInput.ScrollClick.canceled -= ScrollClick_canceled;
    }

    void CameraMovement_performed(InputAction.CallbackContext obj) {
        moveDir = obj.ReadValue<Vector2>();
    }

    void CameraMovement_canceled(InputAction.CallbackContext obj) {
        moveDir = Vector2.zero;
    }

    void CameraRotation_performed(InputAction.CallbackContext obj) {
        rotationDir = obj.ReadValue<float>();
    }

    void CameraRotation_canceled(InputAction.CallbackContext obj) {
        rotationDir = 0f;
    }

    void CameraZoom_performed(InputAction.CallbackContext obj) {
        zoomDir = obj.ReadValue<float>();
    }

    void CameraZoom_canceled(InputAction.CallbackContext obj) {
        zoomDir = 0f;
    }

    void PointerPosition_performed(InputAction.CallbackContext obj) {
        mousePos = obj.ReadValue<Vector2>();
    }

    void LeftClick_performed(InputAction.CallbackContext obj) {
        leftClickPerformed?.Invoke();
    }

    void LeftClick_canceled(InputAction.CallbackContext obj) {
        leftClickCanceled?.Invoke();
    }

    void RightClick_performed(InputAction.CallbackContext obj) {
        rightClickPerformed?.Invoke();
    }

    void RightClick_canceled(InputAction.CallbackContext obj) {
        rightClickCanceled?.Invoke();
    }

    void ScrollClick_performed(InputAction.CallbackContext obj) {
        scrollClickPerformed?.Invoke();
    }

    void ScrollClick_canceled(InputAction.CallbackContext obj) {
        scrollClickCanceled?.Invoke();
    }
}