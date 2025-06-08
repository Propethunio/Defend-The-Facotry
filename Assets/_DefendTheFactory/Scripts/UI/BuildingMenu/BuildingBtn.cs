using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingBtn : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
	[SerializeField] private Image glow;
	[SerializeField] private Image icon;

	public BaseBuildableObjectSO buildableObject { get; private set; }

	private RectTransform dragDrop;
	private RectTransform canvasRect;
	private Image image;
	private bool isDummy = true;

	public event Action<BaseBuildableObjectSO, BuildingBtn> onBtnClick;

	public void Init(BaseBuildableObjectSO data, RectTransform dragDropPanel, RectTransform mainCanvasRect) {
		buildableObject = data;
		icon.sprite = buildableObject.icon;
		icon.enabled = true;
		dragDrop = dragDropPanel;
		isDummy = false;
		canvasRect = mainCanvasRect;
		image = dragDrop.GetComponent<Image>();
	}

	public void ToggleHighlight(bool isActive) {
		glow.enabled = isActive;
	}

	public void OnPointerDown(PointerEventData eventData) { }

	public void OnPointerClick(PointerEventData eventData) {
		if (isDummy) return;

		onBtnClick?.Invoke(buildableObject, this);
	}

	public void OnBeginDrag(PointerEventData eventData) {
		if (isDummy) return;

		image.sprite = buildableObject.icon;
		image.enabled = true;

		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint)) {
			dragDrop.anchoredPosition = localPoint;
		}
	}

	public void OnDrag(PointerEventData eventData) {
		if (isDummy) return;

		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint)) {
			dragDrop.anchoredPosition = localPoint;
		}
	}

	public void OnEndDrag(PointerEventData eventData) {
		if (isDummy) return;

		image.enabled = false;
	}
}