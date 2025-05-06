using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/QuestChain")]
public class QuestListSO : ScriptableObject {
	[field: SerializeField] public List<Quest> Quests { get; private set; }
}

[Serializable]
public class Quest {
	[field: SerializeField] public string QuestTitle { get; private set; }
	[field: SerializeField] public List<QuestStepSO> QuestSteps { get; private set; }
}