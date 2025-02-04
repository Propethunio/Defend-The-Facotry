using TMPro;
using UnityEngine;

public class TestButton : MonoBehaviour {

    [SerializeField] PlacedObjectTypeSO obj;

    void Start() {
        TMP_Text text = GetComponentInChildren<TMP_Text>();
        text.text = obj.nameString;
    }

    public void SetItem(PlacedObjectTypeSO test) {
        BuildingSystem.Instance.Test(test);
    }
}