using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] bool showBeltDebug;

    void Start() {
        new BuildingSystem(width, height);
        new BeltManager(showBeltDebug);
        MouseClickPlane.Instance.Setup(width, height);
        TilemapVisual.Instance.Init(width, height);
        TimeTickSystem.Instance.SetIsTicking(true);
    }

    void Update() {
        HandleDebugSpawnItem();
    }

    void HandleDebugSpawnItem() {
        if(Input.GetKeyDown(KeyCode.I)) {
            PlacedObject placedObject = BuildingSystem.Instance.GetGridObject(BuildingSystem.Instance.GetMouseWorldSnappedPosition()).placedObject;
            if(placedObject != null && placedObject is ConveyorBelt) {
                ConveyorBelt belt = placedObject as ConveyorBelt;

                if(belt.worldItem == null) {
                    WorldItem worldItem = WorldItem.Create(belt.GetGridPosition(), GameAssets.i.itemSO_Refs.ironOre);
                    belt.TrySetWorldItem(worldItem);
                }
            }
        }
    }
}