using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Tilemaps;

public class PlanetGenerator : MonoBehaviour {  

	/*____________ VARIABLES' LIST ____________*/

	//public GameObject frontgroundModel;
	//public GameObject backgroundModel;
	[HideInInspector]
	public Planet planet;

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

	private int nbTotalTile;
	private Vector2Int fieldOfRender;

	[HideInInspector]
	public int maximalHeight;

	void Awake() {
		planet = Scenes.getSceneParameter ();
	}

	void Start(){
		world ();
	}

	public void world (){
		// Initialisation des variables et de la grid
		Initialisation ();

		if (planet.savedMapHeight.GetLength (0) == 0 || planet.savedMapMatrix == null) {
			planet.savedMapHeight = new int[planet.horizontalSize + sizeInterpolation];
			planet.savedMapMatrix = new int[planet.horizontalSize + sizeInterpolation, maximalHeight];

			// Generation des vecteurs ainsi que de leur type de tile
			GenerateTileSettings ();
		
			planet.tilesType = new List<TileBase>();
			planet.tilesType.Insert (0,(TileBase)null);
			planet.tilesType.Add (planet.planetType.ruleTile);
			planet.tilesType.Add (planet.planetType.unbreakableTile);

			unbreakableTileID = 2;

			// Ajout des tiles indestructibles
			for (int i = 0; i < planet.savedMapMatrix.GetLength (0); i++)
				planet.savedMapMatrix [i, 0] = unbreakableTileID;

			Vector2[] octaveOffsets = new Vector2[3];

			// TODO : AJOUTER SEED CAVE A PLANET
			InitVarCaves (ref octaveOffsets, planet.seedCave, 3,  new Vector2(5f,5f));

			float maxNoiseHeight = float.MinValue;
			float minNoiseHeight = float.MaxValue;

			float halfWidth = 400 / 2f;
			float halfHeight = 100 / 2f;

			// Ajout des grottes
			AddPerlinNoiseCave (halfWidth,halfHeight,20f,3,0.289f,3f, octaveOffsets,maxNoiseHeight, minNoiseHeight,0.0f,0.30f, 0, planet.verticalSize/2, Mathf.RoundToInt(planet.verticalSize*0.05f));
			AddPerlinNoiseCave (halfWidth,halfHeight,planet.caveWidth,3,0.439f,3f, octaveOffsets,maxNoiseHeight, minNoiseHeight,0.23f,planet.caveQuantity, 0,1, planet.verticalSize/2);

			// Ajout des minerais
			foreach (Ore ore in planet.oreList) {
				octaveOffsets = new Vector2[3];
				InitVarCaves (ref octaveOffsets, ore.seedDeposit, 3,  new Vector2(5f,5f));
				planet.tilesType.Add (ore.tile);
				AddPerlinNoiseArea (halfWidth,halfHeight,ore.depositWidth,3,0.0f,3f, octaveOffsets,maxNoiseHeight, minNoiseHeight,0.0f,ore.depositRarity/10f, planet.tilesType.Count-1, ore.area);
			}

			planet.tilesType.Add (planet.planetType.chestSprite);

			int nbTourDeBoucle = 0;
			int startingPos;
			bool chestPosFounded;
			bool restart;

			// Ajout des coffres
			int numChest = 0;
			while (numChest < planet.numberOfChest) {
				startingPos = Mathf.RoundToInt(UnityEngine.Random.Range (0, planet.savedMapMatrix.GetLength(0)-1));

				chestPosFounded = false;
				restart = false;
				for (int i = startingPos; i < planet.savedMapMatrix.GetLength(0) && !chestPosFounded && !restart; i++) {
					for (int j = 0; j < planet.savedMapHeight[i] && !chestPosFounded && !restart; j++) {
						if (planet.savedMapMatrix [i, j] == planet.tilesType.Count-1) {
							restart = true;
						} else if (planet.savedMapMatrix [i, j] == 0) {
							planet.savedMapMatrix [i, j] = planet.tilesType.Count-1;
							chestPosFounded = true;
							//print ("Chest placed !, [x,y] = ["+i+","+j+"]");
						}
					}
				}
				nbTourDeBoucle++;
				if (!restart)
					numChest++;
			}

		}

		float xPos = (planet.savedMapMatrix.GetLength(0) * frontGround.cellSize.x) / 2;
		int numeroXdanslaliste = frontGround.WorldToCell (new Vector3 (xPos, 0f, 0f)).x;
		int numeroYdanslaliste = planet.savedMapHeight[numeroXdanslaliste];
		float yPos = numeroYdanslaliste * frontGround.cellSize.y;

		mainCharacter.transform.position = new Vector3 (xPos,yPos, 0f);

		/*
		for (int i = 0; i < planet.savedMapMatrix.GetLength (0); i++)
			for (int j = planet.savedMapHeight [i] - 1; j >= planet.savedMapHeight [i] - fieldOfRender.y; j--) {
				frontGround.SetTile (new Vector3Int (i, j, 0), planet.tilesType [planet.savedMapMatrix [i, j]]);
				if (j < planet.savedMapHeight [i] - 1)
					backGround.SetTile (new Vector3Int (i, j, 0), planet.planetType.backgroundTile);
			}
			*/

	}

