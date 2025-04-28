using UnityEngine;
using UtilsClass;

public class MouseInteractionManager {
    private bool isBuildingSystemEnabled;
    private Camera cam;
    private InputManager input;
    private IReactOnMouse lastHoveredObject;

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

        hit.transform.GetComponent<IReactOnMouse>().MouseLeftClickObject();
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