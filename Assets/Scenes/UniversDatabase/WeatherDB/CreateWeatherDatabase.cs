using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class CreateWeatherDatabase : MonoBehaviour {

	public static WeatherList asset;

	#if UNITY_EDITOR
	public static WeatherList createWeatherDatabase() {
		asset = ScriptableObject.CreateInstance<WeatherList>();                    

		AssetDatabase.CreateAsset (asset, "Assets/Resources/Databases/WeatherDatabase.asset");
		AssetDatabase.SaveAssets();     
		return asset;
	}
	#endif
}
