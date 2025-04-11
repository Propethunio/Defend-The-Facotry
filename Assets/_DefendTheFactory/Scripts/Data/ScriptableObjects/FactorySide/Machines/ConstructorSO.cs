using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Buildable Objects/Constructor")]
public class ConstructorSO : BaseMachineSO {
    [field: SerializeField] public Vector2Int inputBeltPosition { get; private set; }
    [field: SerializeField] public List<SimpleItemRecipeSO> itemRecipeList { get; private set; }
}