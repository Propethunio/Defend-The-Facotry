using System;

public class HealthManager {
	private int maxHealth;
	private int currentHealth;
	
	public event Action<int, int> HealthChanged;
	public event Action GameOver;

	public void SetStartHealth(int health) {
		maxHealth = health;
		currentHealth = health;
	}

	public void Damage(int dmg) {
		currentHealth -= dmg;
		HealthChanged?.Invoke(currentHealth, maxHealth);

		if (currentHealth <= 0) {
			GameOver?.Invoke();
		}
	}
}