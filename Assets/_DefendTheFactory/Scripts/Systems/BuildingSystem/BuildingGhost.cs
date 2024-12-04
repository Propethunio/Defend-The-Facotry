using System;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    public static BuildingGhost Instance { get; private set; }

    public Action positionChanged;

    public Vector3 lastPosition { get; private set; }

    [SerializeField] float snapSpeed;

    Transform visual;
    BuildingSystem buildingSystem;

    private void Awake() {
        if(Instance == null) Instance = this;
        else return;
    }

    void LateUpdate() {
        if(!visual) return;

        MoveGhostToGridPosition();
    }

    public void Init() {
        buildingSystem = BuildingSystem.Instance;
        buildingSystem.OnSelectedObject += RefreshVisual;
        buildingSystem.OnBuildCanceled += DestroyVisual;
    }

    void MoveGhostToGridPosition() {
        Vector3 targetPosition = buildingSystem.GetMouseWorldSnappedPosition();

        if(targetPosition != Vector3.back && lastPosition != targetPosition) {
            lastPosition = targetPosition;
            positionChanged?.Invoke();
        }

        transform.position = Vector3.Lerp(transform.position, lastPosition, Time.deltaTime * snapSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, buildingSystem.GetPlacedObjectRotation(), Time.deltaTime * snapSpeed);
    }

    void RefreshVisual() {
        DestroyVisual();
        visual = Instantiate(buildingSystem.GetPlacedObjectTypeSO().visual, Vector3.zero, Quaternion.identity, transform);
        visual.localPosition = Vector3.zero;
        visual.localEulerAngles = Vector3.zero;
    }

    void DestroyVisual() {
        if(visual != null) {
            Destroy(visual.gameObject);
        }
    }
}