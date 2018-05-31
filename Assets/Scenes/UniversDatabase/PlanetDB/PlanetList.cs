using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetList : ScriptableObject {

	[SerializeField]
	public List<Planet> planetList = new List<Planet>();
}
