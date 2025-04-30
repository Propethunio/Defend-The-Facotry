using System;
using System.Collections.Generic;
using UnityEngine;
using UtilsClass;

public class BuildingSystem {
	public event Action OnSystemEnabled;
	public event Action OnSystemDisabled;
	public Action OnSelectedObject;
	public Action OnBuildCanceled;
	public Action OnObjectPlaced;

	public Grid<GridCell> grid { get; private set; }
	public BuildingDir dir { get; private set; }

	private BaseBuildableObjectSO placedObjectTypeSO;
	private InputManager inputManager;
	private ItemsManager itemsManager;
	private bool isBuildingSystemActive;
	private bool isDemolishActive;
	private TilemapVisual tilemapVisual;
	private MouseWorldPosition mouseWorldPosition;

	public void Init(int width, int height) {
		grid = new Grid<GridCell>(width, height, (_, _, _) => new GridCell());
		inputManager = Injector.Resolve<InputManager>();
		itemsManager = Injector.Resolve<ItemsManager>();
		tilemapVisual = Injector.Resolve<TilemapVisual>();
		mouseWorldPosition = Injector.Resolve<MouseWorldPosition>();
		Injector.Resolve<BuildingGhost>().Init();
	}

	~BuildingSystem() {
		Unsubscribe();
	}

	private void EnableBuildingSystem() {
		if (isBuildingSystemActive) return;

		dir = BuildingDir.Down;
		isBuildingSystemActive = true;
		tilemapVisual.Show();
		Subscribe();
		OnSystemEnabled?.Invoke();
	}

	public void DisableBuildingSystem() {
		if (!isBuildingSystemActive) return;

		placedObjectTypeSO = null;
		isBuildingSystemActive = false;
		isDemolishActive = false;
		tilemapVisual.Hide();
		OnBuildCanceled?.Invoke();
		Unsubscribe();
		OnSystemDisabled?.Invoke();
	}

	private void Subscribe() {
		inputManager.LeftClickAction += HandleObjectPlacement;
		inputManager.BuildingRotationAction += HandleDirRotation;
		inputManager.RightClickPerformedAction += DisableBuildingSystem;
		inputManager.RegisterBackAction(DisableBuildingSystem);
	}

	private void Unsubscribe() {
		inputManager.LeftClickAction -= HandleObjectPlacement;
		inputManager.BuildingRotationAction -= HandleDirRotation;
		inputManager.RightClickPerformedAction -= DisableBuildingSystem;
		inputManager.UnregisterBackAction(DisableBuildingSystem);
	}

	private void HandleObjectPlacement() {
		if (MyUtils.IsPointerOverUI() || !mouseWorldPosition.TryGetMouseWorldPosition(out Vector3 mousePosition)) return;

		int x = Mathf.FloorToInt(mousePosition.x);
		int z = Mathf.FloorToInt(mousePosition.z);
		Vector2Int placedObjectOrigin = new Vector2Int(x, z);
		TryPlaceObject(placedObjectOrigin);
	}

	private void HandleDirRotation() {
		dir = GetNextDir(dir);
	}

	public void HandleDemolish() {
		if (!mouseWorldPosition.TryGetMouseWorldPosition(out Vector3 mousePosition)) return;

		int x = Mathf.FloorToInt(mousePosition.x);
		int z = Mathf.FloorToInt(mousePosition.z);
		BasePlacedObject placedObject = grid.gridArray[x, z].placedObject;

		if (placedObject == null) return;

		if (placedObject is ConveyorBelt conveyorBelt && conveyorBelt.parentBuilding != null && conveyorBelt.parentBuilding is not LogisticMachine<BaseBuildableObjectSO>) {
			placedObject = conveyorBelt.parentBuilding;
		}

		if (!placedObject.isDestroyable()) return;

		List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
		int gridCellsCount = gridPositionList.Count;

		for (int i = 0; i < gridCellsCount; i++) {
			Vector2Int gridPosition = gridPositionList[i];
			grid.gridArray[gridPosition.x, gridPosition.y].ClearPlacedObject();
		}

		ReturnItems(placedObject);
		placedObject.DestroySelf();
	}

