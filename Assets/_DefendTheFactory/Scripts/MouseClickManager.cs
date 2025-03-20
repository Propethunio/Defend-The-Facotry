using UnityEngine;
using UtilsClass;

public class MouseClickManager {
    private bool isBuildingSystemEnabled;
    private Camera cam;
    private InputManager input;

    public MouseClickManager() {
        cam = Camera.main;
        input = InputManager.Instance;
        Subscribe();
    }

    ~MouseClickManager() {
        Unsubscribe();
    }

    private void Subscribe() {
        BuildingSystem buildingSystem = BuildingSystem.Instance;
        buildingSystem.OnSystemEnabled += BuildingSystemEnabled;
        buildingSystem.OnSystemDisabled += BuildingSystemDisabled;
        input.leftClickAction += HandleLeftClickAction;
    }

    private void Unsubscribe() {
        BuildingSystem buildingSystem = BuildingSystem.Instance;
        buildingSystem.OnSystemEnabled -= BuildingSystemEnabled;
        buildingSystem.OnSystemDisabled -= BuildingSystemDisabled;
        input.leftClickAction -= HandleLeftClickAction;
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

        hit.transform.GetComponent<BasePlacedObject>().MouseLeftClickObject();
    }
}