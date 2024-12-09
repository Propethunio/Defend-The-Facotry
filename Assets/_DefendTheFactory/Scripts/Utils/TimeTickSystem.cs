using System;
using UnityEngine;

public class TimeTickSystem : MonoBehaviour {

    public static TimeTickSystem Instance { get; private set; }

    public event Action OnEarlyTick;
    public event Action OnTick;
    public event Action OnLateTick;

    const float TICK_TIMER_MAX = 1f;
    float tickTimer;
    bool isTicking;

    void Awake() {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update() {
        if(!isTicking) return;

        tickTimer += Time.deltaTime;
        if(tickTimer >= TICK_TIMER_MAX) {
            tickTimer -= TICK_TIMER_MAX;
            OnEarlyTick?.Invoke();
            OnTick?.Invoke();
            OnLateTick?.Invoke();
        }
    }

    public void ToggleIsTick() {
        isTicking = !isTicking;
    }

    public void SetIsTicking(bool shouldTick) {
        isTicking = shouldTick;
    }
}