using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour {
	[SerializeField] private TMP_Text towersAmountText;
	[SerializeField] private Image healthFill;
	
	private HealthManager healthManager;
	private BuildingSystem buildingSystem;

	private void Start() {
		healthManager = Injector.Resolve<HealthManager>();
		buildingSystem = Injector.Resolve<BuildingSystem>();
		OnTowerAmountChanged(0, buildingSystem.maxTowers);
		Subscribe();
	}

	private void OnDestroy() {
		Unsubscribe();
	}

	private void Subscribe() {
		healthManager.HealthChanged += HealthChanged;
		buildingSystem.TowerAmountChanged += OnTowerAmountChanged;
	}

	private void Unsubscribe() {
		healthManager.HealthChanged -= HealthChanged;
		buildingSystem.TowerAmountChanged -= OnTowerAmountChanged;
	}

	private void HealthChanged(int currentHealth, int maxHealth) {
		healthFill.fillAmount = (float)currentHealth / maxHealth;
	}

	private void OnTowerAmountChanged(int currentTower, int maxTower) {
		towersAmountText.text = currentTower + "/" + maxTower;
	}
}