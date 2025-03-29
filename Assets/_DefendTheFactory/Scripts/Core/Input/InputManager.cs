using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    public static InputManager Instance { get; private set; }

    public Vector2 mousePos { get; private set; }
    public Vector2 moveDir { get; private set; }
    public float rotationDir { get; private set; }
    public float zoomDir { get; private set; }

    public event Action mouseMoveAction, leftClickAction, rightClickPerformedAction, rightClickCanceledAction, scrollClickPerformedAction, scrollClickCanceledAction;
    public event Action backClickAction, buildingMenuAction, buildingRotationAction;
    public event Action<int> timeChangeAction;

    private InputMap input;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        input = new InputMap();
        SubscribeEvents();
        EnableGameInput();
    }

    private void Update() {
        if (moveDir != Vector2.zero || rotationDir != 0f || zoomDir != 0f) {
            mouseMoveAction?.Invoke();
        }
    }

    public void EnableMenuInput() {
        input.Disable();
        input.MenuInput.Enable();
    }

    public void EnableGameInput() {
        input.Disable();
        input.GameInput.Enable();
    }

    private void SubscribeEvents() {
        input.GameInput.CameraMovement.performed += CameraMovement_performed;
        input.GameInput.CameraMovement.canceled += CameraMovement_canceled;
        input.GameInput.CameraRotation.performed += CameraRotation_performed;
        input.GameInput.CameraRotation.canceled += CameraRotation_canceled;
        input.GameInput.CameraZoom.performed += CameraZoom_performed;
        input.GameInput.CameraZoom.canceled += CameraZoom_canceled;
        input.GameInput.PointerPosition.performed += PointerPosition_performed;
        input.GameInput.LeftClick.performed += LeftClick_performed;
        input.GameInput.RightClick.performed += RightClick_performed;
        input.GameInput.RightClick.canceled += RightClick_canceled;
        input.GameInput.ScrollClick.performed += ScrollClick_performed;
        input.GameInput.ScrollClick.canceled += ScrollClick_canceled;
        input.GameInput.Pouse.performed += Pause_performed;
        input.GameInput.TimeNormal.performed += TimeNormal_performed;
        input.GameInput.TimeFast.performed += TimeFast_performed;
        input.GameInput.TimeExtraFast.performed += TimeExtraFast_performed;
        input.GameInput.Back.performed += Back_performed;
        input.GameInput.BuildingMenu.performed += BuildingMenu_performed;
        input.GameInput.BuildingRotation.performed += BuildingRotation_performed;
    }

    private void CameraMovement_performed(InputAction.CallbackContext obj) {
        moveDir = obj.ReadValue<Vector2>();
    }

    private void CameraMovement_canceled(InputAction.CallbackContext obj) {
        moveDir = Vector2.zero;
    }

    private void CameraRotation_performed(InputAction.CallbackContext obj) {
        rotationDir = obj.ReadValue<float>();
    }

    private void CameraRotation_canceled(InputAction.CallbackContext obj) {
        rotationDir = 0f;
    }

    private void CameraZoom_performed(InputAction.CallbackContext obj) {
        zoomDir = obj.ReadValue<float>();
    }

    private void CameraZoom_canceled(InputAction.CallbackContext obj) {
        zoomDir = 0f;
    }

    private void PointerPosition_performed(InputAction.CallbackContext obj) {
        mousePos = obj.ReadValue<Vector2>();
        mouseMoveAction?.Invoke();
    }

    private void LeftClick_performed(InputAction.CallbackContext obj) {
        leftClickAction?.Invoke();
    }

    private void RightClick_performed(InputAction.CallbackContext obj) {
        rightClickPerformedAction?.Invoke();
    }

    private void RightClick_canceled(InputAction.CallbackContext obj) {
        rightClickCanceledAction?.Invoke();
    }

    private void ScrollClick_performed(InputAction.CallbackContext obj) {
        scrollClickPerformedAction?.Invoke();
    }

    private void ScrollClick_canceled(InputAction.CallbackContext obj) {
        scrollClickCanceledAction?.Invoke();
    }

    private void Pause_performed(InputAction.CallbackContext obj) {
        timeChangeAction?.Invoke(0);
    }

    private void TimeNormal_performed(InputAction.CallbackContext obj) {
        timeChangeAction?.Invoke(1);
    }

    private void TimeFast_performed(InputAction.CallbackContext obj) {
        timeChangeAction?.Invoke(2);
    }

    private void TimeExtraFast_performed(InputAction.CallbackContext obj) {
        timeChangeAction?.Invoke(3);
    }

    private void Back_performed(InputAction.CallbackContext obj) {
        backClickAction?.Invoke();
    }

    private void BuildingMenu_performed(InputAction.CallbackContext obj) {
        buildingMenuAction?.Invoke();
    }

    private void BuildingRotation_performed(InputAction.CallbackContext obj) {
        buildingRotationAction?.Invoke();
    }
}