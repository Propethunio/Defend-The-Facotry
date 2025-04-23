using UnityEngine;
using UnityEngine.UI;

public class EnemyBar : MonoBehaviour {
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider shieldBar;

    private void Start() {
        //healthBar.gameObject.SetActive(false);
        //shieldBar.gameObject.SetActive(false);
    }

    public void SetHealthBar(float value) {
        healthBar.gameObject.SetActive(true);
        healthBar.value = value;
    }

    public void SetShieldBar(float value) {
        shieldBar.gameObject.SetActive(true);
        shieldBar.value = value;
        if(value == 0) {
            healthBar.gameObject.SetActive(false);
        }
    }
}