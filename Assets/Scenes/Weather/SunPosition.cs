using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunPosition : MonoBehaviour {
	
	private Vector3 playerPosition;
	//private GroundGenerator scriptGen;
	private PlanetGenerator scriptGen;
	private Transform grid;
	private Vector3 sunNextPosition;

	void Start () {
		Light sunLight = this.GetComponent<Light> ();
		sunLight.range = 20;
		sunLight.intensity = 800;

		grid = GameObject.FindGameObjectWithTag ("FrontGround").transform.parent;
		//scriptGen = grid.GetComponent<GroundGenerator> ();
		scriptGen = grid.GetComponent<PlanetGenerator> ();
	}

	void Update () {
		playerPosition = GameObject.FindGameObjectWithTag ("Player").transform.position;
		int x = scriptGen.frontGround.WorldToCell (playerPosition).x;
		x = ((x % scriptGen.planet.savedMapMatrix.GetLength(0)) + scriptGen.planet.savedMapMatrix.GetLength(0)) % scriptGen.planet.savedMapMatrix.GetLength(0);
		sunNextPosition = new Vector3 (playerPosition.x, scriptGen.planet.savedMapHeight [x] * scriptGen.frontGround.cellSize.x + (Mathf.Abs(scriptGen.planet.savedMapHeight [x] * scriptGen.frontGround.cellSize.x-playerPosition.y))+9, -1);

		this.transform.position = sunNextPosition;
	}
}
