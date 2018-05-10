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
	public bool special;
	public float a;
	public float b;

	public GameObject tileCoal;
	public GameObject tileDiamond;
	public GameObject tileGold;
	public GameObject tileIron;

	public GameObject column;

	public float chanceCoal;
	public float chanceDiamond;
	public float chanceGold;
	public float chanceIron;

	void Awake () {
		seed = GameObject.Find("Planet").GetComponent<GenerateChunks>().seed;
		special = GameObject.Find("Planet").GetComponent<GenerateChunks>().special;
		Generate ();
	}

	public void Generate () {

		if(this.special){
			a = GameObject.Find("Planet").GetComponent<GenerateChunks>().a;
			b = GameObject.Find("Planet").GetComponent<GenerateChunks>().b;

			float d = 0.0f;

			float ratioY = Mathf.Abs(b - a);
			float distX = 1.0f / width * 0.5f;

			for (float i = 0.0f; i < width; i+=0.5f) {
				float height = interpolation_cos(a, b, d);
				height *= ratioY;
				height += Mathf.Min(a,b);
				d += distX;

				GameObject columne = Instantiate (column, Vector3.zero, Quaternion.identity) as GameObject;
				columne.transform.parent = this.gameObject.transform;
				columne.name = "column";
				columne.transform.localPosition = new Vector3 (i, 0.0f);

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
					newTile.transform.parent = columne.transform;
					newTile.transform.localScale = new Vector3 (0.5f, 0.5f);
					newTile.transform.localPosition = new Vector3 (0.0f, j);
				}
			}
		} else {
			for (float i = 0.0f; i < width; i+=0.5f) {
				float height = Mathf.Round((Mathf.PerlinNoise (seed, (i + transform.position.x) / smoothness) * heightMultiplier + heightAddition)*100f)/100f;

				if(i == 0.0f){
					a = height;
				} else if (i == width-0.5f) {
					b = height;
				}
				GameObject columne = Instantiate (column, Vector3.zero, Quaternion.identity) as GameObject;
				columne.transform.parent = this.gameObject.transform;
				columne.transform.localPosition = new Vector3 (i, 0.0f);

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
					newTile.transform.parent = columne.transform;
					newTile.transform.localScale = new Vector3 (0.5f, 0.5f);
					newTile.transform.localPosition = new Vector3 (0.0f, j);
				}
			}
		}
		// AddCaves ();
		AddRessources();
	}

	float interpolation_cos(float a, float b, float x) {
		if(a < b){
			return (1 - Mathf.Cos(x * Mathf.PI)) / 2;
		} else {
			return (-1 * (1 - Mathf.Cos(x * Mathf.PI)) / 2) + 1;
		}
	}
	
	public void AddRessources() {
		foreach(GameObject t in GameObject.FindGameObjectsWithTag("TileStone")){
			if (t.transform.parent.parent == this.gameObject.transform) {
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
					stoneTileToChange.transform.parent = t.transform.parent;
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
