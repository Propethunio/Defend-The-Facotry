using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Items/World Item")]
public class ItemSO : FlyweightSettingsSO {
	[field: SerializeField] public Sprite icon { get; private set; }
	[field: SerializeField] public string itemName { get; private set; }

	public override Flyweight Create() {
		GameObject go = Instantiate(prefab);
		go.SetActive(false);
		go.name = itemName;
		Flyweight flyweight = go.AddComponent<WorldItem>();
		flyweight.settings = this;
		return flyweight;
	}
}