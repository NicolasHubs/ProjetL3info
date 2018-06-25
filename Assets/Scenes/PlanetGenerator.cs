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

	[HideInInspector]
	public Planet planet;

	[HideInInspector]
	public Tile.ColliderType savedCollider;

	//Cette variable il faudra l'enlever quand on aura fait la bdd de tous les blocs du jeu, elle permet de savoir l'id du bloc incassable
	[HideInInspector]
	public int unbreakableTileID;

	private GameObject mainCharacter;
	//private Camera mainCamera;

	private Vector3Int currentPos;

	private int sizeInterpolation;

	//private GameObject grid;	
	[HideInInspector]
	public Tilemap frontGround;
	[HideInInspector]
	public Tilemap backGround;
	[HideInInspector]
	public Tilemap flora;
	
	private Vector3Int cellCamPosition;

	private int nbTotalTile;

	[HideInInspector]
	public Vector2Int fieldOfRender;

	[HideInInspector]
	public int maximalHeight;

	[HideInInspector]
	public int additionalSight;

	// ---------- TILES LIST ------------ //
	
	void Awake() {
		planet = Scenes.getSceneParameter ();
	}

	void Start(){
		GenerateWorld ();
	}

	public void GenerateWorld (){
		// Initialisation des variables et de la grid
		Initialisation ();

		if (planet.savedMapHeight.GetLength (0) == 0 || planet.savedMapMatrix == null) {
			planet.savedMapHeight = new int[planet.horizontalSize + sizeInterpolation];
			planet.savedMapMatrix = new int[planet.horizontalSize + sizeInterpolation, maximalHeight];

			planet.playerLastPosition = new Vector3 (0,0,0);

			// Generation des vecteurs ainsi que de leur type de tile
			GenerateTileSettings ();

			planet.tilesType = new List<TileBase> ();
			planet.tilesType.Insert (0, (TileBase)null);
			planet.tilesType.Add (planet.planetType.ruleTile);
			planet.tilesType.Add (planet.planetType.unbreakableTile);

			unbreakableTileID = 2;

			// Ajout des tiles indestructibles
			for (int i = 0; i < planet.savedMapMatrix.GetLength (0); i++)
				planet.savedMapMatrix [i, 0] = unbreakableTileID;

			Vector2[] octaveOffsets = new Vector2[3];

			InitVarCaves (ref octaveOffsets, planet.seedCave, 3, new Vector2 (5f, 5f));

			float maxNoiseHeight = float.MinValue;
			float minNoiseHeight = float.MaxValue;

			float halfWidth = 400 / 2f;
			float halfHeight = 100 / 2f;

			// Ajout des grottes
			AddPerlinNoiseCave (halfWidth, halfHeight, 20f, 3, 0.289f, 3f, octaveOffsets, maxNoiseHeight, minNoiseHeight, 0.0f, 0.30f, 0, planet.verticalSize / 2, Mathf.RoundToInt (planet.verticalSize * 0.05f));
			AddPerlinNoiseCave (halfWidth, halfHeight, planet.caveWidth, 3, 0.439f, 3f, octaveOffsets, maxNoiseHeight, minNoiseHeight, 0.23f, planet.caveQuantity, 0, 1, planet.verticalSize / 2);

			// Ajout des minerais
			foreach (Ore ore in planet.oreList) {
				octaveOffsets = new Vector2[3];
				InitVarCaves (ref octaveOffsets, ore.seedDeposit, 3, new Vector2 (5f, 5f));
				planet.tilesType.Add (ore.tile);
				AddPerlinNoiseArea (halfWidth, halfHeight, ore.depositWidth, 3, 0.0f, 3f, octaveOffsets, maxNoiseHeight, minNoiseHeight, 0.0f, ore.depositRarity / 10f, planet.tilesType.Count - 1, ore.area);
			}

			planet.tilesType.Add (planet.planetType.chestSprite);

			int nbTourDeBoucle = 0;
			int startingPos;
			bool chestPosFounded;
			bool restart;

			// Ajout des coffres
			int numChest = 0;
			while (numChest < planet.numberOfChest) {
				startingPos = Mathf.RoundToInt (UnityEngine.Random.Range (0, planet.savedMapMatrix.GetLength (0) - 1));

				chestPosFounded = false;
				restart = false;
				for (int i = startingPos; i < planet.savedMapMatrix.GetLength (0) && !chestPosFounded && !restart; i++) {
					for (int j = 0; j < planet.savedMapHeight [i] && !chestPosFounded && !restart; j++) {
						if (planet.savedMapMatrix [i, j] == planet.tilesType.Count - 1) {
							restart = true;
						} else if (planet.savedMapMatrix [i, j] == 0) {
							planet.savedMapMatrix [i, j] = planet.tilesType.Count - 1;
							chestPosFounded = true;
							//print ("Chest placed !, [x,y] = ["+i+","+j+"]");
						}
					}
				}
				nbTourDeBoucle++;
				if (!restart)
					numChest++;
			}

			// Ajout de la végétation
			List <int> allDifferentTreeSize = new List<int>();

			// Ajout à tilesType et sauvegarde des différentes tailles d'arbres
			for (int i = 0; i < planet.planetType.treeList.Count; i++) {
				Flora tree = planet.planetType.treeList [i];
				planet.tilesType.Add (tree.tree);
				if (!allDifferentTreeSize.Contains (tree.sizeX)) {
					AddSorted(allDifferentTreeSize,tree.sizeX);
				}
			}
			if (allDifferentTreeSize.Count > 0) {
					
				int largestTreeSize = allDifferentTreeSize[allDifferentTreeSize.Count-1];

				// Calculer toutes les positions possibles
				for (int i = 0; i < planet.savedMapMatrix.GetLength (0) - largestTreeSize; i++) {
					bool hasSpace = true;

					int randomTreeSize = UnityEngine.Random.Range (0, allDifferentTreeSize.Count);

					for (int k = 0; k < allDifferentTreeSize[randomTreeSize]; k++) {
						if (planet.savedMapMatrix [i + k, planet.savedMapHeight [i]] != 0 || planet.savedMapMatrix [i + k, planet.savedMapHeight [i]-1] == 0) {
							hasSpace = false;
							break;
						}
					}

					if (hasSpace) {
						int randomStartNumber = UnityEngine.Random.Range(0, planet.planetType.treeList.Count);
						int valueInArray = randomStartNumber;
						for (int j = 0; j < planet.planetType.treeList.Count; j++) {
							valueInArray = mod(j+valueInArray,planet.planetType.treeList.Count);
							if (planet.planetType.treeList [valueInArray].sizeX == allDifferentTreeSize [randomTreeSize]) {
								break;
							}
						}
						planet.savedMapMatrix [i, planet.savedMapHeight [i]] = 100 + planet.tilesType.Count - planet.planetType.treeList.Count + valueInArray;
						i += allDifferentTreeSize[randomTreeSize];
					}
				}
			
			}

			float xPos = (planet.savedMapMatrix.GetLength (0) * frontGround.cellSize.x) / 2;
			int numeroXdanslaliste = frontGround.WorldToCell (new Vector3 (xPos, 0f, 0f)).x;
			int numeroYdanslaliste = planet.savedMapHeight [numeroXdanslaliste];
			float yPos = numeroYdanslaliste * frontGround.cellSize.y;
			mainCharacter.transform.position = new Vector3 (xPos, yPos + 0.9f, 0f);

		} else {
			mainCharacter.transform.position = new Vector3(planet.playerLastPosition.x, planet.playerLastPosition.y + 0.9f, planet.playerLastPosition.z);
		}

		/*
		for (int i = 0; i < planet.savedMapMatrix.GetLength (0); i++)
			for (int j = planet.savedMapHeight [i] - 1; j >= planet.savedMapHeight [i] - fieldOfRender.y - 5; j--) {
				frontGround.SetTile (new Vector3Int (i, j, 0), planet.tilesType [planet.savedMapMatrix [i, j]]);
				frontGround.SetColliderType (new Vector3Int (i, j, 0), ColliderCheckNeighboor(i,j));
				if (j < planet.savedMapHeight [i] - 1)
					backGround.SetTile (new Vector3Int (i, j, 0), planet.planetType.backgroundTile);
			}
		*/
		currentPos = frontGround.WorldToCell(mainCharacter.transform.position);
	}

	int mod(int x, int y) {
		return (x % y + y) % y;
	}

	void AddSorted(List<int> array, int item)
	{
		if (array.Count == 0)
		{
			array.Add(item);
			return;
		}
		if (array[array.Count-1].CompareTo(item) <= 0)
		{
			array.Add(item);
			return;
		}
		if (array[0].CompareTo(item) >= 0)
		{
			array.Insert(0, item);
			return;
		}
		int index = array.BinarySearch(item);
		if (index < 0) 
			index = ~index; // Bitwise complement operator (basically invert bits)
		array.Insert(index, item);
	}


	private void Initialisation(){
		sizeInterpolation = Mathf.RoundToInt(planet.horizontalSize*0.05f);

		frontGround = GameObject.FindGameObjectWithTag("FrontGround").gameObject.GetComponent<Tilemap>();
		backGround = GameObject.FindGameObjectWithTag("BackGround").gameObject.GetComponent<Tilemap>();
		flora = GameObject.FindGameObjectWithTag("Flora").gameObject.GetComponent<Tilemap>();
		
		mainCharacter = GameObject.FindGameObjectsWithTag("Player")[0];

		currentPos = frontGround.WorldToCell(mainCharacter.transform.position);

		maximalHeight = planet.verticalSize + planet.heightMultiplier*2;

		GameObject.FindGameObjectWithTag ("BackgroundHandler").GetComponent<CycleJourNuit> ().speed = planet.daySpeedMultiplier;

		float cameraHeight = 2 * Camera.main.orthographicSize;
		float cameraWidth = cameraHeight * Camera.main.aspect;

		cameraHeight /= frontGround.cellSize.y;
		cameraWidth /= frontGround.cellSize.x;
		int worldCameraheight = Mathf.CeilToInt((cameraHeight+2)/2);
		int worldCameraWidth = Mathf.CeilToInt((cameraWidth+2)/2);
		fieldOfRender = new Vector2Int (worldCameraWidth+8, worldCameraheight); 
		additionalSight = 3;
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
		planet.playerLastPosition = mainCharacter.transform.position;
		currentPos = frontGround.WorldToCell(planet.playerLastPosition);

		int x = 0;
		int y = 0;

		for(int i = currentPos.x - fieldOfRender.x - additionalSight; i < currentPos.x + fieldOfRender.x + additionalSight; i++){
			if(i < 0) {
				x = Mathf.Abs(i) - 1;
				int quotient = x / planet.savedMapMatrix.GetLength(0);
				x = (planet.savedMapMatrix.GetLength(0) - 1) - (x - planet.savedMapMatrix.GetLength(0) * quotient);
			} else {
				int quotient = i / planet.savedMapMatrix.GetLength(0);
				x = i - planet.savedMapMatrix.GetLength(0) * quotient;
			}

			if (i >= currentPos.x - fieldOfRender.x && i <= currentPos.x + fieldOfRender.x) {
				for(int j = currentPos.y - fieldOfRender.y; j <= currentPos.y + fieldOfRender.y; j++){
					if (j >= 0 && j <= planet.savedMapHeight [x]) {
						y = (int) Mathf.Clamp((float)j, 0.0f, (float)(maximalHeight-1));
						if(planet.savedMapMatrix[x, y] >= 100){
							flora.SetTile (new Vector3Int (i, j, 0), planet.tilesType [planet.savedMapMatrix [x, y]-100]);
						} else {
							frontGround.SetColliderType (new Vector3Int (i, j, 0), Tile.ColliderType.Grid);
							frontGround.SetTile (new Vector3Int (i, j, 0), planet.tilesType [planet.savedMapMatrix [x, y]]);
							if (j < planet.savedMapHeight [x] - 1) 
								backGround.SetTile (new Vector3Int (i, j, 0), planet.planetType.backgroundTile);
						}
					}
				}

				for (int j = currentPos.y - fieldOfRender.y - additionalSight; j < currentPos.y - fieldOfRender.y; j++) 
					if (j >= 0 && j < planet.savedMapHeight [x]) 
						frontGround.SetColliderType (new Vector3Int (i, j, 0), Tile.ColliderType.None);

				if (currentPos.y + fieldOfRender.y < planet.savedMapHeight [x]) 
					for (int j = currentPos.y + fieldOfRender.y; j < Mathf.Min(currentPos.y + fieldOfRender.y + additionalSight, planet.savedMapHeight [x]); j++) 
						if (j >= 0) 
							frontGround.SetColliderType (new Vector3Int (i, j, 0), Tile.ColliderType.None);

			} else {
				for (int j = currentPos.y - fieldOfRender.y - additionalSight; j < currentPos.y + fieldOfRender.y + additionalSight; j++) 
					if (j >= 0 && j < planet.savedMapHeight [x]) 
						frontGround.SetColliderType (new Vector3Int (i, j, 0), Tile.ColliderType.None);
			}
		}
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

