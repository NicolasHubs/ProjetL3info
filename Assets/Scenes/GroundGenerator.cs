using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GroundGenerator : MonoBehaviour {  

	/*____________ INSPECTOR PARAMETERS ____________*/

	// ------------ LOADING -------------- //
	[Header("Loading bar")]

	public LoadingGroup loadingComponents;

	// ---------- PLANET SPEC ------------ //
	[Header("Spec of the planet")]

	[Range(80,10000)]
	public int numberOfColumns;

	[Tooltip("To set a random seed value, set the seed to 0")]
	public float seed;
	public int heightMultiplier;
	public int heightAddition;
	public float smoothness;
	public int maximalHeight;

	// ------- TILES' BACKGROUND -------- //
	[Header("Background of the tiles")]

	public UnityEngine.Tilemaps.TileBase backgroundTileType;

	// ---------- TILES LIST ------------ //
	[Header("List of the tiles")]

	[Tooltip("The layers are orderly from top to bottom")]
	public List<TerrainType> tilesType;

	public UnityEngine.Tilemaps.TileBase lastTileType;

	// ---------- ORE LIST ------------ //
	[Header("Ores' list")]

	[Tooltip("The ores are orderly from rarest to common")]
	public List<OreType> oresType;


	/*____________ VARIABLES DEFINITION ____________*/

	private GameObject mainCharacter;
	private Camera mainCamera;

	private Vector3Int currentPos;
	private Vector3Int lastPos;

	private int sizeInterpolation;

	private GameObject grid;	
	private UnityEngine.Tilemaps.Tilemap frontGround;
	private UnityEngine.Tilemaps.Tilemap backGround;

	private Vector3Int cellCamPosition;

	[HideInInspector]
	public int[,] mapMatrix;
	[HideInInspector]
	public int[] mapHeight;

	public Vector2Int fieldOfRender;
	private int depthBeforeLastTile;
	private int nbTotalTile;

	// Loading bar struct
	[System.Serializable]
	public struct LoadingGroup {
		public Image backBar;
		public Image frontBar;
		public Text valueTxt;
		public Text infoTxt;
	}

	// Tile spec struct
	[System.Serializable]
	public struct TerrainType {
		[Tooltip("Type of the tile, rock or dirt for example")]
		public UnityEngine.Tilemaps.TileBase type;

		[Tooltip("Represent the depth of the layer")]
		public int depth;
	}

	// Variable utilisée pour le choix du minerai aléatoire
	private float totalRarity;

	// Ore spec struct
	[System.Serializable]
	public struct OreType {
		public UnityEngine.Tilemaps.TileBase type;

		[RangeAttribute(1,2)]
		public float rarity;

		public int depthMin;

		// Valeur que l'on soustrait à la hauteur max de la colonne
		public int depthSubToMaxHeight;
	}

	void Start () {

		// Initialisation des variables et de la grid
		Initialisation ();

		// Generation des vecteurs ainsi que de leur type de tile
		GenerateTileSettings();

		TerrainType lastTile = new TerrainType ();
		TerrainType emptyTile = new TerrainType ();
		emptyTile.type = null;
		lastTile.type = lastTileType;
		tilesType.Insert (0,emptyTile);
		tilesType.Add (lastTile);

		Vector2[] octaveOffsets = new Vector2[3];

		InitVarCaves (ref octaveOffsets, Mathf.RoundToInt(UnityEngine.Random.Range(-100000f, 100000f)), 3,  new Vector2(5f,5f));

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = 400 / 2f;
		float halfHeight = 100 / 2f;

		// Ajout des grottes
		AddPerlinNoiseArea (halfWidth,halfHeight,20f,3,0.289f,3f, octaveOffsets,maxNoiseHeight, minNoiseHeight,0.0f,0.30f, 0, heightAddition/2, depthBeforeLastTile);
		AddPerlinNoiseArea (halfWidth,halfHeight,40f,3,0.439f,3f, octaveOffsets,maxNoiseHeight, minNoiseHeight,0.23f,0.40f, 0,0, heightAddition/2);

		foreach (OreType ore in oresType) {
			nbTotalTile++;
			TerrainType oreTile = new TerrainType ();
			oreTile.type = ore.type;

			octaveOffsets = new Vector2[3];
			InitVarCaves (ref octaveOffsets, Mathf.RoundToInt(UnityEngine.Random.Range(-100000f, 100000f)), 3,  new Vector2(5f,5f));

			AddPerlinNoiseArea (halfWidth,halfHeight,18f,3,0.0f,3f, octaveOffsets,maxNoiseHeight, minNoiseHeight,0.0f,ore.rarity/10f, nbTotalTile, ore.depthMin, ore.depthSubToMaxHeight);

			tilesType.Add (oreTile);
		}

		/* mainCharacter placed at the middle of the world 
		 * in order to generate the world to the left and to the right*/
		float xPos = (mapMatrix.GetLength(0) * frontGround.cellSize.x) / 2;
		int numeroXdanslaliste = frontGround.WorldToCell (new Vector3 (xPos, 0f, 0f)).x;
		int numeroYdanslaliste = mapHeight[numeroXdanslaliste];
		float yPos = numeroYdanslaliste * frontGround.cellSize.y;
		mainCharacter.transform.position = new Vector3 (xPos,yPos, 0f);

		/*
		for (int i = 0; i < mapMatrix.GetLength(0); i++)
			for (int j = mapHeight[i]-1; j >= 0; j--)
				frontGround.SetTile (new Vector3Int (i, j, 0), tilesType [mapMatrix [i, j]].type);
		*/
		//StartCoroutine (LoadGameScene ());
	}

	private void Initialisation(){

		sizeInterpolation = Mathf.RoundToInt(numberOfColumns*0.05f);

		grid = this.gameObject;
		frontGround = GameObject.FindGameObjectsWithTag("FrontGround")[0].gameObject.GetComponent<UnityEngine.Tilemaps.Tilemap>();
		backGround = GameObject.FindGameObjectsWithTag ("BackGround") [0].gameObject.GetComponent<UnityEngine.Tilemaps.Tilemap> ();

		mainCharacter = GameObject.FindGameObjectsWithTag("Player")[0];
		mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();

		currentPos = frontGround.WorldToCell(mainCamera.transform.position);
		lastPos = currentPos;

		if (seed == 0) 
			seed = UnityEngine.Random.Range(-100000f, 100000f);

		mapHeight = new int[numberOfColumns+sizeInterpolation];
		mapMatrix = new int[numberOfColumns+sizeInterpolation,maximalHeight];
		fieldOfRender = new Vector2Int (35, 20);
	}

	private void GenerateTileSettings(){
		depthBeforeLastTile = 0;

		foreach (TerrainType terrain in tilesType)
			depthBeforeLastTile += terrain.depth;

		nbTotalTile = tilesType.Count + 1;

		for (int i = sizeInterpolation; i < numberOfColumns+sizeInterpolation; i++) {
			int height = Mathf.RoundToInt (Mathf.PerlinNoise (seed, i / smoothness) * heightMultiplier) + heightAddition;
			this.mapHeight[i] = height;

			for (int j = height-1; j >= 0; j--) {
				if (j >= height-depthBeforeLastTile && tilesType.Count>0) {
					int depthProgress = 0,numTile = 1;
					foreach (TerrainType tileType in tilesType) {
						depthProgress += tileType.depth;
						if (j >= height - depthProgress) {
							mapMatrix[i,j] = numTile;
							break;
						}
						numTile++;
					}
				} else {
					mapMatrix [i, j] = nbTotalTile;
				}
			}
		}

		int a = this.mapHeight[mapMatrix.GetLength(0)-1];
		int b = this.mapHeight[sizeInterpolation];

		float d = 0.0f;

		float ratioY = Mathf.Abs(b - a);
		float distX = 1.0f / sizeInterpolation;

		for(int i = 0; i < sizeInterpolation; i++){
			float height_f = interpolation_cos(a, b, d);
			height_f *= ratioY;
			height_f += Mathf.Min(a,b);
			d += distX;

			int height = (int) height_f;

			this.mapHeight[i] = height;

			for (int j = height-1; j >= 0; j--) {
				if (j >= height-depthBeforeLastTile && tilesType.Count>0) {
					int depthProgress = 0,numTile = 1;
					foreach (TerrainType tileType in tilesType) {
						depthProgress += tileType.depth;
						if (j >= height - depthProgress) {
							mapMatrix[i,j] = numTile;
							break;
						}
						numTile++;
					}
				} else {
					mapMatrix [i, j] = nbTotalTile;
				}
			}
		}
	}

	float interpolation_cos(float a, float b, float x) {
		if(a < b){
			return (1 - Mathf.Cos(x * Mathf.PI)) / 2;
		} else {
			return (-1 * (1 - Mathf.Cos(x * Mathf.PI)) / 2) + 1;
		}
	}

	void Update (){
		currentPos = frontGround.WorldToCell(mainCamera.transform.position);

		int x = 0;
		int y = 0;

		for(int i = currentPos.x - fieldOfRender.x; i < currentPos.x + fieldOfRender.x; i++){
			if(i < 0) {
				x = Mathf.Abs(i) - 1;
				int quotient = x / mapMatrix.GetLength(0);
				x = (mapMatrix.GetLength(0) - 1) - (x - mapMatrix.GetLength(0) * quotient);
			} else {
				int quotient = i / mapMatrix.GetLength(0);
				x = i - mapMatrix.GetLength(0) * quotient;
			}
			for(int j = currentPos.y - fieldOfRender.y; j < currentPos.y + fieldOfRender.y; j++){

				y = (int) Mathf.Clamp((float)j, 0.0f, (float)(maximalHeight-1));

				frontGround.SetTile(new Vector3Int(i, j, 0), tilesType[mapMatrix[x, y]].type);
				//backGround.SetTile (new Vector3Int (i, j, 0), backgroundTileType);
			}
		}

		lastPos = currentPos;
	}


	private void InitVarCaves(ref Vector2 [] octaveOffsets, int seedC, int octaves, Vector2 offset){
		System.Random prng = new System.Random (seedC);

		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) + offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}
	}

	private void AddPerlinNoiseArea(float halfWidth, float halfHeight, float scale, int octaves, float persistance, float lacunarity, Vector2 [] octaveOffsets, float maxNoiseHeight, float minNoiseHeight, float bornea, float borneb,int blocID, int depthMin, int depthSubToMaxHeight){
		for (int j = 0; j < mapMatrix.GetLength (0); j++) {
			for (int i = depthMin; i < mapHeight[j] - depthSubToMaxHeight; i++) {
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int k = 0; k < octaves; k++) {
					float sampleX = (j - halfWidth) / scale * frequency + octaveOffsets [k].x;
					float sampleY = (i - halfHeight) / scale * frequency + octaveOffsets [k].y;

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxNoiseHeight)
					maxNoiseHeight = noiseHeight;
				else if (noiseHeight < minNoiseHeight)
					minNoiseHeight = noiseHeight;

				float val = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseHeight);

				if (val > bornea && val <= borneb && mapMatrix [j, i]!=0) {
						mapMatrix [j, i] = blocID;
				}
			}
		}
	}



	/*private void addCaves(int octaves, int i, int j, float halfWidth, float halfHeight, float scale, Vector2 [] octaveOffsets, float persistance, float lacunarity, float maxNoiseHeight, float minNoiseHeight, int nbTotalTile){
		float amplitude = 1;
		float frequency = 1;
		float noiseHeightCave = 0;
		float noiseHeightOre = 0;

		for (int k = 0; k < octaves; k++) {
			float sampleXCave = (j - halfWidth) / scaleCave * frequency + octaveOffsets [k].x;
			float sampleYCave = (i - halfHeight) / scaleCave * frequency + octaveOffsets [k].y;

			float sampleXOre = (j - halfWidth) / scaleOre * frequency + octaveOffsets [k].x;
			float sampleYOre = (i - halfHeight) / scaleOre * frequency + octaveOffsets [k].y;


			float perlinValueCave = Mathf.PerlinNoise (sampleXCave, sampleYCave) * 2 - 1;
			//noiseHeightCave += perlinValueCave * amplitude;

			float perlinValueOre = Mathf.PerlinNoise (sampleXOre, sampleYOre) * 2 - 1;
			//noiseHeightOre += perlinValueOre * amplitude;

			amplitude *= persistanceCave;
			frequency *= lacunarity;
		}

		if (noiseHeightCave > maxNoiseHeight)
			maxNoiseHeight = noiseHeightCave;
		else if (noiseHeightCave < minNoiseHeight) 
			minNoiseHeight = noiseHeightCave;

		if (noiseHeightOre > maxNoiseHeight)
			maxNoiseHeight = noiseHeightOre;
		else if (noiseHeightOre < minNoiseHeight) 
			minNoiseHeight = noiseHeightOre;

		float valCave = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseHeightCave);
		float valOre = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseHeightOre);

		if (valCave > 0.4 || valCave <= 0.2) {
			if (valOre <= 0.14) { // Il faut au moins 1 minerai sinon ça n'a pas de sens
				float r = UnityEngine.Random.Range (0f, totalRarity);
				int numTileOre = 2;
				mapMatrix [i, j] = nbTotalTile + numTileOre;
			} else {
				mapMatrix [i, j] = nbTotalTile + 1;
			}
		}
	}*/


	IEnumerator LoadGameScene(){
		loadingComponents.backBar.enabled = true;
		loadingComponents.infoTxt.text = "Loading...";
		while (enabled) {
			float value = frontGround.size.x;
			float progress = value / numberOfColumns;
			loadingComponents.frontBar.fillAmount = progress;
			loadingComponents.valueTxt.text = Mathf.RoundToInt(progress * 100f) + "%";
			yield return null;
		}
		loadingComponents.infoTxt.text = "Done !";
		yield return new WaitForSeconds(3);
		loadingComponents.frontBar.enabled = false;
		loadingComponents.backBar.enabled = false;
		loadingComponents.valueTxt.enabled = false;
		loadingComponents.infoTxt.enabled = false;
	}
}

