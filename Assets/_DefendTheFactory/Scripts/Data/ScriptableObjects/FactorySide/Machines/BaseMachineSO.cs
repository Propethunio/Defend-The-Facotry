using UnityEngine;

public abstract class BaseMachineSO : BaseBuildableObjectSO {
	[field: SerializeField] public Vector2Int outputBeltPosition { get; private set; }
}