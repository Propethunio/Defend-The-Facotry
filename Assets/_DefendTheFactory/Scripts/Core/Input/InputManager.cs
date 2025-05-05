using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : DependencyMonoBehaviour<InputManager> {
	public Vector2 mousePos { get; private set; }
	public Vector2 moveDir { get; private set; }
	public float rotationDir { get; private set; }
	public float zoomDir { get; private set; }

	public event Action MouseMoveAction, LeftClickAction, RightClickPerformedAction, RightClickCanceledAction, ScrollClickPerformedAction, ScrollClickCanceledAction;
	public event Action OpenMenuAction, BuildingMenuAction, BuildingRotationAction;
	public event Action<int> HotbarAction, TimeChangeAction;

	private Stack<Action> backStackActions = new Stack<Action>();
	private HealthManager healthManager;
	private InputMap input;

	private void Start() {
		healthManager = Injector.Resolve<HealthManager>();
		input = new InputMap();
		Subscribe();
		EnableGameInput();
	}

	private void Update() {
		if (moveDir != Vector2.zero || rotationDir != 0f || zoomDir != 0f) {
			MouseMoveAction?.Invoke();
		}
	}

	private void OnDestroy() {
		Unsubscribe();
	}

	private void EnableGameInput() {
		input.Disable();
		input.GameInput.Enable();
	}

	private void DisableGameInput() {
		input.Disable();
	}

	private void Subscribe() {
		healthManager.GameOver += OnGameOver;
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
		input.GameInput.Pause.performed += Pause_performed;
		input.GameInput.TimeNormal.performed += TimeNormal_performed;
		input.GameInput.TimeFast.performed += TimeFast_performed;
		input.GameInput.TimeExtraFast.performed += TimeExtraFast_performed;
		input.GameInput.Back.performed += Back_performed;
		input.GameInput.BuildingMenu.performed += BuildingMenu_performed;
		input.GameInput.BuildingRotation.performed += BuildingRotation_performed;
		input.GameInput.Hotbar1.performed += Hotbar1_performed;
		input.GameInput.Hotbar2.performed += Hotbar2_performed;
		input.GameInput.Hotbar3.performed += Hotbar3_performed;
		input.GameInput.Hotbar4.performed += Hotbar4_performed;
		input.GameInput.Hotbar5.performed += Hotbar5_performed;
		input.GameInput.Hotbar6.performed += Hotbar6_performed;
		input.GameInput.Hotbar7.performed += Hotbar7_performed;
		input.GameInput.Hotbar8.performed += Hotbar8_performed;
	}

	private void Unsubscribe() {
		healthManager.GameOver -= OnGameOver;
		input.GameInput.CameraMovement.performed -= CameraMovement_performed;
		input.GameInput.CameraMovement.canceled -= CameraMovement_canceled;
		input.GameInput.CameraRotation.performed -= CameraRotation_performed;
		input.GameInput.CameraRotation.canceled -= CameraRotation_canceled;
		input.GameInput.CameraZoom.performed -= CameraZoom_performed;
		input.GameInput.CameraZoom.canceled -= CameraZoom_canceled;
		input.GameInput.PointerPosition.performed -= PointerPosition_performed;
		input.GameInput.LeftClick.performed -= LeftClick_performed;
		input.GameInput.RightClick.performed -= RightClick_performed;
		input.GameInput.RightClick.canceled -= RightClick_canceled;
		input.GameInput.ScrollClick.performed -= ScrollClick_performed;
		input.GameInput.ScrollClick.canceled -= ScrollClick_canceled;
		input.GameInput.Pause.performed -= Pause_performed;
		input.GameInput.TimeNormal.performed -= TimeNormal_performed;
		input.GameInput.TimeFast.performed -= TimeFast_performed;
		input.GameInput.TimeExtraFast.performed -= TimeExtraFast_performed;
		input.GameInput.Back.performed -= Back_performed;
		input.GameInput.BuildingMenu.performed -= BuildingMenu_performed;
		input.GameInput.BuildingRotation.performed -= BuildingRotation_performed;
		input.GameInput.Hotbar1.performed -= Hotbar1_performed;
		input.GameInput.Hotbar2.performed -= Hotbar2_performed;
		input.GameInput.Hotbar3.performed -= Hotbar3_performed;
		input.GameInput.Hotbar4.performed -= Hotbar4_performed;
		input.GameInput.Hotbar5.performed -= Hotbar5_performed;
		input.GameInput.Hotbar6.performed -= Hotbar6_performed;
		input.GameInput.Hotbar7.performed -= Hotbar7_performed;
		input.GameInput.Hotbar8.performed -= Hotbar8_performed;
	}

	private void OnGameOver() {
		HandleResetBackState();
		DisableGameInput();
	}
	
	public void RegisterBackAction(Action action) {
		backStackActions.Push(action);
	}

	public void UnregisterBackAction(Action action) {
		Stack<Action> tempStack = new Stack<Action>();

		while (backStackActions.Count > 0) {
			Action current = backStackActions.Pop();
			if (current != action) {
				tempStack.Push(current);
			}
		}

		while (tempStack.Count > 0) {
			backStackActions.Push(tempStack.Pop());
		}
	}

	public void HandleResetBackState() {
		if (backStackActions.Count == 0) return;
		
		HandleBack();
	}
	
	private void HandleBack() {
		if (backStackActions.Count > 0) {
			backStackActions.Pop()?.Invoke();
		}
		else {
			OpenMenuAction?.Invoke();
		}
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
		MouseMoveAction?.Invoke();
	}

	private void LeftClick_performed(InputAction.CallbackContext obj) {
		LeftClickAction?.Invoke();
	}

	private void RightClick_performed(InputAction.CallbackContext obj) {
		RightClickPerformedAction?.Invoke();
	}

	private void RightClick_canceled(InputAction.CallbackContext obj) {
		RightClickCanceledAction?.Invoke();
	}

	private void ScrollClick_performed(InputAction.CallbackContext obj) {
		ScrollClickPerformedAction?.Invoke();
	}

	private void ScrollClick_canceled(InputAction.CallbackContext obj) {
		ScrollClickCanceledAction?.Invoke();
	}

	private void Pause_performed(InputAction.CallbackContext obj) {
		TimeChangeAction?.Invoke(0);
	}

	private void TimeNormal_performed(InputAction.CallbackContext obj) {
		TimeChangeAction?.Invoke(1);
	}

	private void TimeFast_performed(InputAction.CallbackContext obj) {
		TimeChangeAction?.Invoke(2);
	}

	private void TimeExtraFast_performed(InputAction.CallbackContext obj) {
		TimeChangeAction?.Invoke(3);
	}

	private void Back_performed(InputAction.CallbackContext obj) {
		HandleBack();
	}

	private void BuildingMenu_performed(InputAction.CallbackContext obj) {
		BuildingMenuAction?.Invoke();
	}

	private void BuildingRotation_performed(InputAction.CallbackContext obj) {
		BuildingRotationAction?.Invoke();
	}

	private void Hotbar1_performed(InputAction.CallbackContext obj) {
		HotbarAction?.Invoke(0);
	}

	private void Hotbar2_performed(InputAction.CallbackContext obj) {
		HotbarAction?.Invoke(1);
	}

	private void Hotbar3_performed(InputAction.CallbackContext obj) {
		HotbarAction?.Invoke(2);
	}

	private void Hotbar4_performed(InputAction.CallbackContext obj) {
		HotbarAction?.Invoke(3);
	}

	private void Hotbar5_performed(InputAction.CallbackContext obj) {
		HotbarAction?.Invoke(4);
	}

	private void Hotbar6_performed(InputAction.CallbackContext obj) {
		HotbarAction?.Invoke(5);
	}

	private void Hotbar7_performed(InputAction.CallbackContext obj) {
		HotbarAction?.Invoke(6);
	}

	private void Hotbar8_performed(InputAction.CallbackContext obj) {
		HotbarAction?.Invoke(7);
	}
}