using System.Collections;
using UnityEngine;
using UtilsClass;

public class MouseInteractionManager {
	private bool isBuildingSystemEnabled;
	private Camera cam;
	private InputManager input;
	private IReactOnMouse lastHoveredObject;
	private bool restrictClickOnResource;
	private float lastResourceClickTime = -Mathf.Infinity;

	private const float RESOURCE_CLICK_COOLDOWN = 0.5f;

	public MouseInteractionManager() {
		cam = Camera.main;
		input = Injector.Resolve<InputManager>();
		Subscribe();
	}

	~MouseInteractionManager() {
		Unsubscribe();
	}

	private void Subscribe() {
		BuildingSystem buildingSystem = Injector.Resolve<BuildingSystem>();
		buildingSystem.OnSystemEnabled += BuildingSystemEnabled;
		buildingSystem.OnSystemDisabled += BuildingSystemDisabled;
		input.LeftClickAction += HandleLeftClickAction;
		input.MouseMoveAction += HandleMouseHover;
	}

	private void Unsubscribe() {
		BuildingSystem buildingSystem = Injector.Resolve<BuildingSystem>();
		buildingSystem.OnSystemEnabled -= BuildingSystemEnabled;
		buildingSystem.OnSystemDisabled -= BuildingSystemDisabled;
		input.LeftClickAction -= HandleLeftClickAction;
		input.MouseMoveAction -= HandleMouseHover;
	}

	private void BuildingSystemEnabled() {
		isBuildingSystemEnabled = true;
	}

	private void BuildingSystemDisabled() {
		isBuildingSystemEnabled = false;
	}

	private void HandleLeftClickAction() {
		if (isBuildingSystemEnabled || MyUtils.IsPointerOverUI()) return;

		Ray ray = cam.ScreenPointToRay(input.mousePos);

		if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 6)) return;

		IReactOnMouse mouseClickObject = hit.transform.GetComponent<IReactOnMouse>();

		if (mouseClickObject is ResourceNode) {
			HandleResourceClick(mouseClickObject);
		}
		else {
			mouseClickObject.MouseLeftClickObject();
		}
	}

	private void HandleResourceClick(IReactOnMouse mouseClickObject) {
		if (Time.time - lastResourceClickTime < RESOURCE_CLICK_COOLDOWN) return;

		mouseClickObject.MouseLeftClickObject();
		lastResourceClickTime = Time.time;
	}

	private void HandleMouseHover() {
		if (isBuildingSystemEnabled || MyUtils.IsPointerOverUI()) {
			if (lastHoveredObject == null) return;

			lastHoveredObject.MouseExitObject();
			lastHoveredObject = null;
			return;
		}

		Ray ray = cam.ScreenPointToRay(input.mousePos);

		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 6)) {
			IReactOnMouse hoveredObject = hit.transform.GetComponent<IReactOnMouse>();

			if (lastHoveredObject == hoveredObject) return;

			lastHoveredObject?.MouseExitObject();

			if (!hoveredObject.ShouldHighlight()) return;

			hoveredObject.MouseEnterObject();
			lastHoveredObject = hoveredObject;
		}
		else if (lastHoveredObject != null) {
			lastHoveredObject.MouseExitObject();
			lastHoveredObject = null;
		}
	}
}