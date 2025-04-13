using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour {
    [SerializeField] private BaseBuildableObjectSO obj;

    private void Start() {
        GetComponentInChildren<TMP_Text>().text = obj.nameString;
        GetComponent<Button>().onClick.AddListener(() => Injector.Resolve<BuildingSystem>().Test(obj));
    }
}