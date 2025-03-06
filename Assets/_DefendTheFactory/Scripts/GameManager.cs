using UnityEngine;
using UtilsClass;

public class GameManager : MonoBehaviour {

    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] bool showBeltDebug;

    void Awake() {
        new ItemsManager();
    }

    void Start() {
        new BuildingSystem(width, height);
        new BeltManager(showBeltDebug);
        MouseClickPlane.Instance.Setup(width, height);
        TilemapVisual.Instance.Init(width, height);
        TimeTickSystem.Instance.SetIsTicking(true);
    }

    void Update() {
        HandleDebugSpawnItem();
        HandleDebugDeleteBuilding();
    }

    void HandleDebugSpawnItem() {
        if(Input.GetKeyDown(KeyCode.I)) {
            BasePlacedObject placedObject = BuildingSystem.Instance.GetGridObject(BuildingSystem.Instance.GetMouseWorldSnappedPosition()).placedObject;
            if(placedObject != null && placedObject is ConveyorBelt) {
                ConveyorBelt belt = placedObject as ConveyorBelt;

                if(belt.startItem == null) {
                    WorldItem worldItem = WorldItem.Create(belt.origin, belt.dir, GameAssets.i.itemSO_Refs.ironOre);
                    belt.TrySetWorldItem(worldItem);
                }
            }
        }
    }

    void HandleDebugDeleteBuilding() {
        if(Input.GetMouseButtonDown(1) && !MyUtils.IsPointerOverUI()) {
            BuildingSystem.Instance.HandleDemolish();
        }
    }
}