using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public static class Scenes {

	private static Planet planet;

	public static void Load(string sceneName, Planet planetParam) {
		Scenes.planet = new Planet ();
		Scenes.planet = planetParam.clone();
		SceneManager.LoadScene(sceneName);
	}

	public static void Load(string sceneName) {
		SceneManager.LoadScene(sceneName);
	}

	public static AsyncOperation LoadAsync(string sceneName, Planet planetParam) {
		Scenes.planet = new Planet ();
		Scenes.planet = planetParam.clone();
		return SceneManager.LoadSceneAsync(sceneName);
	}

	public static Planet getSceneParameter() {
		return planet;
	}

	public static void setSceneParameter(Planet planetParam) {
		if (planet == null)
			Scenes.planet = new Planet ();
		Scenes.planet = planetParam.clone();
	}
}