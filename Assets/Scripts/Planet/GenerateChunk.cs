using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateChunk : MonoBehaviour {

	public GameObject DirtTile;
	public GameObject GrassTile;
	public GameObject StoneTile;

	public int width;
	public float heightMultiplier;
	public int heightAddition;

	public float smoothness;

	[HideInInspector]
	public float seed;

	/*[HideInInspector]
	public bool isFirstChunk = false;
	public bool isLastChunk = false;*/

	public GameObject tileCoal;
	public GameObject tileDiamond;
	public GameObject tileGold;
	public GameObject tileIron;

	public float chanceCoal;
	public float chanceDiamond;
	public float chanceGold;
	public float chanceIron;

	// private List<List<GameObject>> chunkData = new List<List<GameObject>>(); // this is how you define a 2d generic List.

	void Start () {
		Generate ();
	}

	public void Generate () {
		for (float i = 0.0f; i < width; i+=0.5f) {
			float height = Mathf.Round((Mathf.PerlinNoise (seed, (i + transform.position.x) / smoothness) * heightMultiplier + heightAddition)*100f)/100f;
			// chunkData.Add(new List<GameObject>());
			for (float j = 0.0f; j < height; j+=0.5f) {
				GameObject selectedTile;
				if (j < height - 6) {
					selectedTile = StoneTile;
				} else if (j < height - 0.5f) {
					selectedTile = DirtTile;
				} else {
					selectedTile = GrassTile;
				}
				GameObject newTile = Instantiate (selectedTile, Vector3.zero, Quaternion.identity) as GameObject;
				newTile.transform.parent = this.gameObject.transform;
				newTile.transform.localScale = new Vector3 (0.5f, 0.5f);
				newTile.transform.localPosition = new Vector3 (i, j);
				/*if (selectedTile == GrassTile && ((isFirstChunk && i == 0.0f) || (isLastChunk && i+0.5f >= width))) {
					newTile.AddComponent<TeleportEntity> ();
					newTile.GetComponent<BoxCollider2D> ().isTrigger = true;
					newTile.GetComponent<BoxCollider2D> ().size = new Vector2 (2f,100f);
					newTile.GetComponent<TeleportEntity> ().x = 60f;
					newTile.GetComponent<TeleportEntity> ().y = 40f;

				}*/

				/*if (selectedTile == StoneTile)
					chunkData[i].Add(newTile);*/
			}
		}
		// AddCaves ();
		AddRessources();
	}

	public void AddRessources() {
		foreach(GameObject t in GameObject.FindGameObjectsWithTag("TileStone")){
			if (t.transform.parent == this.gameObject.transform) {
				float r = Random.Range (0f, 100f);
				GameObject selectedTile = null;

				if (r < chanceDiamond) {
					selectedTile = tileDiamond;
				} else if (r < chanceGold) {
					selectedTile = tileGold;
				} else if (r < chanceIron) {
					selectedTile = tileIron;
				} else if (r < chanceCoal) {
					selectedTile = tileCoal;
				}

				if (selectedTile != null) {
					GameObject stoneTileToChange = Instantiate (selectedTile, t.transform.position, Quaternion.identity) as GameObject;
					stoneTileToChange.transform.parent = this.gameObject.transform;
					stoneTileToChange.transform.localScale = new Vector3 (0.5f, 0.5f);
					Destroy (t);
				}
			}
		}
	}
	/*
	public void AddCaves() {

		SimplexNoiseGenerator noise = new SimplexNoiseGenerator();
		string noise_seed = noise.GetSeed();
		Vector3 coords = Vector3.one;

		int octaves = 6;
		int multiplier = 25;
		float amplitude = 0.5f;
		float lacunarity = 2.0f;
		float persistence = 0.5f;

		for (int i=0;i<=chunkData.Count;i++){
			for (int j=0;j<=chunkData[i].Count;j++){
				float perlin = noise.coherentNoise(i,j,0,octaves,multiplier,amplitude,lacunarity,persistence);
				if (perlin < -0.05)
					Destroy (chunkData[i][j]);
			}
		}
	}*/
}
