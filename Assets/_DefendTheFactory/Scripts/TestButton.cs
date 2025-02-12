using TMPro;
using UnityEngine;

public class TestButton : MonoBehaviour {

    [SerializeField] BaseBuildableObjectSO obj;

    void Start() {
        TMP_Text text = GetComponentInChildren<TMP_Text>();
        text.text = obj.nameString;
    }

    public void SetItem(BaseBuildableObjectSO test) {
        BuildingSystem.Instance.Test(test);
    }
}