using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {
    [SerializeField] private Slider loadingBar;
    [SerializeField] private float baseFillSpeed;
    [SerializeField] private float maxFillSpeed;
    [SerializeField] private float minProgressDifferenceForJump;
    [SerializeField] private float minTimeBetweenJumpsInValue;
    [SerializeField] private float maxJumpValue;

    private float targetProgress;
    private float timerForJumpInValue;
    private bool canJumpInValue = true;

    private void Update() {
        float currentFillAmount = loadingBar.value;
        float progressDifference = Mathf.Abs(currentFillAmount - targetProgress);
        float dynamicFillSpeed = progressDifference * baseFillSpeed;
        dynamicFillSpeed = Mathf.Clamp(dynamicFillSpeed, 1f, maxFillSpeed);
        loadingBar.value = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);

        if (canJumpInValue) {
            if (!(progressDifference >= minProgressDifferenceForJump)) return;

            canJumpInValue = false;
            float randomValue = Random.Range(0f, maxJumpValue);
            loadingBar.value += randomValue;
        }
        else {
            timerForJumpInValue += Time.deltaTime;

            if (!(timerForJumpInValue >= minTimeBetweenJumpsInValue)) return;

            timerForJumpInValue = 0f;
            canJumpInValue = true;
        }
    }

    public void UpdateTargetValue(float targetValue) {
        targetProgress = targetValue;
    }
}