using System;
using UnityEngine;

public class TimeTickSystem : MonoBehaviour {
    public static TimeTickSystem Instance { get; private set; }

    public event Action OnMicroTick;
    public event Action OnEarlyTick;
    public event Action OnTick;
    public event Action OnLateTick;

    private const float MICRO_TICK_TIMER_MAX = 0.1f;
    private const int MICRO_TICKS_FOR_FULL_TICK = 5;

    private bool isTicking;
    private float tickTimer;
    private int productionTicksAmount;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update() {
        if (!isTicking) return;

        tickTimer += Time.deltaTime;

        if (!(tickTimer >= MICRO_TICK_TIMER_MAX)) return;

        tickTimer -= MICRO_TICK_TIMER_MAX;
        productionTicksAmount++;
        OnMicroTick?.Invoke();

        if (productionTicksAmount != MICRO_TICKS_FOR_FULL_TICK) return;

        productionTicksAmount = 0;
        OnEarlyTick?.Invoke();
        OnTick?.Invoke();
        OnLateTick?.Invoke();
    }

    public void ToggleIsTick() {
        isTicking = !isTicking;
    }

    public void SetIsTicking(bool shouldTick) {
        isTicking = shouldTick;
    }
}