	private void UpdateCanBuildTilemap() {
		for (int x = 0; x < grid.width; x++) {
			for (int y = 0; y < grid.height; y++) {
				// Tilemap
				tilemapVisual.SetTilemapSprite(new Vector3(x, y), grid.gridArray[x, y].placedObject == null ? TilemapSprite.CanBuild : TilemapSprite.CannotBuild);
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

		if (placedObjectTypeSO == null) {
			tilemapVisual.Hide();
		}
		else {
			tilemapVisual.Show();
		}

		OnSelectedObject?.Invoke();
	}

	private void TryPlaceObject(Vector2Int placedObjectOrigin) {
		if (!CanAfford()) return;

		List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
		List<ConveyorBelt> beltsToRemove = new();

		int gridPositionCount = gridPositionList.Count;

		for (int i = 0; i < gridPositionCount; i++) {
			Vector2Int gridPosition = gridPositionList[i];

			if (!IsValidGridPosition(gridPosition)) return;

			GridCell cell = grid.gridArray[gridPosition.x, gridPosition.y];

			if (cell.isPathCell || (cell.placedObject != null && cell.placedObject is not ConveyorBelt)) return;

			if (cell.placedObject == null) continue;

			ConveyorBelt belt = cell.placedObject as ConveyorBelt;

			if (belt.parentBuilding != null) return;

			beltsToRemove.Add(belt);
		}

		int beltsToRemoveCount = beltsToRemove.Count;

		for (var i = 0; i < beltsToRemoveCount; i++) {
			var belt = beltsToRemove[i];
			belt.DestroySelf();
		}

		ConsumeItems();
		Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
		Vector3 placedObjectWorldPosition = new Vector3(placedObjectOrigin.x, 0, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y);
		BasePlacedObject placedObject = BaseDataPlacedObject<BaseBuildableObjectSO>.Create(placedObjectWorldPosition, dir, placedObjectTypeSO);
		placedObject.SetData(placedObjectOrigin, dir, placedObjectTypeSO);

		for (int i = 0; i < gridPositionCount; i++) {
			Vector2Int gridPosition = gridPositionList[i];
			grid.gridArray[gridPosition.x, gridPosition.y].SetPlacedObject(placedObject);
		}

		placedObject.GridSetupDone();
		OnObjectPlaced?.Invoke();
	}

	public void TryPlaceMapGeneratedObject(Vector2Int placedObjectOrigin, BaseBuildableObjectSO generatedObject, BuildingDir dir, Transform parent = null) {
		List<Vector2Int> gridPositionList = generatedObject.GetGridPositionList(placedObjectOrigin, dir);

		int gridPositionCount = gridPositionList.Count;

		for (int i = 0; i < gridPositionCount; i++) {
			Vector2Int gridPosition = gridPositionList[i];

			if (!IsValidGridPosition(gridPosition)) return;

			GridCell cell = grid.gridArray[gridPosition.x, gridPosition.y];

			if (cell.isPathCell || cell.placedObject != null) return;
		}

		Vector2Int rotationOffset = generatedObject.GetRotationOffset(dir);
		Vector3 placedObjectWorldPosition = new Vector3(placedObjectOrigin.x, 0, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y);
		BasePlacedObject placedObject = BaseDataPlacedObject<BaseBuildableObjectSO>.Create(placedObjectWorldPosition, dir, generatedObject);

		if (parent != null) {
			placedObject.transform.parent = parent;
		}

		placedObject.SetData(placedObjectOrigin, dir, generatedObject);

		for (int i = 0; i < gridPositionCount; i++) {
			Vector2Int gridPosition = gridPositionList[i];
			grid.gridArray[gridPosition.x, gridPosition.y].SetPlacedObject(placedObject);
		}

		placedObject.GridSetupDone();
	}

	private bool CanAfford() {
		int itemsCostCount = placedObjectTypeSO.cost.Count;

		for (int i = 0; i < itemsCostCount; i++) {
			if (!itemsManager.CanAfford(placedObjectTypeSO.cost[i].item, placedObjectTypeSO.cost[i].amount)) return false;
		}

		return true;
	}

	private void ConsumeItems() {
		int itemsCostCount = placedObjectTypeSO.cost.Count;

		for (int i = 0; i < itemsCostCount; i++) {
			itemsManager.RemoveItems(placedObjectTypeSO.cost[i].item, placedObjectTypeSO.cost[i].amount);
		}
	}

	private void ReturnItems(BasePlacedObject placedObject) {
		List<ItemIntPair> cost = placedObject.GetItemCost();
		float refundPenalty = placedObject.GetRefundPenalty();

		int itemsCostCount = cost.Count;

		if (refundPenalty > 0f) {
			for (int i = 0; i < itemsCostCount; i++) {
				int itemAmount = (int)(cost[i].amount - cost[i].amount * refundPenalty);
				itemsManager.AddItems(cost[i].item, itemAmount);
			}
		}
		else {
			for (int i = 0; i < itemsCostCount; i++) {
				itemsManager.AddItems(cost[i].item, cost[i].amount);
			}
		}
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

	private bool IsValidGridPosition(Vector2Int gridPosition) {
		return (gridPosition.x >= 0 && gridPosition.x < grid.gridArray.GetLength(0) && gridPosition.y >= 0 && gridPosition.y < grid.gridArray.GetLength(1));
	}

	public Vector3 GetMouseWorldSnappedPosition() {
		if (!mouseWorldPosition.TryGetMouseWorldPosition(out Vector3 mousePosition)) return Vector3.back;

		int x = Mathf.FloorToInt(mousePosition.x);
		int z = Mathf.FloorToInt(mousePosition.z);

		if (placedObjectTypeSO != null) {
			Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
			Vector3 placedObjectWorldPosition = new Vector3(x, 0, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y);
			return placedObjectWorldPosition;
		}
		else {
			return mousePosition;
		}
	}

	public Quaternion GetPlacedObjectRotation() {
		return placedObjectTypeSO == null ? Quaternion.identity : Quaternion.Euler(0, GetRotationAngle(dir), 0);
	}

	public BaseBuildableObjectSO GetPlacedObjectTypeSO() {
		return placedObjectTypeSO;
	}

	public void SetSelectedPlacedObject(BaseBuildableObjectSO placedObjectTypeSO) {
		EnableBuildingSystem();
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

	public void AddGhostBeltToGrid(Vector2Int beltPosition, BaseDataPlacedObject<BaseBuildableObjectSO> belt) {
		grid.gridArray[beltPosition.x, beltPosition.y].SetPlacedObject(belt);
	}

	private BuildingDir GetNextDir(BuildingDir dir) {
		switch (dir) {
			default:
			case BuildingDir.Down: return BuildingDir.Left;
			case BuildingDir.Left: return BuildingDir.Up;
			case BuildingDir.Up: return BuildingDir.Right;
			case BuildingDir.Right: return BuildingDir.Down;
		}
	}

	public Vector2Int GetDirForwardVector(BuildingDir dir) {
		switch (dir) {
			default:
			case BuildingDir.Down: return new Vector2Int(0, -1);
			case BuildingDir.Left: return new Vector2Int(-1, 0);
			case BuildingDir.Up: return new Vector2Int(0, +1);
			case BuildingDir.Right: return new Vector2Int(+1, 0);
		}
	}

	public int GetRotationAngle(BuildingDir dir) {
		switch (dir) {
			default:
			case BuildingDir.Down: return 0;
			case BuildingDir.Left: return 90;
			case BuildingDir.Up: return 180;
			case BuildingDir.Right: return 270;
		}
	}
}