using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherList : ScriptableObject {

	[SerializeField]
	public List<Weather> weatherList = new List<Weather>();

	public List<string> convertToListString()
	{
		List<string> result = new List<string> ();

		foreach (Weather weather in weatherList)
			result.Add (weather.name);

		return new List<string> (result);
	}
}
