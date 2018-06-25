using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weather {

	public string name;

	public Weather(string name) {
		this.name = name;
	}

	public Weather() { 
		name = "";
	}

	public Weather clone(){
		return new Weather ((string)name.Clone ());
	}
}
