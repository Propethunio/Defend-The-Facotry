using System;
using System.Collections.Generic;
using UnityEngine;
using UtilsClass;

public class BuildingSystem {

    public static BuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    public Grid<GridCell> grid { get; private set; }
    PlacedObjectTypeSO placedObjectTypeSO;
    PlacedObjectTypeSO.Dir dir;

    bool isDemolishActive;

    public BuildingSystem(int width, int height) {
        Instance = this;
        grid = new Grid<GridCell>(width, height, (Grid<GridCell> g, int x, int y) => new GridCell(x, y));
    }

    void Update() {
        if(placedObjectTypeSO == GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt) {
            // Placing a belt, place in a line
            HandleBeltPlacement();
        } else {
            HandleNormalObjectPlacement();
        }
        HandleDirRotation();
        HandleDemolish();

        if(Input.GetMouseButtonDown(1)) {
            DeselectObjectType();
        }
    }

    void HandleNormalObjectPlacement() {
        if(Input.GetMouseButtonDown(0) && placedObjectTypeSO != null && !MyUtils.IsPointerOverUI()) {
            if(!Mouse3D.TryGetMouseWorldPosition(out Vector3 mousePosition)) return;

            grid.GetXZ(mousePosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            if(TryPlaceObject(placedObjectOrigin, placedObjectTypeSO, dir, out PlacedObject placedObject)) {
                // Object placed
            } else {
                // Error!
                //UtilsClass.CreateWorldTextPopup("Cannot Build Here!", mousePosition);
            }
        }
    }

    private void HandleBeltPlacement() {
        // Placing a belt, place in a line
        if(Input.GetMouseButton(0) && placedObjectTypeSO == GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt && !MyUtils.IsPointerOverUI()) {
            if(!Mouse3D.TryGetMouseWorldPosition(out Vector3 mousePosition)) return;

            grid.GetXZ(mousePosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            TryPlaceObject(placedObjectOrigin, placedObjectTypeSO, dir, out PlacedObject placedObject);

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

    private void HandleDirRotation() {
        if(Input.GetKeyDown(KeyCode.R)) {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }
    }

    private void HandleDemolish() {
        if(isDemolishActive && Input.GetMouseButtonDown(0) && !MyUtils.IsPointerOverUI()) {
            if(!Mouse3D.TryGetMouseWorldPosition(out Vector3 mousePosition)) return;
            PlacedObject placedObject = grid.GetGridObject(mousePosition).placedObject;
            if(placedObject != null) {
                // Demolish
                placedObject.DestroySelf();

                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                foreach(Vector2Int gridPosition in gridPositionList) {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }
            }
        }
    }

    private void UpdateCanBuildTilemap() {
        for(int x = 0; x < grid.width; x++) {
            for(int y = 0; y < grid.height; y++) {
                // Tilemap
                TilemapVisual.Instance.SetTilemapSprite(new Vector3(x, y),
                    grid.GetGridObject(x, y).placedObject == null ?
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

    public bool TryPlaceObject(int x, int y, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir) {
        return TryPlaceObject(new Vector2Int(x, y), placedObjectTypeSO, dir, out PlacedObject placedObject);
    }

    public bool TryPlaceObject(Vector2Int placedObjectOrigin, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir) {
        return TryPlaceObject(placedObjectOrigin, placedObjectTypeSO, dir, out PlacedObject placedObject);
    }

    public bool TryPlaceObject(Vector2Int placedObjectOrigin, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir, out PlacedObject placedObject) {
        // Test Can Build
        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
        bool canBuild = true;
        foreach(Vector2Int gridPosition in gridPositionList) {
            if(!grid.IsValidGridPositionWithPadding(gridPosition)) {
                // Not valid
                canBuild = false;
                break;
            }
            if(grid.GetGridObject(gridPosition.x, gridPosition.y).placedObject != null) {
                canBuild = false;
                break;
            }
        }

        if(canBuild) {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y);

            placedObject = PlacedObject.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);

            foreach(Vector2Int gridPosition in gridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }

            placedObject.GridSetupDone();

            OnObjectPlaced?.Invoke(placedObject, EventArgs.Empty);

            return true;
        } else {
            // Cannot build here
            placedObject = null;
            return false;
        }
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        grid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition) {
        return grid.GetWorldPosition(gridPosition.x, gridPosition.y);
    }

    public GridCell GetGridObject(Vector2Int gridPosition) {
        return grid.GetGridObject(gridPosition.x, gridPosition.y);
    }

    public GridCell GetGridObject(Vector3 worldPosition) {
        return grid.GetGridObject(worldPosition);
    }

    public bool IsValidGridPosition(Vector2Int gridPosition) {
        return grid.IsValidGridPosition(gridPosition);
    }

    public Vector3 GetMouseWorldSnappedPosition() {
        if(!Mouse3D.TryGetMouseWorldPosition(out Vector3 mousePosition)) return Vector3.zero;

        grid.GetXZ(mousePosition, out int x, out int z);

        if(placedObjectTypeSO != null) {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y);
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
}