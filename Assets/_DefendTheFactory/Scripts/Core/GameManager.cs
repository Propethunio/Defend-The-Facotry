using System.Collections.Generic;
using UnityEngine;
using UtilsClass;

public class GameManager : MonoBehaviour {
    [SerializeField] private MapDataSO data;
    [SerializeField] private bool showBeltDebug;
    [SerializeField] private bool generateMapAsync;
    [SerializeField] private bool addResources;
    
    public EnemyLogic enemyPrefab;
    public List<ItemSO> items;
    
    private void Awake() {
        Debug.Log("GameManager");
        SceneLoader.Instance.OnSceneGroupLoaded += Init;
    }

    private void OnDestroy() {
        SceneLoader.Instance.OnSceneGroupLoaded -= Init;
    }

    private void Init() {
        Debug.Log("GameManager Init");
        SceneLoader.Instance.OnSceneGroupLoaded -= Init;
        new ItemsManager();
        new WaveManager(enemyPrefab);
        MapGenerator mapGenerator = new MapGenerator(data);
        new BuildingSystem(mapGenerator.width, mapGenerator.height);
        new BeltManager(showBeltDebug);
        new MouseInteractionManager();
        mapGenerator.GenerateMap(generateMapAsync);
        MouseClickPlane.Instance.Setup(mapGenerator.width, mapGenerator.height);
        TilemapVisual.Instance.Init(mapGenerator.width, mapGenerator.height);
        TimeTickSystem.Instance.SetIsTicking(true);

        if (addResources) {
            foreach (var item in items) {
                ItemsManager.Instance.AddItems(item, 300);   
            }
        }
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