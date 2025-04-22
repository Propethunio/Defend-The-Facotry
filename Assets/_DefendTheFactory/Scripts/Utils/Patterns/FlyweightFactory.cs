using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FlyweightFactory {
	private readonly Dictionary<FlyweightType, IObjectPool<Flyweight>> pools = new Dictionary<FlyweightType, IObjectPool<Flyweight>>();

	public Flyweight Spawn(FlyweightSettingsSO settings) => GetPoolFor(settings).Get();
	public void ReturnToPool(Flyweight flyweight) => GetPoolFor(flyweight.settings).Release(flyweight);

	private IObjectPool<Flyweight> GetPoolFor(FlyweightSettingsSO settings) {
		if (pools.TryGetValue(settings.type, out IObjectPool<Flyweight> pool)) return pool;

		pool = new ObjectPool<Flyweight>(settings.Create, settings.OnGet, settings.OnRelease, settings.OnDestroyPoolObject, true, 30, 150);
		pools.Add(settings.type, pool);
		return pool;
	}

	public WorldItem CreateWorldItem(Vector2Int gridPosition, BuildingDir dir, ItemSO itemScriptableObject) {
		WorldItem worldItem = Spawn(itemScriptableObject) as WorldItem;
		Vector3 worldPosition = new Vector3(gridPosition.x, 0.31f, gridPosition.y);
		Quaternion rotation = Quaternion.identity;

		switch (dir) {
			case BuildingDir.Down:
				worldPosition += new Vector3(0.5f, 0, 0.75f);
				rotation = Quaternion.Euler(0, 180, 0);
				break;
			case BuildingDir.Left:
				worldPosition += new Vector3(0.75f, 0, 0.5f);
				rotation = Quaternion.Euler(0, 270, 0);
				break;
			case BuildingDir.Up:
				worldPosition += new Vector3(0.5f, 0, 0.25f);
				rotation = Quaternion.Euler(0, 0, 0);
				break;
			case BuildingDir.Right:
				worldPosition += new Vector3(0.25f, 0, 0.5f);
				rotation = Quaternion.Euler(0, 90, 0);
				break;
		}

		worldItem.transform.position = worldPosition;
		worldItem.transform.rotation = rotation;
		return worldItem;
	}
}