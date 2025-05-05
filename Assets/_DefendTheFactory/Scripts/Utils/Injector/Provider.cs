using UnityEngine;

public class Provider : MonoBehaviour, IDependencyProvider {
	[Provide] public ItemsManager ProvideItemsManager() => new ItemsManager();
	[Provide] public WaveManager ProvideWaveManager() => new WaveManager();
	[Provide] public BuildingSystem ProvideBuildingSystem() => new BuildingSystem();
	[Provide] public BeltManager ProvideBeltManager() => new BeltManager();
	[Provide] public FlyweightFactory ProvideFlyweightFactory() => new FlyweightFactory();
	[Provide] public HealthManager ProvideHealthManager() => new HealthManager();
}