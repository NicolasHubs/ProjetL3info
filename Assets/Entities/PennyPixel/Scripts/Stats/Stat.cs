using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Classe utilisée pour toutes les stats */

[System.Serializable]
public class Stat {

	[SerializeField]
	private int baseValue = 0;	// Valeur de base de la stat

	// Valeurs qui changent la stat, exemple : valeur de l'armure du casque
	private List<int> modifiers = new List<int>();

	public int GetValue () {
		int finalValue = baseValue;
		modifiers.ForEach(x => finalValue += x);
		return finalValue;
	}

	public void SetValue (int newValue) {
		baseValue = newValue;
	}

	public void AddModifier (int modifier) {
		if (modifier != 0)
			modifiers.Add(modifier);
	}

	public void RemoveModifier (int modifier) {
		if (modifier != 0)
			modifiers.Remove(modifier);
	}

}