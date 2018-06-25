using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class CreateFloraDatabase : MonoBehaviour {

	public static FloraList asset;

	#if UNITY_EDITOR
	public static FloraList createFloraDatabase() {
		asset = ScriptableObject.CreateInstance<FloraList>();                    

		AssetDatabase.CreateAsset (asset, "Assets/Resources/Databases/FloraDatabase.asset");
		AssetDatabase.SaveAssets();     
		return asset;
	}
	#endif
}
