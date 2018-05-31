using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class CreatePlanetDatabase : MonoBehaviour {

	public static PlanetList asset;

	#if UNITY_EDITOR
	public static PlanetList createPlanetDatabase() {
		asset = ScriptableObject.CreateInstance<PlanetList>();                    

		AssetDatabase.CreateAsset (asset, "Assets/Scenes/UniversDatabase/Resources/PlanetDatabase.asset");
		AssetDatabase.SaveAssets();     
		return asset;
	}
	#endif
}
