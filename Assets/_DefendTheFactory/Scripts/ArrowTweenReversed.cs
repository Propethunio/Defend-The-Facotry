using UnityEngine;
using DG.Tweening;

public class ArrowTweenReversed : MonoBehaviour {

    private void Start() {
        transform.DOLocalMove(transform.localPosition + transform.parent.InverseTransformDirection(-transform.forward.normalized) * .1f, .75f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}