using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour {

    public Vector2Int origin { get; protected set; }
    public Dir dir { get; protected set; }
    protected PlacedObjectTypeSO placedObjectTypeSO;

    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, Dir dir, PlacedObjectTypeSO placedObjectTypeSO) {
        PlacedObject placedObject = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, BuildingSystem.Instance.GetRotationAngle(dir), 0)).GetComponent<PlacedObject>();
        //ParticleSystem fxBuildingPlaced = Instantiate(GameAssets.i.fxBuildingPlaced, worldPosition, Quaternion.identity).GetComponent<ParticleSystem>();

        //ParticleSystem.MainModule mainModule = fxBuildingPlaced.main;
        //ParticleSystem.MinMaxCurve startSize = mainModule.startSize;
        //startSize.constant += .2f * Mathf.Max(placedObjectTypeSO.width, placedObjectTypeSO.height);
        //mainModule.startSize = startSize;

        //ParticleSystem.ShapeModule shapeModule = fxBuildingPlaced.shape;
        //Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
        //shapeModule.position = new Vector3(-rotationOffset.x, 0f, -rotationOffset.y) + new Vector3(placedObjectTypeSO.width, .4f, placedObjectTypeSO.height) * .5f;
        //shapeModule.scale = new Vector3(placedObjectTypeSO.width, placedObjectTypeSO.height, 1);

        //Transform soundTransform = Instantiate(GameAssets.i.sndBuilding, worldPosition, Quaternion.identity);
        //Destroy(soundTransform.gameObject, 2f);
        //AudioSource audioSource = soundTransform.GetComponent<AudioSource>();
        //audioSource.pitch = Random.Range(.85f, 1.15f);

        //if (placedObjectTypeSO == GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt) {
        //    audioSource.volume *= .5f;
        //}

        placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.origin = origin;
        placedObject.dir = dir;
        placedObject.Setup();

        return placedObject;
    }

    protected virtual void TriggerGridObjectChanged() {
        foreach(Vector2Int gridPosition in GetGridPositionList()) {
            BuildingSystem.Instance.grid.TriggerGridObjectChanged(gridPosition.x, gridPosition.y);
        }
    }

    protected virtual void Setup() { }

    public virtual void GridSetupDone() { }

    public Vector2Int GetGridPosition() {
        return origin;
    }

    public List<Vector2Int> GetGridPositionList() {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }

    public virtual void DestroySelf() {
        Destroy(gameObject);
    }

    public override string ToString() {
        return placedObjectTypeSO.nameString;
    }

}
