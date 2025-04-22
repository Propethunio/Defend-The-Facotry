using UnityEngine;

public abstract class FlyweightSettingsSO : ScriptableObject {
	public FlyweightType type;
	public GameObject prefab;

	public virtual Flyweight Create() {
		GameObject go = Instantiate(prefab);
		go.SetActive(false);
		go.name = prefab.name;
		Flyweight flyweight = go.AddComponent<Flyweight>();
		flyweight.settings = this;
		return flyweight;
	}

	public virtual void OnGet(Flyweight flyweight) => flyweight.gameObject.SetActive(true);
	public virtual void OnRelease(Flyweight flyweight) => flyweight.gameObject.SetActive(false);
	public virtual void OnDestroyPoolObject(Flyweight flyweight) => Destroy(flyweight.gameObject);
}