using System;
using TMPro;
using UnityEngine;

public class testowyUI : MonoBehaviour {
    [SerializeField] private TMP_Text timerTxt;

    private WaveManager waveManager;
    private bool isNight;
    
    private void Start() {
        waveManager = Injector.Resolve<WaveManager>();
        waveManager.OnWaveTick += onTick;
        waveManager.OnNightActive += onNight;
    }

    private void onTick(int amount) {
        if (isNight) {
            timerTxt.text = "Enemies to spawn left: " + amount;
        }
        else {
            timerTxt.text = "Time left until night: " + amount;
        }
    }

    private void onNight(bool b) {
        isNight = b;
    }
}
