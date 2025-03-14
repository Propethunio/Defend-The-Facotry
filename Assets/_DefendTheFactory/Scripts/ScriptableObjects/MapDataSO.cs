using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Map Data")]
public class MapDataSO : ScriptableObject {
    [field: SerializeField] public Vector2Int widthRange { get; private set; }
    [field: SerializeField] public Vector2Int heightRange { get; private set; }
    [field: SerializeField] public Vector2Int portalPaddingRange { get; private set; }
    [field: SerializeField] public Vector2Int basePaddingRange { get; private set; }
    [field: SerializeField] public Vector2Int PathLenghtRange { get; private set; }
    [field: SerializeField] public int heightPadding { get; private set; }
    [field: SerializeField] public int minimumStraightLenghtOnPortal { get; private set; }
    [field: SerializeField] public int minimumStraightLenghtOnBase { get; private set; }
    [field: SerializeField] public int maximumStraightPathLenght { get; private set; }
    [field: SerializeField] public int maximumBacktrackingPathLenght { get; private set; }
    [field: SerializeField] public int backtrackingPreventingPadding { get; private set; }
    [field: SerializeField] public Vector2Int backtracksAmountRange { get; private set; }
    [field: SerializeField] public bool shouldPreventSquareLoops { get; private set; }
    [field: SerializeField] public bool shouldAllowPathSplits { get; private set; }
    [field: SerializeField] public Vector2Int splitsAmountRange { get; private set; }
    [field: SerializeField] public Vector2Int splitLengthRange { get; private set; }
    [field: SerializeField] public int splitPadding { get; private set; }
    [field: SerializeField] public GameObject pathStraightPrefab { get; private set; }
    [field: SerializeField] public GameObject pathTurnPrefab { get; private set; }
    [field: SerializeField] public GameObject pathSplitPrefab { get; private set; }
    [field: SerializeField] public GameObject groundPrefab { get; private set; }
    [field: SerializeField] public MainBaseSO baseData { get; private set; }
    [field: SerializeField] public BaseBuildableObjectSO portalData { get; private set; }
    [field: SerializeField] public int resourcesOnMapPercent { get; private set; }
    [field: SerializeField] public List<ResourceWeightPair> resourcesOnMap { get; private set; }
}

[Serializable]
public struct ResourceWeightPair {
    [field: SerializeField] public ResourceNodeSO resourceNode { get; private set; }
    [field: SerializeField] public int weight { get; private set; }
}