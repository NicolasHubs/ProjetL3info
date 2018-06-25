using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class CreateOreDatabase : MonoBehaviour {

	public static OreList asset;

	#if UNITY_EDITOR
	public static OreList createOreDatabase() {
		asset = ScriptableObject.CreateInstance<OreList>();                    

		AssetDatabase.CreateAsset (asset, "Assets/Resources/Databases/OreDatabase.asset");
		AssetDatabase.SaveAssets();     
		return asset;
	}
	#endif
}
