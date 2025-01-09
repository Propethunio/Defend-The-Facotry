using System;
using System.Collections.Generic;
using UnityEngine;
using UtilsClass;

public class BuildingSystem {

    public static BuildingSystem Instance { get; private set; }

    public Action OnSelectedObject;
    public Action OnBuildCanceled;
    public Action OnObjectPlaced;

    public Grid<GridCell> grid { get; private set; }
    public PlacedObjectTypeSO placedObjectTypeSO { get; private set; }
    public BuildingDir dir { get; private set; }

    InputManager inputManager;
    bool isBuildingSystemActive;
    bool isDemolishActive;
    BuildingGhost buildingGhost;

    public BuildingSystem(int width, int height) {
        if(Instance == null) Instance = this;
        else return;
        grid = new Grid<GridCell>(width, height, (Grid<GridCell> g, int x, int y) => new GridCell(x, y));
        inputManager = InputManager.Instance;
        buildingGhost = BuildingGhost.Instance;
        buildingGhost.Init();
    }

    public void Test(PlacedObjectTypeSO test) {
        EnableBuildingSystem();
        SetSelectedPlacedObject(test);
    }

    void EnableBuildingSystem() {
        if(isBuildingSystemActive) return;

        dir = BuildingDir.Down;
        isBuildingSystemActive = true;
        TilemapVisual.Instance.Show();
        Subscribe();
    }

    void DisableBuildingSystem() {
        if(!isBuildingSystemActive) return;

        placedObjectTypeSO = null;
        isBuildingSystemActive = false;
        isDemolishActive = false;
        TilemapVisual.Instance.Hide();
        OnBuildCanceled?.Invoke();
        Unsubscribe();
    }

    void Subscribe() {
        inputManager.leftClickAction += HandleObjectPlacement;
        inputManager.buildingRotationAction += HandleDirRotation;
        inputManager.rightClickPerformedAction += DisableBuildingSystem;
        inputManager.backClickAction += DisableBuildingSystem;
    }

    void Unsubscribe() {
        inputManager.leftClickAction -= HandleObjectPlacement;
        inputManager.buildingRotationAction -= HandleDirRotation;
        inputManager.rightClickPerformedAction -= DisableBuildingSystem;
        inputManager.backClickAction -= DisableBuildingSystem;
    }

    void HandleObjectPlacement() {
        if(!MyUtils.IsPointerOverUI() && Mouse3D.TryGetMouseWorldPosition(out Vector3 mousePosition)) {
            int x = Mathf.FloorToInt(mousePosition.x);
            int z = Mathf.FloorToInt(mousePosition.z);
            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            TryPlaceObject(placedObjectOrigin);
        }
    }

    void HandleDirRotation() {
        dir = GetNextDir(dir);
    }

    private void HandleDemolish() {
        if(isDemolishActive && Input.GetMouseButtonDown(0) && !MyUtils.IsPointerOverUI()) {
            if(!Mouse3D.TryGetMouseWorldPosition(out Vector3 mousePosition)) return;

            int x = Mathf.FloorToInt(mousePosition.x);
            int z = Mathf.FloorToInt(mousePosition.z);

            PlacedObject placedObject = grid.gridArray[x, z].placedObject;
            if(placedObject != null) {
                // Demolish
                placedObject.DestroySelf();

                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                foreach(Vector2Int gridPosition in gridPositionList) {
                    grid.gridArray[gridPosition.x, gridPosition.y].ClearPlacedObject();
                }
            }
        }
    }

    private void UpdateCanBuildTilemap() {
        for(int x = 0; x < grid.width; x++) {
            for(int y = 0; y < grid.height; y++) {
                // Tilemap
                TilemapVisual.Instance.SetTilemapSprite(new Vector3(x, y),
                    grid.gridArray[x, y].placedObject == null ?
                    TilemapSprite.CanBuild :
                    TilemapSprite.CannotBuild);
            }
        }
    }

