using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Buildable Objects/Assembler")]
public class AssemblerSO : BaseMachineSO
{
    [field: SerializeField] public List<BeltPositionWithDir> inputBeltPositions { get; private set; }
    [field: SerializeField] public List<ItemRecipeSO> itemRecipeList { get; private set; }
    [field: SerializeField] public GameObject upgradedModel { get; private set; }

    
    [Serializable]
    public class BeltPositionWithDir
    {
        [field: SerializeField] public Vector2Int beltPosition { get; private set; }
        [field: SerializeField] public BuildingDir beltDir { get; private set; }
    }
}