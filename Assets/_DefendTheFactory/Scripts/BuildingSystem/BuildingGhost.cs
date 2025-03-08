using System;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {
    public static BuildingGhost Instance { get; private set; }

    [SerializeField] private float snapSpeed;

    public Action positionChanged;

    private Vector3 lastPosition;
    private Transform visual;
    private BuildingSystem buildingSystem;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void LateUpdate() {
        if (!visual) return;

        MoveGhostToGridPosition();
    }

    public void Init() {
        buildingSystem = BuildingSystem.Instance;
        buildingSystem.OnSelectedObject += RefreshVisual;
        buildingSystem.OnBuildCanceled += DestroyVisual;
    }

    private void MoveGhostToGridPosition() {
        Vector3 targetPosition = buildingSystem.GetMouseWorldSnappedPosition();

        if (targetPosition != Vector3.back && lastPosition != targetPosition) {
            lastPosition = targetPosition;
            positionChanged?.Invoke();
        }

        transform.position = Vector3.Lerp(transform.position, lastPosition, Time.deltaTime * snapSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, buildingSystem.GetPlacedObjectRotation(), Time.deltaTime * snapSpeed);
    }

    private void RefreshVisual() {
        DestroyVisual();
        visual = Instantiate(buildingSystem.GetPlacedObjectTypeSO().visual, Vector3.zero, Quaternion.identity, transform);
        visual.localPosition = Vector3.zero;
        visual.localEulerAngles = Vector3.zero;
    }

    private void DestroyVisual() {
        if (visual != null) {
            Destroy(visual.gameObject);
        }
    }
}