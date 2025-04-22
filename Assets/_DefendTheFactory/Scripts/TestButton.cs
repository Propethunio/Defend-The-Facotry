using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour {
	[SerializeField] private BaseBuildableObjectSO obj;

	private void Start() {
		if (obj == null) return;
		
		GetComponentInChildren<TMP_Text>().text = obj.nameString;
		GetComponent<Button>().onClick.AddListener(() => Injector.Resolve<BuildingSystem>().Test(obj));
	}
}