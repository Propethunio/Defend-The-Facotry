using System;
using System.Collections.Generic;
using UnityEngine;
using UtilsClass;

public class BuildingSystem {

    public static BuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    [SerializeField] PlacedObjectTypeSO ghostBelt;

    public Grid<GridCell> grid { get; private set; }

    PlacedObjectTypeSO placedObjectTypeSO;
    PlacedObjectTypeSO.Dir dir;
    InputManager inputManager;
    bool isBuildingSystemActive;
    bool isDemolishActive;

    public BuildingSystem(int width, int height) {
        if(Instance == null) Instance = this;
        else return;
        grid = new Grid<GridCell>(width, height, (Grid<GridCell> g, int x, int y) => new GridCell(x, y));
        inputManager = InputManager.Instance;
    }

    public void Test(PlacedObjectTypeSO test) {
        EnableBuildingSystem();
        SetSelectedPlacedObject(test);
    }

    void EnableBuildingSystem() {
        if(isBuildingSystemActive) return;

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

    private void HandleBeltPlacement() {
        // Placing a belt, place in a line
        if(Input.GetMouseButton(0) && placedObjectTypeSO == GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt && !MyUtils.IsPointerOverUI()) {
            if(!Mouse3D.TryGetMouseWorldPosition(out Vector3 mousePosition)) return;

            int x = Mathf.FloorToInt(mousePosition.x);
            int z = Mathf.FloorToInt(mousePosition.z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            //TryPlaceObject(placedObjectOrigin, placedObjectTypeSO, dir);

            // ###### TODO: PLACE BELTS IN A LINE
            /*
            if (beltPlaceStartGridPosition == null) {
                // First stage, select start point
                beltPlaceStartGridPosition = placedObjectOrigin;
            } else {
                // Second stage, place all belts
                Vector2Int beltPlaceEndGridPosition = placedObjectOrigin;
                for (int beltX = beltPlaceStartGridPosition.Value.x; beltX < beltPlaceEndGridPosition.x; beltX++) {
                    TryPlaceObject(beltX, beltPlaceStartGridPosition.Value.y, placedObjectTypeSO, PlacedObjectTypeSO.Dir.Right);
                }
                for (int beltY = beltPlaceStartGridPosition.Value.y; beltY < beltPlaceEndGridPosition.y; beltY++) {
                    TryPlaceObject(beltPlaceEndGridPosition.x, beltY, placedObjectTypeSO, PlacedObjectTypeSO.Dir.Up);

                }
            }
            */
        }
    }

    void HandleDirRotation() {
        dir = PlacedObjectTypeSO.GetNextDir(dir);
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

        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
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
        OnObjectPlaced?.Invoke(placedObject, EventArgs.Empty);
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
        if(!Mouse3D.TryGetMouseWorldPosition(out Vector3 mousePosition)) return Vector3.zero;

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
        if(placedObjectTypeSO != null) {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        } else {
            return Quaternion.identity;
        }
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
}