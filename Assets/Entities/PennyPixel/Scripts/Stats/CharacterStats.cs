using UnityEngine;

/* Classe de base dont le joueur et les ennemis peuvent dériver pour inclure des statistiques. */

public class CharacterStats : MonoBehaviour {
	
	// Vie
	[HideInInspector]
	public int maxHealth = 100;
	[HideInInspector]
	public Stat currentHealth;

	// Dommages
	//public int maxDamage = 100;
	[HideInInspector]
	public Stat currentDamage;

	// Armure
	[HideInInspector]
	public int maxArmor = 100;
	[HideInInspector]
	public Stat currentArmor;

	// Définit les stats au max au démarrage du jeu.
	void Awake () {
		currentHealth.SetValue(maxHealth);
		currentDamage.SetValue(20);
	}

	// Inflige des dommages au character
	public void TakeDamage (int damage) {
		
		// Calcule les dommages subis 
		currentHealth.SetValue(currentHealth.GetValue() - (damage - ((currentArmor.GetValue() * damage) / 100)));

		Debug.Log(transform.name + " takes " + damage + " damage.");

		// Si la vie est inferieure à 0
		if (currentHealth.GetValue() <= 0) {
			Die();
		}
	}

	public virtual void Die () {
		
		// Cette méthode est destinée à etre overwritten
		Debug.Log(transform.name + " died.");
	}

}