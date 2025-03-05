using System;
using UnityEngine;

public class TimeTickSystem : MonoBehaviour {

    public static TimeTickSystem Instance { get; private set; }

    public event Action OnProductionTick;
    public event Action OnSubTick;
    public event Action OnEarlyTick;
    public event Action OnTick;
    public event Action OnLateTick;

    const float TICK_TIMER_MAX = 0.1f;
    const int PRODUCTION_TICKS_MAX = 10;

    bool isTicking;
    float tickTimer;
    int ticksAmount;
    int amountForSubTick;

    void Awake() {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        amountForSubTick = PRODUCTION_TICKS_MAX / 2;
    }

    void Update() {
        if(!isTicking) return;

        tickTimer += Time.deltaTime;

        if(tickTimer >= TICK_TIMER_MAX) {
            tickTimer -= TICK_TIMER_MAX;
            ticksAmount++;
            OnProductionTick?.Invoke();

            if(ticksAmount == amountForSubTick) {
                OnSubTick?.Invoke();
            }

            if(ticksAmount == PRODUCTION_TICKS_MAX) {
                ticksAmount = 0;
                OnEarlyTick?.Invoke();
                OnTick?.Invoke();
                OnLateTick?.Invoke();
            }
        }
    }

    public void ToggleIsTick() {
        isTicking = !isTicking;
    }

    public void SetIsTicking(bool shouldTick) {
        isTicking = shouldTick;
    }
}