	private void Initialisation(){
		sizeInterpolation = Mathf.RoundToInt(planet.horizontalSize*0.05f);

		//grid = this.gameObject;
		frontGround = GameObject.FindGameObjectsWithTag("FrontGround")[0].gameObject.GetComponent<Tilemap>();

		backGround = GameObject.FindGameObjectsWithTag ("BackGround") [0].gameObject.GetComponent<Tilemap>();
		//backGround.color = Color.black;
		mainCharacter = GameObject.FindGameObjectsWithTag("Player")[0];
		mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();

		currentPos = frontGround.WorldToCell(mainCamera.transform.position);

		maximalHeight = planet.verticalSize + planet.heightMultiplier*2;

		GameObject.FindGameObjectWithTag ("BackgroundHandler").GetComponent<CycleJourNuit> ().speed = planet.daySpeedMultiplier;

		fieldOfRender = new Vector2Int (40, 20);
	}

	private void GenerateTileSettings(){
		for (int i = sizeInterpolation; i < planet.horizontalSize+sizeInterpolation; i++) {
			int height = Mathf.RoundToInt (Mathf.PerlinNoise (planet.seed, i / planet.smoothness) * planet.heightMultiplier) + planet.verticalSize;
			this.planet.savedMapHeight[i] = height;

			for (int j = height-1; j >= 0; j--) 
				planet.savedMapMatrix [i, j] = 1;
		}

		int a = this.planet.savedMapHeight[planet.savedMapMatrix.GetLength(0)-1];
		int b = this.planet.savedMapHeight[sizeInterpolation];

		float d = 0.0f;

		float ratioY = Mathf.Abs(b - a);
		float distX = 1.0f / sizeInterpolation;

		for(int i = 0; i < sizeInterpolation; i++){
			float height_f = interpolation_cos(a, b, d);
			height_f *= ratioY;
			height_f += Mathf.Min(a,b);
			d += distX;

			int height = (int) height_f;

			this.planet.savedMapHeight[i] = height;

			for (int j = height-1; j >= 0; j--)
				planet.savedMapMatrix [i, j] = 1;
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

		for(int i = currentPos.x - fieldOfRender.x-5; i < currentPos.x + fieldOfRender.x+5; i++){
			if(i < 0) {
				x = Mathf.Abs(i) - 1;
				int quotient = x / planet.savedMapMatrix.GetLength(0);
				x = (planet.savedMapMatrix.GetLength(0) - 1) - (x - planet.savedMapMatrix.GetLength(0) * quotient);
			} else {
				int quotient = i / planet.savedMapMatrix.GetLength(0);
				x = i - planet.savedMapMatrix.GetLength(0) * quotient;
			}
			if (i >= currentPos.x - fieldOfRender.x && i < currentPos.x + fieldOfRender.x) {
				
				for(int j = currentPos.y - fieldOfRender.y; j < currentPos.y + fieldOfRender.y; j++){
					if (j >= 0 && j < planet.savedMapHeight [x]) {
						y = (int) Mathf.Clamp((float)j, 0.0f, (float)(maximalHeight-1));
						frontGround.SetTile (new Vector3Int (i, j, 0), planet.tilesType [planet.savedMapMatrix [x, y]]);
						if (j < planet.savedMapHeight [x] - 1) 
							backGround.SetTile (new Vector3Int (i, j, 0), planet.planetType.backgroundTile);
					}

				}

				for (int j = currentPos.y - fieldOfRender.y - 15; j < currentPos.y - fieldOfRender.y; j++) {
					if (j >= 0 && j < planet.savedMapHeight [x]) {
						frontGround.SetTile (new Vector3Int (i, j, 0), null);
						frontGround.SetTile (new Vector3Int (i, j, 0), null);
						if (j < planet.savedMapHeight [x] - 1)
							backGround.SetTile (new Vector3Int (i, j, 0), null);
					}

				}
			} else {
				for (int j = currentPos.y - fieldOfRender.y - 15; j < currentPos.y + fieldOfRender.y + 15; j++) {
					if (j >= 0 && j < planet.savedMapHeight [x]) {
						frontGround.SetTile (new Vector3Int (i, j, 0), null);
						frontGround.SetTile (new Vector3Int (i, j, 0), null);
						if (j < planet.savedMapHeight [x] - 1)
							backGround.SetTile (new Vector3Int (i, j, 0), null);
					}

				}
			}
		}

	
	}

	public void clearWorld(){
		/*
		frontGround.ClearAllTiles ();
		backGround.ClearAllTiles ();
		Destroy(GameObject.FindGameObjectWithTag ("FrontGround").gameObject);
		Destroy(GameObject.FindGameObjectWithTag ("BackGround").gameObject);
		Destroy (grid.GetComponent<Grid> ());
		grid.AddComponent<Grid> ();
		grid.GetComponent<Grid> ().cellSize.x = 0.25f;
		grid.GetComponent<Grid> ().cellSize.y = 0.25f

		GameObject newFrontground = frontgroundModel;
		GameObject newBackground = backgroundModel;

		newFrontground.transform.parent.SetParent(grid.transform);
		newBackground.transform.parent.SetParent(grid.transform);

		Instantiate(newFrontground);
		Instantiate(newBackground);


		*/
	}

	private void InitVarCaves(ref Vector2 [] octaveOffsets, float seedC, int octaves, Vector2 offset){
		System.Random prng = new System.Random (Mathf.RoundToInt(seedC));

		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) + offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}
	}

	private void AddPerlinNoiseArea(float halfWidth, float halfHeight, float scale, int octaves, float persistance, float lacunarity, Vector2 [] octaveOffsets, float maxNoiseHeight, float minNoiseHeight, float bornea, float borneb,int blocID, int area){
		for (int j = 0; j < planet.savedMapMatrix.GetLength (0); j++) {
			for (int i = (planet.savedMapHeight[j] - Mathf.RoundToInt(area*(planet.savedMapHeight[j]/3)))+1; i < planet.savedMapHeight [j] - Mathf.RoundToInt ((area-1) * (planet.savedMapHeight [j] / 3)); i++) {
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

				if (val > bornea && val <= borneb && planet.savedMapMatrix [j, i]!=0)
					planet.savedMapMatrix [j, i] = blocID;

			}
		}
	}

	private void AddPerlinNoiseCave(float halfWidth, float halfHeight, float scale, int octaves, float persistance, float lacunarity, Vector2 [] octaveOffsets, float maxNoiseHeight, float minNoiseHeight, float bornea, float borneb,int blocID, int depthMin, int depthSubToMax){
		for (int j = 0; j < planet.savedMapMatrix.GetLength (0); j++) {
			for (int i = depthMin; i < planet.savedMapHeight [j] - depthSubToMax; i++) {
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

				if (val > bornea && val <= borneb && planet.savedMapMatrix [j, i]!=0)
					planet.savedMapMatrix [j, i] = blocID;

			}
		}
	}
	/*
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
	}*/
}

