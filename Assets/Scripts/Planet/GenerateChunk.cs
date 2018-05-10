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
			AddCaves (100,100,0,20f,5,0.5f,2f, new Vector2(0f,0f));
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
		//AddRessources();
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

	public void AddCaves(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
		System.Random prng = new System.Random (seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) + offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}

		if (scale <= 0) {
			scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;

		int x = 0, y = 0;
		GameObject t;
		foreach (GameObject tutu in GameObject.FindGameObjectsWithTag("Column")){
			
			foreach (Transform child in tutu.transform) {
				
				t = child.gameObject;
				if (child.tag == "TileStone") {
					x++;

					float amplitude = 1;
					float frequency = 1;
					float noiseHeight = 0;

					for (int i = 0; i < octaves; i++) {
						float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets [i].x;
						float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets [i].y;

						float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
						noiseHeight += perlinValue * amplitude;

						amplitude *= persistance;
						frequency *= lacunarity;
					}

					if (noiseHeight > maxNoiseHeight) {
						maxNoiseHeight = noiseHeight;
					} else if (noiseHeight < minNoiseHeight) {
						minNoiseHeight = noiseHeight;
					}

					if (Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseHeight) <= 0.4) {
						Destroy (child.gameObject);
					}
				}
			}
			x = 0;
			y++;
		}
	}
}
