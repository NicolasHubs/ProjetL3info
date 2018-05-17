using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GroundGenerator : MonoBehaviour {  
	
	/*____________ INSPECTOR PARAMETERS ____________*/

	// ----------- CHARACTER ------------- //
	[Header("Character")]
	public GameObject camMainCharacter;

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

	// ---------- TILES LIST ------------ //
	[Header("List of the tiles")]

	[Tooltip("The layers are orderly from top to bottom")]
	public List<TerrainType> tilesType;

	[Space(10)]
	public UnityEngine.Tilemaps.TileBase lastTileType;


	/*____________ VARIABLES DEFINITION ____________*/

	private GameObject groundGrid;	
	private GameObject frontground;

	private Vector3Int cellCamPosition;

	private int[,] mapMatrix;

	public void setMatrix(int x, int y, int val){
		mapMatrix [x, y] = val;
	}

	public void printTab(){
		for (int i = 0; i < numberOfColumns/4; i++)
			for (int j = heightMap [i] - 1; j >= 0; j--)
				print(mapMatrix [i, j]); 

	}
	public int getMatrix(int x, int y){
		return mapMatrix[x,y];
	}

    private UnityEngine.Tilemaps.Tilemap world;

	private int[] heightMap;

	private Vector2Int fieldOfRender;

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

	void Start () {

		// Initialisation des variables et de la grid
		Initialisation ();

		// Generation des vecteurs ainsi que de leur type de tile
		GenerateTileSettings(400,100,5,50f,3,0.439f,3f, new Vector2(5f,5f));

		TerrainType lastTile = new TerrainType ();
		TerrainType emptyTile = new TerrainType ();
		emptyTile.type = null;
		lastTile.type = lastTileType;
		tilesType.Insert (0, emptyTile);
		tilesType.Add (lastTile);

		/* mainCharacter placed at the middle of the world 
		 * in order to generate the world to the left and to the right*/
		float xPos = (numberOfColumns * world.cellSize.x) / 2;
		int numeroXdanslaliste = world.WorldToCell (new Vector3 (xPos, 0f, 0f)).x;
		int numeroYdanslaliste = heightMap[numeroXdanslaliste];
		float yPos = numeroYdanslaliste * world.cellSize.y;
		camMainCharacter.transform.position = new Vector3 (xPos,yPos, 0f);

		cellCamPosition = world.WorldToCell (camMainCharacter.transform.position);

		for (int i = cellCamPosition.x - fieldOfRender.x; i < cellCamPosition.x + fieldOfRender.x; i++)
			for (int j = cellCamPosition.y - fieldOfRender.y; j < cellCamPosition.y + fieldOfRender.y; j++)
					world.SetTile (new Vector3Int (i, j, 0), tilesType [mapMatrix [i, j]].type);
			
		//StartCoroutine (LoadGameScene ());
	}

	private void Initialisation(){
		groundGrid = this.gameObject;
		frontground = groundGrid.transform.Find ("Frontground").gameObject;

		if (seed == 0) 
			seed = UnityEngine.Random.Range(-100000f, 100000f);

		world = frontground.GetComponent<UnityEngine.Tilemaps.Tilemap> ();

		heightMap = new int[numberOfColumns];
		mapMatrix = new int[numberOfColumns,maximalHeight];
		fieldOfRender = new Vector2Int (35, 20);
	}

	private void GenerateTileSettings(int mapWidth, int mapHeight, int seedCave, float scale, int octaves, float persistance, float lacunarity, Vector2 offset){
		Vector2[] octaveOffsets = new Vector2[octaves];

		InitVarCaves (ref octaveOffsets, seedCave, octaves, offset, scale);

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;

		int depthBeforeLastTile = 0;

		foreach (TerrainType terrain in tilesType)
			depthBeforeLastTile += terrain.depth;

		int nbTotalTile = tilesType.Count;

		for (int i = 0; i < numberOfColumns; i++) {
			int height = Mathf.RoundToInt (Mathf.PerlinNoise (seed, i / smoothness) * heightMultiplier) + heightAddition;
			heightMap[i] = height;

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
					//addCaves (octaves, i, j, halfWidth, halfHeight, scale, octaveOffsets, persistance, lacunarity, maxNoiseHeight, minNoiseHeight, nbTotalTile);
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

					if (val > 0.4 || val <= 0.2)
						mapMatrix [i, j] = nbTotalTile+1;
				}
			}
		}
	}

	void Update (){
		
		cellCamPosition = world.WorldToCell (camMainCharacter.transform.position);

		int x = 0;
		int y = 0;

		for(int i = cellCamPosition.x - fieldOfRender.x; i < cellCamPosition.x + fieldOfRender.x; i++){
			if(i < 0) {
				x = Mathf.Abs(i) - 1;
				int quotient = x / numberOfColumns;
				x = (numberOfColumns - 1) - (x - numberOfColumns * quotient);
			} else {
				int quotient = i / numberOfColumns;
				x = i - numberOfColumns * quotient;
			}

			for(int j = cellCamPosition.y - fieldOfRender.y; j < cellCamPosition.y + fieldOfRender.y; j++){
				y = (int) Mathf.Clamp((float)j, 0.0f, (float)(maximalHeight-1));

				world.SetTile(new Vector3Int(i, j, 0), tilesType[mapMatrix[x,y]].type);
			}
		}

	}

	private void InitVarCaves(ref Vector2 [] octaveOffsets, int seed, int octaves, Vector2 offset, float scale){
		System.Random prng = new System.Random (seed);

		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) + offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}

		if (scale <= 0)
			scale = 0.0001f;
	}

	private void addCaves(int octaves, int i, int j, float halfWidth, float halfHeight, float scale, Vector2 [] octaveOffsets , float persistance, float lacunarity, float maxNoiseHeight, float minNoiseHeight, int nbTotalTile){
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

		if (val > 0.4 || val <= 0.2)
			mapMatrix [i, j] = nbTotalTile+1;
	}

	IEnumerator LoadGameScene(){
		loadingComponents.backBar.enabled = true;
		loadingComponents.infoTxt.text = "Loading...";
		while (enabled) {
			float value = world.size.x;
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

