using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[System.Serializable]
public class StellarSystem{

	public string name;
	public List<Planet> planetList;

	public StellarSystem(string name, List<Planet> planetList) {
		this.name = name;
		this.planetList = planetList;
	}

	public StellarSystem() {
		name = "";
		planetList = new List<Planet>();
	}

	public StellarSystem clone(){
		return new StellarSystem ((string)name.Clone(), planetList);
	}
}
