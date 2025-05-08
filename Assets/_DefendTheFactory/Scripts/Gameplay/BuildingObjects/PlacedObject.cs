using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDataPlacedObject<T> : BasePlacedObject where T : BaseBuildableObjectSO {
	public T buildableDataSO { get; private set; }

	private ObjectOutline outline;
	private BuildingPopupManager popupManager;

	public virtual void Start() {
		popupManager = Injector.Resolve<BuildingPopupManager>();

		if (buildableDataSO == null || !buildableDataSO.shouldHighlight) return;

		outline = GetComponent<ObjectOutline>();
	}

	public override bool ShouldHighlight() {
		return buildableDataSO.shouldHighlight;
	}

	public override void MouseEnterObject() {
		outline.SetOutline(true);
	}

	public override void MouseExitObject() {
		outline.SetOutline(false);
	}

	public override void MouseLeftClickObject() {
		popupManager.ShowBuildingPopup(this);
	}

	public static BasePlacedObject Create(Vector3 worldPosition, BuildingDir dir, T placedObjectDataSO) {
		return Instantiate(placedObjectDataSO.prefab, worldPosition, Quaternion.Euler(0, Injector.Resolve<BuildingSystem>().GetRotationAngle(dir), 0)).GetComponent<BasePlacedObject>();
	}

	protected abstract void Initialize(Vector2Int origin, BuildingDir dir, T placedObjectDataSO);

	public override void SetData(Vector2Int origin, BuildingDir dir, BaseBuildableObjectSO placedObjectDataSO) {
		if (placedObjectDataSO is T castedDataSO) {
			Initialize(origin, dir, castedDataSO);
		}
		else {
			Debug.LogError($"Invalid type passed to Initialize. Expected {typeof(T)} but got {placedObjectDataSO.GetType()}");
		}
	}

	public override bool isDestroyable() {
		return !buildableDataSO.isNotDestroyable;
	}

	public override List<ItemIntPair> GetItemCost() {
		return buildableDataSO.cost;
	}

	public override float GetRefundPenalty() {
		return buildableDataSO.refundPenalty;
	}

	protected void BaseDataSet(Vector2Int origin, BuildingDir dir, T placedObjectDataSO) {
		this.origin = origin;
		this.dir = dir;
		buildableDataSO = placedObjectDataSO;
		Setup();
	}

	protected virtual void TriggerGridObjectChanged() {
		foreach (Vector2Int gridPosition in GetGridPositionList()) {
			Injector.Resolve<BuildingSystem>().grid.TriggerGridObjectChanged(gridPosition.x, gridPosition.y);
		}
	}

	public override List<Vector2Int> GetGridPositionList() {
		return buildableDataSO.GetGridPositionList(origin, dir);
	}
	
	public override bool IsDataTheSame(BaseBuildableObjectSO data) {
		return data == buildableDataSO;
	}
}