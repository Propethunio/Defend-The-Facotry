using System.Collections.Generic;
using UnityEngine;
using UtilsClass;

public class GameManager : MonoBehaviour {
    [SerializeField] private MapDataSO data;
    [SerializeField] private bool showBeltDebug;
    [SerializeField] private bool generateMapAsync;
    [SerializeField] private bool addResources;

    public List<ItemSO> items;

    private void Awake() {
        SceneLoader.Instance.OnSceneGroupLoaded += Init;
    }

    private void OnDestroy() {
        SceneLoader.Instance.OnSceneGroupLoaded -= Init;
    }

    private void Init() {
        SceneLoader.Instance.OnSceneGroupLoaded -= Init;
        Injector.Resolve<WaveManager>().Init();
        MapGenerator mapGenerator = new MapGenerator(data);
        Injector.Resolve<BuildingSystem>().Init(mapGenerator.width, mapGenerator.height);
        Injector.Resolve<BeltManager>().Init(showBeltDebug);
        new MouseInteractionManager();
        mapGenerator.GenerateMap(generateMapAsync);
        Injector.Resolve<MouseClickPlane>().Setup(mapGenerator.width, mapGenerator.height);
        Injector.Resolve<TilemapVisual>().Init(mapGenerator.width, mapGenerator.height);
        Injector.Resolve<TimeTickSystem>().SetIsTicking(true);

        if (!addResources) return;

        ItemsManager itemsManager = Injector.Resolve<ItemsManager>();

        foreach (var item in items) {
            itemsManager.AddItems(item, 300);
        }
    }

    private void Update() {
        HandleDebugSpawnItem();
        HandleDebugDeleteBuilding();
    }

    private void HandleDebugSpawnItem() {
        if (!Input.GetKeyDown(KeyCode.I)) return;

        BasePlacedObject placedObject = Injector.Resolve<BuildingSystem>().GetGridObject(Injector.Resolve<BuildingSystem>().GetMouseWorldSnappedPosition()).placedObject;
        if (!placedObject || placedObject is not ConveyorBelt belt || belt.startItem) return;

        WorldItem worldItem = WorldItem.Create(belt.origin, belt.dir, GameAssets.i.itemSO_Refs.ironOre);
        belt.TrySetWorldItem(worldItem);
    }

    private void HandleDebugDeleteBuilding() {
        if (Input.GetMouseButtonDown(1) && !MyUtils.IsPointerOverUI()) {
            Injector.Resolve<BuildingSystem>().HandleDemolish();
        }
    }
}