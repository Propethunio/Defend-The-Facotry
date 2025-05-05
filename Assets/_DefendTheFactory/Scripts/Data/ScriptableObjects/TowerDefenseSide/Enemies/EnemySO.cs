using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Enemies/Enemy")]
public class EnemySO : FlyweightSettingsSO {
	[field: SerializeField] public int health { get; private set; }
	[field: SerializeField] public int shield { get; private set; }
	[field: SerializeField] public float speed { get; private set; }
	[field: SerializeField] public bool isFlying { get; private set; }
	[field: SerializeField] public int damage { get; private set; }

	public override Flyweight Create() {
		GameObject go = Instantiate(prefab);
		go.SetActive(false);
		go.name = prefab.name;
		EnemyLogic enemyLogic = go.GetComponent<EnemyLogic>();
		Flyweight flyweight = enemyLogic;
		flyweight.settings = this;
		enemyLogic.SetData(this);
		return flyweight;
	}
}