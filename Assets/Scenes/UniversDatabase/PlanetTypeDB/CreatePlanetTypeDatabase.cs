using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class CreatePlanetTypeDatabase : MonoBehaviour {

	public static PlanetTypeList asset;

	#if UNITY_EDITOR
	public static PlanetTypeList createPlanetTypeDatabase() {
		asset = ScriptableObject.CreateInstance<PlanetTypeList>();                    

		AssetDatabase.CreateAsset (asset, "Assets/Resources/Databases/PlanetTypeDatabase.asset");
		AssetDatabase.SaveAssets();     
		return asset;
	}
	#endif
}
