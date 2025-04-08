using UnityEngine;

public interface IItemProvider {
    bool HasItem();
    WorldItem GetWorldItem();
    BuildingDir GetDir();
    bool ShouldSnapWithLogisticMachine(Vector2Int logisticMachineOrigin);
}