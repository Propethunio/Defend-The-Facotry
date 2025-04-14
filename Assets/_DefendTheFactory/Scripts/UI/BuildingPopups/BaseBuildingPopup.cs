using UnityEngine;

public abstract class BaseBuildingPopup : MonoBehaviour {
    public void Show() {
        gameObject.SetActive(true);
    }

    public void Close() {
        gameObject.SetActive(false);
    }

    public abstract void Setup(BasePlacedObject placedObject);
}