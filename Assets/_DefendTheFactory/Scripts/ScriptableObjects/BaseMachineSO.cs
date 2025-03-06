using UnityEngine;

public abstract class BaseMachineSO : BaseBuildableObjectSO {

    [field: SerializeField] public Vector2Int outputBeltPosition { get; private set; }
    [field: SerializeField] public int maxStoredOutputItems { get; private set; }
}