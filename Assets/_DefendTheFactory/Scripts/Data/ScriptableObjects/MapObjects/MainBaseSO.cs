using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Buildable Objects/Main Base")]
public class MainBaseSO : BaseBuildableObjectSO {
    [field: SerializeField] public List<BeltPositionWithDir> inputBeltPositions { get; private set; }
    [field: SerializeField] public List<Upgrades> UpgradesList { get; private set; }

    [Serializable]
    public class BeltPositionWithDir {
        [field: SerializeField] public Vector2Int beltPosition { get; private set; }
        [field: SerializeField] public BuildingDir beltDir { get; private set; }
    }

    [Serializable]
    public class Upgrades {
        [field: SerializeField] public GameObject upgradePrefab { get; private set; }
        [field: SerializeField] public List<ItemIntPair> cost { get; private set; }
    }
}