    public void DeselectObjectType() {
        placedObjectTypeSO = null;

        isDemolishActive = false;
        RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType() {
        UpdateCanBuildTilemap();

        if(placedObjectTypeSO == null) {
            TilemapVisual.Instance.Hide();
        } else {
            TilemapVisual.Instance.Show();
        }

        OnSelectedObject?.Invoke();
    }

    bool TryPlaceObject(Vector2Int placedObjectOrigin) {

        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);

        foreach(Vector2Int gridPosition in gridPositionList) {
            GridCell cell = grid.gridArray[gridPosition.x, gridPosition.y];
            if(cell == null || cell.placedObject != null) {
                return false;
            }
        }

        Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
        Vector3 placedObjectWorldPosition = new Vector3(placedObjectOrigin.x, 0, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y);
        PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);

        foreach(Vector2Int gridPosition in gridPositionList) {
            grid.gridArray[gridPosition.x, gridPosition.y].SetPlacedObject(placedObject);
        }

        placedObject.GridSetupDone();
        OnObjectPlaced?.Invoke();
        return true;
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        int x = Mathf.FloorToInt(worldPosition.x);
        int z = Mathf.FloorToInt(worldPosition.z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition) {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }

    public GridCell GetGridObject(Vector2Int gridPosition) {
        return grid.gridArray[gridPosition.x, gridPosition.y];
    }

    public GridCell GetGridObject(Vector3 worldPosition) {

        int x = Mathf.FloorToInt(worldPosition.x);
        int z = Mathf.FloorToInt(worldPosition.z);
        return grid.gridArray[x, z];
    }

    public bool IsValidGridPosition(Vector2Int gridPosition) {
        return grid.gridArray[gridPosition.x, gridPosition.y] != null;
    }

    public Vector3 GetMouseWorldSnappedPosition() {
        if(!Mouse3D.TryGetMouseWorldPosition(out Vector3 mousePosition)) return Vector3.back;

        int x = Mathf.FloorToInt(mousePosition.x);
        int z = Mathf.FloorToInt(mousePosition.z);

        if(placedObjectTypeSO != null) {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = new Vector3(x, 0, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y);
            return placedObjectWorldPosition;
        } else {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation() {
        if(placedObjectTypeSO == null) return Quaternion.identity;

        return Quaternion.Euler(0, GetRotationAngle(dir), 0);
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() {
        return placedObjectTypeSO;
    }

    public void SetSelectedPlacedObject(PlacedObjectTypeSO placedObjectTypeSO) {
        this.placedObjectTypeSO = placedObjectTypeSO;
        isDemolishActive = false;
        RefreshSelectedObjectType();
    }

    public void SetDemolishActive() {
        placedObjectTypeSO = null;
        isDemolishActive = true;
        RefreshSelectedObjectType();
    }

    public bool IsDemolishActive() {
        return isDemolishActive;
    }

    public void AddGhostBeltToGrid(Vector2Int beltPosition, PlacedObject belt) {
        grid.gridArray[beltPosition.x, beltPosition.y].SetPlacedObject(belt);
    }

    public BuildingDir GetNextDir(BuildingDir dir) {
        switch(dir) {
            default:
            case BuildingDir.Down: return BuildingDir.Left;
            case BuildingDir.Left: return BuildingDir.Up;
            case BuildingDir.Up: return BuildingDir.Right;
            case BuildingDir.Right: return BuildingDir.Down;
        }
    }

    public Vector2Int GetDirForwardVector(BuildingDir dir) {
        switch(dir) {
            default:
            case BuildingDir.Down: return new Vector2Int(0, -1);
            case BuildingDir.Left: return new Vector2Int(-1, 0);
            case BuildingDir.Up: return new Vector2Int(0, +1);
            case BuildingDir.Right: return new Vector2Int(+1, 0);
        }
    }

    public int GetRotationAngle(BuildingDir dir) {
        switch(dir) {
            default:
            case BuildingDir.Down: return 0;
            case BuildingDir.Left: return 90;
            case BuildingDir.Up: return 180;
            case BuildingDir.Right: return 270;
        }
    }
}