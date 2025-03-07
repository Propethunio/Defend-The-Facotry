using UnityEngine;
using UtilsClass;

public class GameManager : MonoBehaviour {
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private bool showBeltDebug;

    private void Awake() {
        new ItemsManager();
    }

    private void Start() {
        new BuildingSystem(width, height);
        new BeltManager(showBeltDebug);
        MouseClickPlane.Instance.Setup(width, height);
        TilemapVisual.Instance.Init(width, height);
        TimeTickSystem.Instance.SetIsTicking(true);
    }

    private void Update() {
        HandleDebugSpawnItem();
        HandleDebugDeleteBuilding();
    }

    private void HandleDebugSpawnItem() {
        if (!Input.GetKeyDown(KeyCode.I)) return;

        BasePlacedObject placedObject = BuildingSystem.Instance.GetGridObject(BuildingSystem.Instance.GetMouseWorldSnappedPosition()).placedObject;
        if (!placedObject || placedObject is not ConveyorBelt belt || belt.startItem) return;

        WorldItem worldItem = WorldItem.Create(belt.origin, belt.dir, GameAssets.i.itemSO_Refs.ironOre);
        belt.TrySetWorldItem(worldItem);
    }

    private void HandleDebugDeleteBuilding() {
        if (Input.GetMouseButtonDown(1) && !MyUtils.IsPointerOverUI()) {
            BuildingSystem.Instance.HandleDemolish();
        }
    }
}