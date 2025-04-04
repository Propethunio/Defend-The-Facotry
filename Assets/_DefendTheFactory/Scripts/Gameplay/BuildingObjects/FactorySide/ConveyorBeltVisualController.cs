using UnityEngine;

public class ConveyorBeltVisualController : MonoBehaviour {

    [SerializeField] private ConveyorBelt _conveyorBelt;
    [SerializeField] private GameObject _straightBeltVisual;
    [SerializeField] private GameObject _leftTurnVisual;
    [SerializeField] private GameObject _rightTurnVisual;

    private void Awake() {
        _conveyorBelt.OnVisualUpdate += UpdateVisuals;
    }

    private void OnDestroy() {
        _conveyorBelt.OnVisualUpdate -= UpdateVisuals;
    }

    private void UpdateVisuals(Vector2Int origin, Vector2Int previousPosition) {
        _conveyorBelt.OnVisualUpdate -= UpdateVisuals;
        Vector2Int forwardVector = BuildingSystem.Instance.GetDirForwardVector(_conveyorBelt.dir);
        Vector2Int backPosition = origin - forwardVector;

        if(previousPosition == backPosition) {
            ShowStraightVisual();
            return;
        }

        Vector2Int rightVector = new Vector2Int(forwardVector.y, -forwardVector.x);
        Vector2Int leftPosition = origin - rightVector;

        if(previousPosition == leftPosition) {
            ShowLeftVisual();
        } else {
            ShowRightVisual();
        }
    }

    public void ShowStraightVisual() {
        _straightBeltVisual.SetActive(true);
        _leftTurnVisual.SetActive(false);
        _rightTurnVisual.SetActive(false);
    }

    public void ShowLeftVisual() {
        _straightBeltVisual.SetActive(false);
        _leftTurnVisual.SetActive(true);
        _rightTurnVisual.SetActive(false);
    }

    public void ShowRightVisual() {
        _straightBeltVisual.SetActive(false);
        _leftTurnVisual.SetActive(false);
        _rightTurnVisual.SetActive(true);
    }
}