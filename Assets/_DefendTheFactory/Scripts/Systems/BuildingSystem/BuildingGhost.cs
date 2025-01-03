﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    Transform visual;
    PlacedObjectTypeSO placedObjectTypeSO;

    void Start() {
        //RefreshVisual();

        //BuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    void Instance_OnSelectedChanged(object sender, System.EventArgs e) {
        RefreshVisual();
    }

    void LateUpdate() {
        Vector3 targetPosition = BuildingSystem.Instance.GetMouseWorldSnappedPosition();
        targetPosition.y = 0f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

        transform.rotation = Quaternion.Lerp(transform.rotation, BuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
    }

    void RefreshVisual() {
        if(visual != null) {
            Destroy(visual.gameObject);
            visual = null;
        }

        PlacedObjectTypeSO placedObjectTypeSO = BuildingSystem.Instance.GetPlacedObjectTypeSO();

        if(placedObjectTypeSO != null) {
            visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            SetLayerRecursive(visual.gameObject, 11);
        }
    }

    void SetLayerRecursive(GameObject targetGameObject, int layer) {
        targetGameObject.layer = layer;
        foreach(Transform child in targetGameObject.transform) {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
}