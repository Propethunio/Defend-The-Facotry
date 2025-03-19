using System;
using UnityEngine;
using UtilsClass;

public class MouseClickManager : MonoBehaviour {
    private bool isBuildingSystemEnabled;
    private Camera cam;

    private void Start() {
        cam = Camera.main;
        Subscribe();
    }

    private void OnDestroy() {
        Unsubscribe();
    }

    private void Subscribe() {
        BuildingSystem buildingSystem = BuildingSystem.Instance;
        buildingSystem.OnSystemEnabled += BuildingSystemEnabled;
        buildingSystem.OnSystemDisabled += BuildingSystemDisabled;
        InputManager.Instance.leftClickAction += HandleLeftClickAction;
    }

    private void Unsubscribe() {
        BuildingSystem buildingSystem = BuildingSystem.Instance;
        buildingSystem.OnSystemEnabled -= BuildingSystemEnabled;
        buildingSystem.OnSystemDisabled -= BuildingSystemDisabled;
        InputManager.Instance.leftClickAction -= HandleLeftClickAction;
    }

    private void BuildingSystemEnabled() {
        isBuildingSystemEnabled = true;
    }

    private void BuildingSystemDisabled() {
        isBuildingSystemEnabled = false;
    }

    private void HandleLeftClickAction() {
        if(isBuildingSystemEnabled || MyUtils.IsPointerOverUI()) return;

        Ray ray = cam.ScreenPointToRay(InputManager.Instance.mousePos);
        //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        
        Physics.Raycast(ray, out RaycastHit hit);

        if (hit.collider != null) {
            
        }
        
        ResourceNode node = new ResourceNode();
        
        node.ClickResource();
    }
}