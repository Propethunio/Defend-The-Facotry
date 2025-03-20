using UnityEngine;
using UtilsClass;
using System.Diagnostics;

public class GameManager : MonoBehaviour {
    [SerializeField] private MapDataSO data;
    [SerializeField] private bool showBeltDebug;
    [SerializeField] private bool generateMapAsync;

    private void Awake() {
        new ItemsManager();
    }

    private void Start() {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        MapGenerator mapGenerator = new MapGenerator(data);
        new BuildingSystem(mapGenerator.width, mapGenerator.height);
        new BeltManager(showBeltDebug);
        new MouseClickManager();
        mapGenerator.GenerateMap(generateMapAsync);
        MouseClickPlane.Instance.Setup(mapGenerator.width, mapGenerator.height);
        TilemapVisual.Instance.Init(mapGenerator.width, mapGenerator.height);
        TimeTickSystem.Instance.SetIsTicking(true);
        stopwatch.Stop();
        UnityEngine.Debug.Log($"[GAME DIAGNOSTIC INFO] Game setup + map generation took {stopwatch.ElapsedMilliseconds} ms");
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