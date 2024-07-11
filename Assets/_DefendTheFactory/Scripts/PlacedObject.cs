using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour {

    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO) {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));

        ParticleSystem fxBuildingPlaced = Instantiate(GameAssets.i.fxBuildingPlaced, worldPosition, Quaternion.identity).GetComponent<ParticleSystem>();

        ParticleSystem.MainModule mainModule = fxBuildingPlaced.main;
        ParticleSystem.MinMaxCurve startSize = mainModule.startSize;
        startSize.constant += .2f * Mathf.Max(placedObjectTypeSO.width, placedObjectTypeSO.height);
        mainModule.startSize = startSize;

        ParticleSystem.ShapeModule shapeModule = fxBuildingPlaced.shape;
        Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
        shapeModule.position = new Vector3(-rotationOffset.x, 0f, -rotationOffset.y) + new Vector3(placedObjectTypeSO.width, .4f, placedObjectTypeSO.height) * .5f;
        shapeModule.scale = new Vector3(placedObjectTypeSO.width, placedObjectTypeSO.height, 1);

        // Sound effect
        Transform soundTransform = Instantiate(GameAssets.i.sndBuilding, worldPosition, Quaternion.identity);
        Destroy(soundTransform.gameObject, 2f);
        AudioSource audioSource = soundTransform.GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(.85f, 1.15f);

        if (placedObjectTypeSO == GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt) {
            audioSource.volume *= .5f;
        }

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

        placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.origin = origin;
        placedObject.dir = dir;

        placedObject.Setup();

        return placedObject;
    }




    protected PlacedObjectTypeSO placedObjectTypeSO;
    protected Vector2Int origin;
    protected PlacedObjectTypeSO.Dir dir;

    protected virtual void TriggerGridObjectChanged() {
        foreach (Vector2Int gridPosition in GetGridPositionList()) {
            GridBuildingSystem.Instance.GetGridObject(gridPosition).TriggerGridObjectChanged();
        }
    }

    protected virtual void Setup() {
        //Debug.Log("PlacedObject.Setup() " + transform);
    }

    public virtual void GridSetupDone() {
        //Debug.Log("PlacedObject.GridSetupDone() " + transform);
    }

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
