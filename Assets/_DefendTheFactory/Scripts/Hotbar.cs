using System;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour {
	[SerializeField] private List<HotbarBtn> hotbarBtns;

	private int btnsCount;

	private void Start() {
		btnsCount = hotbarBtns.Count;
		Subscribe();
	}

	private void OnDestroy() {
		Unsubscribe();
	}

	private void Subscribe() {
		Injector.Resolve<InputManager>().HotbarAction += OnHotbarAction;
		Injector.Resolve<BuildingSystem>().OnSystemDisabled += DisableHighlights;

		for (int i = 0; i < btnsCount; i++) {
			hotbarBtns[i].OnBtnClick += DisableHighlights;
		}
	}

	private void Unsubscribe() {
		Injector.Resolve<InputManager>().HotbarAction -= OnHotbarAction;
		Injector.Resolve<BuildingSystem>().OnSystemDisabled -= DisableHighlights;

		for (int i = 0; i < btnsCount; i++) {
			hotbarBtns[i].OnBtnClick -= DisableHighlights;
		}
	}

	private void DisableHighlights() {
		for (int i = 0; i < btnsCount; i++) {
			hotbarBtns[i].SetHighlight(false);
		}
	}

	private void OnHotbarAction(int index) {
		DisableHighlights();
		hotbarBtns[index].ButtonAction();
	}
}