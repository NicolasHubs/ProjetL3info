using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
public class CreateGalaxyDatabase : MonoBehaviour {

	public static GalaxyList asset;

	public static GalaxyList createGalaxyDatabase() {
		asset = ScriptableObject.CreateInstance<GalaxyList>();                    

		AssetDatabase.CreateAsset (asset, "Assets/Resources/Databases/GalaxyDatabase.asset");
		AssetDatabase.SaveAssets();     
		return asset;
	}
}

#endif