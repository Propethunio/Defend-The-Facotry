using System;
using UnityEngine;

public abstract class QuestStepSO : ScriptableObject {
	[field: SerializeField] public string QuestStepDsc { get; private set; }
	
	public event Action OnStepCompleted;
	
	public abstract void Execute();

	protected virtual void CompleteStep() {
		OnStepCompleted?.Invoke();
	}
}