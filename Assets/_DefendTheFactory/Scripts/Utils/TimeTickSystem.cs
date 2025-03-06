using System;
using UnityEngine;

public class TimeTickSystem : MonoBehaviour {

    public static TimeTickSystem Instance { get; private set; }

    public event Action OnProductionTick;
    public event Action OnEarlyTick;
    public event Action OnTick;
    public event Action OnLateTick;

    const float PRODUCTION_TICK_TIMER_MAX = 0.1f;
    const int PRODUCTION_TICKS_FOR_FULL_TICK = 5;

    bool isTicking;
    float tickTimer;
    int productionTicksAmount;

    void Awake() {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update() {
        if(!isTicking) return;

        tickTimer += Time.deltaTime;

        if(tickTimer >= PRODUCTION_TICK_TIMER_MAX) {
            tickTimer -= PRODUCTION_TICK_TIMER_MAX;
            productionTicksAmount++;
            OnProductionTick?.Invoke();

            if(productionTicksAmount == PRODUCTION_TICKS_FOR_FULL_TICK) {
                productionTicksAmount = 0;
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