using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class FactorySim : MonoBehaviour {

    int width = 50;
    int height = 50;

    private void Start() {
        new TimeTickSystem();
        new BeltManager();
        new BuildingSystem(width, height);
        TilemapVisual.Instance.Init(width, height);

        for (int i = 0; i < 10; i++) {
            //GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(5 + i, 5), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Right);
            //GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(5 + i, 7), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Left);
        }
        /*
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(11, 12), GameAssets.i.placedObjectTypeSO_Refs.miningMachine, PlacedObjectTypeSO.Dir.Down);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(13, 12), GameAssets.i.placedObjectTypeSO_Refs.grabber, PlacedObjectTypeSO.Dir.Right);

        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(14, 12), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Right);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(15, 12), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Right);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(16, 12), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Right);
        */
        //GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(16, 13), GameAssets.i.placedObjectTypeSO_Refs.grabber, PlacedObjectTypeSO.Dir.Up);
        //GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(16, 14), GameAssets.i.placedObjectTypeSO_Refs.smelter, PlacedObjectTypeSO.Dir.Down);

        /*
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(5, 5), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Down, out PlacedObject beltPlacedObject);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(5, 4), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Down);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(5, 3), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Down);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(5, 6), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Down);
        
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(5, 2), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Right);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(6, 2), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Right);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(7, 2), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Up);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(7, 3), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Right);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(8, 3), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Right);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(9, 3), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Up);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(9, 4), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Up);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(9, 5), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Up);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(9, 6), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Left);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(8, 6), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Left);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(7, 6), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Left);
        GridBuildingSystem.Instance.TryPlaceObject(new Vector2Int(6, 6), GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, PlacedObjectTypeSO.Dir.Left);
        
        WorldItem worldItem = WorldItem.Create(beltPlacedObject.GetGridPosition());

        (beltPlacedObject as ConveyorBelt).TrySetWorldItem(worldItem);
        worldItem.SetGridPosition(beltPlacedObject.GetGridPosition());
        //*/
    }

    private void Update() {
        HandleBuildingSelection();
    }

    private void HandleBuildingSelection() {
        if (Input.GetMouseButtonDown(0) && !CodeMonkey.Utils.UtilsClass.IsPointerOverUI() && BuildingSystem.Instance.GetPlacedObjectTypeSO() == null) {
            // Not building anything

            /*


            if (GridBuildingSystem.Instance.GetGridObject(Mouse3D.TryGetMouseWorldPosition()) != null) {
                PlacedObject placedObject = GridBuildingSystem.Instance.GetGridObject(Mouse3D.TryGetMouseWorldPosition()).GetPlacedObject();
                if (placedObject != null) {
                    // Clicked on something
                    if (placedObject is Smelter) {
                        SmelterUI.Instance.Show(placedObject as Smelter);
                    }
                    if (placedObject is MiningMachine) {
                        MiningMachineUI.Instance.Show(placedObject as MiningMachine);
                    }
                    if (placedObject is Assembler) {
                        AssemblerUI.Instance.Show(placedObject as Assembler);
                    }
                    if (placedObject is Storage) {
                        StorageUI.Instance.Show(placedObject as Storage);
                    }
                    if (placedObject is Grabber) {
                        GrabberUI.Instance.Show(placedObject as Grabber);
                    }
                }
            }


            */
        }
    }

    private void HandleDebugSpawnItem() {
        if (Input.GetKeyDown(KeyCode.I)) {
            PlacedObject placedObject = BuildingSystem.Instance.GetGridObject(BuildingSystem.Instance.GetMouseWorldSnappedPosition()).placedObject;
            if (placedObject != null && placedObject is IWorldItemSlot) {
                IWorldItemSlot worldItemSlot = placedObject as IWorldItemSlot;

                if (worldItemSlot.IsEmpty()) {
                    WorldItem worldItem = WorldItem.Create(worldItemSlot.GetGridPosition(), GameAssets.i.itemSO_Refs.ironOre);

                    worldItemSlot.TrySetWorldItem(worldItem);
                    worldItem.SetGridPosition(worldItemSlot.GetGridPosition());
                }
            }
        }
    }

}
