using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class CreateGalaxyDatabase : MonoBehaviour {

	public static GalaxyList asset;

	#if UNITY_EDITOR
	public static GalaxyList createGalaxyDatabase() {
		asset = ScriptableObject.CreateInstance<GalaxyList>();                    

		AssetDatabase.CreateAsset (asset, "Assets/Resources/Databases/GalaxyDatabase.asset");
		AssetDatabase.SaveAssets();     
		return asset;
	}
	#endif
}
