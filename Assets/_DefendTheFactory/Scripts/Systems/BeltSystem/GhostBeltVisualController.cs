using UnityEngine;

public class GhostBeltVisualController : MonoBehaviour {
    [SerializeField] private GameObject straightBeltVisual;
    [SerializeField] private GameObject leftTurnVisual;
    [SerializeField] private GameObject rightTurnVisual;

    private Vector2Int origin;
    private BeltManager beltManager;
    private BuildingGhost buildingGhost;
    private BuildingSystem buildingSystem;
    private GridCell[,] gridArray;
    private ConveyorBeltVisualController modifiedBeltVisual;
    private MouseWorldPosition mouseWorldPosition;

    private void Start() {
        beltManager = Injector.Resolve<BeltManager>();
        buildingSystem = Injector.Resolve<BuildingSystem>();
        buildingGhost = Injector.Resolve<BuildingGhost>();
        mouseWorldPosition = Injector.Resolve<MouseWorldPosition>();
        gridArray = buildingSystem.grid.gridArray;
        buildingSystem.OnObjectPlaced += ResetModifiedBelt;
        buildingGhost.positionChanged += SetVisual;
    }

    private void OnDestroy() {
        buildingSystem.OnObjectPlaced -= ResetModifiedBelt;
        buildingGhost.positionChanged -= SetVisual;

        if (modifiedBeltVisual != null) {
            modifiedBeltVisual.ShowStraightVisual();
        }
    }

    private void SetVisual() {
        mouseWorldPosition.TryGetMouseWorldPosition(out Vector3 mousePosition);
        origin = new Vector2Int((int)mousePosition.x, (int)mousePosition.z);
        Vector2Int forwardVector = buildingSystem.GetDirForwardVector(buildingSystem.dir);

        TryModifyNextBelt(origin + forwardVector);

        if (gridArray[origin.x, origin.y].placedObject != null || ShouldSnap(origin - forwardVector)) {
            ShowStraightVisual();
            return;
        }

        Vector2Int rightVector = new Vector2Int(forwardVector.y, -forwardVector.x);

        bool snapRight = ShouldSnap(origin + rightVector);
        bool snapLeft = ShouldSnap(origin - rightVector);

        if (snapLeft && !snapRight) {
            ShowLeftVisual();
        }
        else if (snapRight && !snapLeft) {
            ShowRightVisual();
        }
        else {
            ShowStraightVisual();
        }
    }

    private void TryModifyNextBelt(Vector2Int nextPosition) {
        if (modifiedBeltVisual != null) {
            modifiedBeltVisual.ShowStraightVisual();
            modifiedBeltVisual = null;
        }

        if (!IsPositionValid(nextPosition)) return;

        ConveyorBelt nextBelt = gridArray[nextPosition.x, nextPosition.y].placedObject as ConveyorBelt;

        if (nextBelt == null || nextBelt.parentBuilding != null || !beltManager.beltEndsDict.ContainsKey(nextBelt) || nextBelt.nextPosition == origin || nextBelt.previousPosition == origin) {
            return;
        }

        if (IsPositionValid(new Vector2Int(nextBelt.previousPosition.x, nextBelt.previousPosition.y))) {
            ConveyorBelt beltConnectedToNextBelt = gridArray[nextBelt.previousPosition.x, nextBelt.previousPosition.y].placedObject as ConveyorBelt;

            if (beltConnectedToNextBelt != null && beltConnectedToNextBelt.nextPosition == nextBelt.origin) {
                return;
            }
        }

        modifiedBeltVisual = nextBelt.gameObject.GetComponent<ConveyorBeltVisualController>();
        Vector2Int forwardVector = buildingSystem.GetDirForwardVector(nextBelt.dir);
        Vector2Int rightVector = new Vector2Int(forwardVector.y, -forwardVector.x);

        if (nextBelt.origin - rightVector == origin) {
            modifiedBeltVisual.ShowLeftVisual();
        }
        else {
            modifiedBeltVisual.ShowRightVisual();
        }
    }

    private bool ShouldSnap(Vector2Int position) {
        return IsPositionValid(position) && (gridArray[position.x, position.y].placedObject is ConveyorBelt belt && belt.nextPosition == origin || gridArray[position.x, position.y].placedObject is LogisticMachine<BaseBuildableObjectSO> logistic && logistic.IsOnOutputCell(origin));
    }

    private bool IsPositionValid(Vector2Int position) {
        return position.x >= 0 && position.x < gridArray.GetLength(0) && position.y >= 0 && position.y < gridArray.GetLength(1);
    }

    private void ShowStraightVisual() {
        straightBeltVisual.SetActive(true);
        leftTurnVisual.SetActive(false);
        rightTurnVisual.SetActive(false);
    }

    private void ShowLeftVisual() {
        straightBeltVisual.SetActive(false);
        leftTurnVisual.SetActive(true);
        rightTurnVisual.SetActive(false);
    }

    private void ShowRightVisual() {
        straightBeltVisual.SetActive(false);
        leftTurnVisual.SetActive(false);
        rightTurnVisual.SetActive(true);
    }

    private void ResetModifiedBelt(BaseBuildableObjectSO obj) {
        modifiedBeltVisual = null;
    }
}