using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class TutorialCanvas : MonoBehaviour {
	[SerializeField] private QuestListSO questChain;
	[SerializeField] private RectTransform questPanel;
	[SerializeField] private RectTransform questStepsContainer;
	[SerializeField] private QuestStepUi questStepUiPrefab;
	[SerializeField] private TMP_Text questTitleText; 
	
	[field: SerializeField] public Clock hudClock {get; private set;}
	[field: SerializeField] public BuildingsMenu hudBuildingsMenu {get; private set;}
	[field: SerializeField] public Statistics hudStatistics {get; private set;}

	private int currentQuestIndex;
	private int currentQuestStepsCount;
	private int currentStepsCompleted;

	private readonly Dictionary<QuestStepSO, Action> stepCallbacks = new Dictionary<QuestStepSO, Action>();

	private void Awake() {
		if (!GameSetupData.Instance.IsTutorialLevel()) {
			Destroy(gameObject);
		}
	}

	private void Start() {
		LoadNextQuest();
	}

	private void LoadNextQuest() {
		foreach (Transform child in questStepsContainer) {
			Destroy(child.gameObject);
		}

		currentStepsCompleted = 0;
		Quest quest = questChain.Quests[currentQuestIndex];
		questTitleText.text = quest.QuestTitle;
		currentQuestStepsCount = quest.QuestSteps.Count;
		int questStartActionsCount = quest.QuestStartAction.Count;

		for (int i = 0; i < questStartActionsCount; i++) {
			quest.QuestStartAction[i].Execute(this);
		}

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

	private void ExecuteNextQuestWithAnimation() {
		if (currentQuestIndex >= questChain.Quests.Count) {
			Injector.Resolve<GameManager>().GameWon();
			return;
		}

		float containerWidth = questPanel.rect.width;
		Vector2 originalPos = questPanel.anchoredPosition;
		Vector2 offscreenLeft = originalPos + Vector2.left * (containerWidth + 50);
		Sequence transition = DOTween.Sequence();
		transition.Append(questPanel.DOAnchorPos(offscreenLeft, 0.4f).SetEase(Ease.InOutCubic));
		transition.AppendInterval(0.3f);
		transition.AppendCallback(LoadNextQuest);
		transition.Append(questPanel.DOAnchorPos(originalPos, 0.4f).SetEase(Ease.InOutCubic));
	}

	private void OnStepCompleted(QuestStepSO step, QuestStepUi questStepUi) {
		if (stepCallbacks.TryGetValue(step, out var callback)) {
			step.OnStepCompleted -= callback;
			stepCallbacks.Remove(step);
		}

		questStepUi.Complete();
		currentStepsCompleted++;

		if (currentStepsCompleted == currentQuestStepsCount) {
			DOVirtual.DelayedCall(0.5f, ExecuteNextQuestWithAnimation);
		}
	}
}