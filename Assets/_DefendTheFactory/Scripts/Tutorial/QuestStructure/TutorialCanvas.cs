using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvas : MonoBehaviour {
	[SerializeField] private QuestListSO questChain;
	[SerializeField] private RectTransform questStepsContainer;
	[SerializeField] private QuestStepUi questStepUiPrefab;
	[SerializeField] private TMP_Text questTitleText;

	private int currentQuestIndex;
	private int currentQuestStepsCount;
	private int currentStepsCompleted;

	private readonly Dictionary<QuestStepSO, Action> stepCallbacks = new Dictionary<QuestStepSO, Action>();

	private void Start() {
		ExecuteNextQuest();
	}

	private void ExecuteNextQuest() {
		if (currentQuestIndex >= questChain.Quests.Count) {
			Debug.Log("WIN");
			// END TUTORIAL
			return;
		}
		
		foreach (Transform child in questStepsContainer) {
			Destroy(child.gameObject);
		}

		currentStepsCompleted = 0;
		Quest quest = questChain.Quests[currentQuestIndex];
		questTitleText.text = quest.QuestTitle;
		currentQuestStepsCount = quest.QuestSteps.Count;
		
		for (int index = 0; index < currentQuestStepsCount; index++) {
			QuestStepSO step = quest.QuestSteps[index];
			QuestStepUi stepUi = Instantiate(questStepUiPrefab, questStepsContainer);
			stepUi.Init(step.QuestStepDsc);
			Action callback = () => OnStepCompleted(step, stepUi);
			stepCallbacks[step] = callback;
			step.OnStepCompleted += callback;
			step.Execute();
		}
		
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate(questStepsContainer);
		currentQuestIndex++;
	}

	private void OnStepCompleted(QuestStepSO step, QuestStepUi questStepUi) {
		if (stepCallbacks.TryGetValue(step, out var callback)) {
			step.OnStepCompleted -= callback;
			stepCallbacks.Remove(step);
		}

		questStepUi.Complete();
		currentStepsCompleted++;

		if (currentStepsCompleted == currentQuestStepsCount) {
			ExecuteNextQuest();
		}
	}
}