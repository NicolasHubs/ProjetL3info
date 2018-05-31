using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[System.Serializable]
public class Galaxy{

	public string name;
	public List<StellarSystem> stellarSystemList;

	public Galaxy(string name, List<StellarSystem> stellarSystemList) {
		this.name = name;
		this.stellarSystemList = stellarSystemList;
	}

	public Galaxy() {
		name = "";
		stellarSystemList = new List<StellarSystem>();
	}

	public Galaxy clone(){
		return new Galaxy ((string)name.Clone(), stellarSystemList);
	}
}
