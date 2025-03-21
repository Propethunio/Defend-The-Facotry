using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldResourceUI : MonoBehaviour {
    public static WorldResourceUI Instance;

    [SerializeField] private float height;
    [SerializeField] private float startFadeOutTimer;
    [SerializeField] private float fadeTime;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image icon;
    [SerializeField] private Image progressBar;
    [SerializeField] private TMP_Text amountText;

    private Coroutine fadeCoroutine;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void Setup(Sprite sprite, float progress, int amountLeft, Vector3 position) {
        if (fadeCoroutine != null) {
            StopCoroutine(fadeCoroutine);
        }

        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        icon.sprite = sprite;
        progressBar.fillAmount = progress;
        amountText.text = amountLeft.ToString();
        transform.position = position + Vector3.up * height;
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, fadeTime);
    }

    public void OnCursorLeft() {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeAwayAfterDelay());
    }

    private IEnumerator FadeAwayAfterDelay() {
        yield return new WaitForSeconds(startFadeOutTimer);

        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, fadeTime).OnComplete(() => gameObject.SetActive(false));
    }
}