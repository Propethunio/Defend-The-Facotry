using DG.Tweening;
using UnityEngine;

public class WorldItem : Flyweight {
	public ItemSO itemSO { get; private set; }

	private FlyweightFactory factory;
	private Tween moveTween;

	private void Start() {
		factory = Injector.Resolve<FlyweightFactory>();
		itemSO = (ItemSO)settings;
	}

	public void MoveToPosition(Vector2 worldPosition) {
		KillActiveTween();
		moveTween = transform.DOMove(new Vector3(worldPosition.x, transform.position.y, worldPosition.y), .5f).SetEase(Ease.Linear);
	}

	public void MoveToPositionCurved(Vector2 worldPosition) {
		KillActiveTween();
		Vector3 startPosition = transform.position;
		Vector3 controlPoint = new Vector3(Mathf.Floor(startPosition.x) + .5f, startPosition.y, Mathf.Floor(startPosition.z) + .5f);
		Vector3 endPosition = new Vector3(worldPosition.x, startPosition.y, worldPosition.y);
		Vector3 midPoint = (startPosition + endPosition) / 2f;
		moveTween = transform.DOPath(new Vector3[] { startPosition, midPoint, endPosition }, 0.5f, PathType.CatmullRom).SetEase(Ease.InOutSine).SetLookAt(0.01f);
	}

	public void DestroySelf() {
		KillActiveTween();
		factory.ReturnToPool(this);
	}

	private void KillActiveTween() {
		if (moveTween?.IsActive() == true) {
			moveTween.Kill();
		}
	}
}