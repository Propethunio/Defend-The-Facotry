using UnityEngine;

public class BeltVisualController : MonoBehaviour {

    [SerializeField] GameObject straightBeltVisual;
    [SerializeField] GameObject leftTurnVisual;
    [SerializeField] GameObject rightTurnVisual;

    Vector2Int origin;
    BeltManager beltMenager;
    BuildingGhost buildingGhost;
    BuildingSystem buildingSystem;
    GridCell[,] gridArray;
    ConveyorBelt modifiedBelt;

    void Start() {
        beltMenager = BeltManager.Instance;
        buildingSystem = BuildingSystem.Instance;
        gridArray = buildingSystem.grid.gridArray;
        buildingSystem.OnObjectPlaced += ResetModifiedBelt;
        buildingGhost = BuildingGhost.Instance;
        buildingGhost.positionChanged += SetVisual;
    }

    void OnDestroy() {
        buildingSystem.OnObjectPlaced -= ResetModifiedBelt;
        buildingGhost.positionChanged -= SetVisual;
    }

    void SetVisual() {
        Mouse3D.TryGetMouseWorldPosition(out Vector3 mousePosition);
        origin = new Vector2Int((int)mousePosition.x, (int)mousePosition.z);
        Vector2Int forwardVector = buildingSystem.GetDirForwardVector(buildingSystem.dir);

        TryModifyNextBelt(origin + forwardVector);

        if(gridArray[origin.x, origin.y].placedObject != null || ShouldSnap(origin - forwardVector)) {
            ShowStraightVisual();
            return;
        }

        Vector2Int rightVector = new Vector2Int(forwardVector.y, -forwardVector.x);

        bool snapRight = ShouldSnap(origin + rightVector);
        bool snapLeft = ShouldSnap(origin - rightVector);

        if(snapLeft && !snapRight) {
            ShowLeftVisual();
        } else if(snapRight && !snapLeft) {
            ShowRightVisual();
        } else {
            ShowStraightVisual();
        }
    }

    void TryModifyNextBelt(Vector2Int nextPosition) {
        if(modifiedBelt != null) {
            modifiedBelt.ShowStraightVisual();
            modifiedBelt = null;
        }

        if(!IsPositionValid(nextPosition)) return;

        ConveyorBelt nextBelt = gridArray[nextPosition.x, nextPosition.y].placedObject as ConveyorBelt;
        if(nextBelt == null || nextBelt.isPartOfBuilding || !beltMenager.beltEndsDict.ContainsKey(nextBelt) || nextBelt.nextPosition == origin || nextBelt.previousPosition == origin) {
            return;
        }

        if(IsPositionValid(new Vector2Int(nextBelt.previousPosition.x, nextBelt.previousPosition.y))) {
            ConveyorBelt beltConnectedToNextBelt = gridArray[nextBelt.previousPosition.x, nextBelt.previousPosition.y].placedObject as ConveyorBelt;

            if(beltConnectedToNextBelt != null && beltConnectedToNextBelt.nextPosition == nextBelt.origin) {
                return;
            }
        }

        modifiedBelt = nextBelt;
        Vector2Int forwardVector = buildingSystem.GetDirForwardVector(nextBelt.dir);
        Vector2Int rightVector = new Vector2Int(forwardVector.y, -forwardVector.x);

        if(nextBelt.origin - rightVector == origin) {
            nextBelt.ShowLeftVisual();
        } else {
            nextBelt.ShowRightVisual();
        }
    }

    bool ShouldSnap(Vector2Int position) {
        return IsPositionValid(position) && gridArray[position.x, position.y].placedObject is ConveyorBelt belt && belt.nextPosition == origin;
    }

    bool IsPositionValid(Vector2Int position) {
        return position.x >= 0 && position.x < gridArray.GetLength(0) && position.y >= 0 && position.y < gridArray.GetLength(1);
    }

    void ShowStraightVisual() {
        straightBeltVisual.SetActive(true);
        leftTurnVisual.SetActive(false);
        rightTurnVisual.SetActive(false);
    }

    void ShowLeftVisual() {
        straightBeltVisual.SetActive(false);
        leftTurnVisual.SetActive(true);
        rightTurnVisual.SetActive(false);
    }

    void ShowRightVisual() {
        straightBeltVisual.SetActive(false);
        leftTurnVisual.SetActive(false);
        rightTurnVisual.SetActive(true);
    }

    void ResetModifiedBelt() {
        modifiedBelt = null;
    }
}