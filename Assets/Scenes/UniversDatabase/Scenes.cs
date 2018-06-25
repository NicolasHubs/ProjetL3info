using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public static class Scenes {

	private static Planet planet;

	#if UNITY_EDITOR
	public static void Load(string sceneName, Planet planetParam) {
		Scenes.planet = new Planet ();
		Scenes.planet = planetParam.clone();
		SceneManager.LoadScene(sceneName);
	}
	#endif

	public static Planet getSceneParameter() {
		return planet;
	}

	public static void setSceneParameter(Planet planetParam) {
		if (planet == null)
			Scenes.planet = new Planet ();
		Scenes.planet = planetParam.clone();
	}
}