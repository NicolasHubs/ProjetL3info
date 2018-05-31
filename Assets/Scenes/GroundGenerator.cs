using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Tilemaps;

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

	public TileBase backgroundTileType;

	// ---------- TILES LIST ------------ //
	[Header("List of the tiles")]

	[Tooltip("The layers are orderly from top to bottom")]
	public List<TerrainType> tilesType;

	public TileBase lastTileType;
	public TileBase unbreakableTileType;

	// ---------- ORE LIST ------------ //
	[Header("Ores' list")]

	[Tooltip("The ores are orderly from rarest to common")]
	public List<OreType> oresType;

	// ----------- CHEST ------------- //
	public int nbChest;
	public TileBase chest;


	/*____________ VARIABLES' LIST ____________*/

	//Cette variable il faudra l'enlever quand on aura fait la bdd de tous les blocs du jeu, elle permet de savoir l'id du bloc incassable
	[HideInInspector]
	public int unbreakableTileID;

	private GameObject mainCharacter;
	private Camera mainCamera;

	private Vector3Int currentPos;

	private int sizeInterpolation;

	//private GameObject grid;	
	[HideInInspector]
	public Tilemap frontGround;
	[HideInInspector]
	public Tilemap backGround;

	private Vector3Int cellCamPosition;

	[HideInInspector]
	public int[,] mapMatrix;
	[HideInInspector]
	public int[] mapHeight;

	[HideInInspector]
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
		public TileBase type;

		[Tooltip("Represent the depth of the layer")]
		public int depth;
	}

	// Variable utilisée pour le choix du minerai aléatoire
	private float totalRarity;

	// Ore spec struct
	[System.Serializable]
	public struct OreType {
		public TileBase type;

		[RangeAttribute(1,2)]
		public float rarity;

		[RangeAttribute(1,3)]
		[Tooltip("Le monde est divisé en 3 zones verticalement pour faciliter le placement des minerais")]
		public int area;
	}

	void Start () {

		// Initialisation des variables et de la grid
		Initialisation ();

		// Generation des vecteurs ainsi que de leur type de tile
		GenerateTileSettings();

		TerrainType lastTile = new TerrainType ();
		TerrainType emptyTile = new TerrainType ();
		TerrainType unbreakableTile = new TerrainType ();
		emptyTile.type = null;
		lastTile.type = lastTileType;
		unbreakableTile.type = unbreakableTileType;
		tilesType.Insert (0,emptyTile);
		tilesType.Add (lastTile);
		tilesType.Add (unbreakableTile);

		nbTotalTile++;
		unbreakableTileID = nbTotalTile;

		// Ajout des tiles indestructibles
		for (int i = 0; i < mapMatrix.GetLength (0); i++)
			mapMatrix [i, 0] = nbTotalTile;

		Vector2[] octaveOffsets = new Vector2[3];

		InitVarCaves (ref octaveOffsets, Mathf.RoundToInt(UnityEngine.Random.Range(-100000f, 100000f)), 3,  new Vector2(5f,5f));

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = 400 / 2f;
		float halfHeight = 100 / 2f;

		// Ajout des grottes
		AddPerlinNoiseCave (halfWidth,halfHeight,20f,3,0.289f,3f, octaveOffsets,maxNoiseHeight, minNoiseHeight,0.0f,0.30f, 0, heightAddition/2, depthBeforeLastTile);
		AddPerlinNoiseCave (halfWidth,halfHeight,40f,3,0.439f,3f, octaveOffsets,maxNoiseHeight, minNoiseHeight,0.23f,0.40f, 0,1, heightAddition/2);

		// Ajout des minerais
		foreach (OreType ore in oresType) {
			nbTotalTile++;
			TerrainType oreTile = new TerrainType ();
			oreTile.type = ore.type;

			octaveOffsets = new Vector2[3];
			InitVarCaves (ref octaveOffsets, Mathf.RoundToInt(UnityEngine.Random.Range(-100000f, 100000f)), 3,  new Vector2(5f,5f));

			AddPerlinNoiseArea (halfWidth,halfHeight,18f,3,0.0f,3f, octaveOffsets,maxNoiseHeight, minNoiseHeight,0.0f,ore.rarity/10f, nbTotalTile, ore.area);

			tilesType.Add (oreTile);
		}

		nbTotalTile++;
		TerrainType chestTile = new TerrainType ();
		chestTile.type = chest;
		tilesType.Add (chestTile);

		int nbTourDeBoucle = 0;
		int startingPos;
		bool chestPosFounded;
		bool restart; //On recherche une nouvelle position pour le coffre actuel car on vient de tomber sur un coffre déjà placé

		// Ajout des coffres
		int numChest = 0;
		while (numChest < nbChest) {
			startingPos = Mathf.RoundToInt(UnityEngine.Random.Range (0, mapMatrix.GetLength(0)-1));

			chestPosFounded = false;
			restart = false;
			for (int i = startingPos; i < mapMatrix.GetLength(0) && !chestPosFounded && !restart; i++) {
				for (int j = 0; j < mapHeight[i] && !chestPosFounded && !restart; j++) {
					if (mapMatrix [i, j] == nbTotalTile) {
						restart = true;
					} else if (mapMatrix [i, j] == 0) {
						mapMatrix [i, j] = nbTotalTile;
						chestPosFounded = true;
						print ("Chest placed !, [x,y] = ["+i+","+j+"]");
					}
				}
			}
			nbTourDeBoucle++;
			if (!restart)
				numChest++;
		}


		/* mainCharacter placed at the middle of the world 
		 * in order to generate the world to the left and to the right*/ 
		float xPos = (mapMatrix.GetLength(0) * frontGround.cellSize.x) / 2;
		int numeroXdanslaliste = frontGround.WorldToCell (new Vector3 (xPos, 0f, 0f)).x;
		int numeroYdanslaliste = mapHeight[numeroXdanslaliste];
		float yPos = numeroYdanslaliste * frontGround.cellSize.y;
		mainCharacter.transform.position = new Vector3 (xPos,yPos, 0f);

		/*
		for (int i = 0; i < mapMatrix.GetLength (0); i++)
			for (int j = mapHeight [i] - 1; j >= 0; j--) {
				frontGround.SetTile (new Vector3Int (i, j, 0), tilesType [mapMatrix [i, j]].type);
				if (j < mapHeight [i] - 1) 
					backGround.SetTile (new Vector3Int (i, j, 0), backgroundTileType);
			}
		*/
		//StartCoroutine (LoadGameScene ());
	}

	private void Initialisation(){
		sizeInterpolation = Mathf.RoundToInt(numberOfColumns*0.05f);

		//grid = this.gameObject;
		frontGround = GameObject.FindGameObjectsWithTag("FrontGround")[0].gameObject.GetComponent<Tilemap>();
		backGround = GameObject.FindGameObjectsWithTag ("BackGround") [0].gameObject.GetComponent<Tilemap>();
		//backGround.color = Color.black;
		mainCharacter = GameObject.FindGameObjectsWithTag("Player")[0];
		mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();

		currentPos = frontGround.WorldToCell(mainCamera.transform.position);
	
		if (seed == 0) 
			seed = UnityEngine.Random.Range(-100000f, 100000f);

		mapHeight = new int[numberOfColumns+sizeInterpolation];
		mapMatrix = new int[numberOfColumns+sizeInterpolation,maximalHeight];
		fieldOfRender = new Vector2Int (40, 20);
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
					Debug.Log (i + " " + j);
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
					Debug.Log (i + " " + j);
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
				if (j >= 0) {
					y = (int) Mathf.Clamp((float)j, 0.0f, (float)(maximalHeight-1));
					frontGround.SetTile (new Vector3Int (i, j, 0), tilesType [mapMatrix [x, y]].type);
					if (j < mapHeight [x] - 1) 
						backGround.SetTile (new Vector3Int (i, j, 0), backgroundTileType);
				}
			}
		}
	}

	private void InitVarCaves(ref Vector2 [] octaveOffsets, int seedC, int octaves, Vector2 offset){
		System.Random prng = new System.Random (seedC);

		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) + offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}
	}

	private void AddPerlinNoiseArea(float halfWidth, float halfHeight, float scale, int octaves, float persistance, float lacunarity, Vector2 [] octaveOffsets, float maxNoiseHeight, float minNoiseHeight, float bornea, float borneb,int blocID, int area){
		for (int j = 0; j < mapMatrix.GetLength (0); j++) {
			for (int i = (mapHeight[j] - Mathf.RoundToInt(area*(mapHeight[j]/3)))+1; i < mapHeight [j] - Mathf.RoundToInt ((area-1) * (mapHeight [j] / 3)); i++) {
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

				if (val > bornea && val <= borneb && mapMatrix [j, i]!=0)
						mapMatrix [j, i] = blocID;
				
			}
		}
	}

	private void AddPerlinNoiseCave(float halfWidth, float halfHeight, float scale, int octaves, float persistance, float lacunarity, Vector2 [] octaveOffsets, float maxNoiseHeight, float minNoiseHeight, float bornea, float borneb,int blocID, int depthMin, int depthSubToMax){
		for (int j = 0; j < mapMatrix.GetLength (0); j++) {
			for (int i = depthMin; i < mapHeight [j] - depthSubToMax; i++) {
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

				if (val > bornea && val <= borneb && mapMatrix [j, i]!=0)
					mapMatrix [j, i] = blocID;

			}
		}
	}

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

