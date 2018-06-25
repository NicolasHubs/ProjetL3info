using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[Serializable]
public class Planet {

	public string name;
	public float seed;
	public int galaxy;
	public int stellarSystem;
	public int planetID;
	public PlanetType planetType;
	public int horizontalSize;
	public int verticalSize;
	public float smoothness;
	public int heightMultiplier;
	public float seedCave;
	public float caveWidth;
	public float caveQuantity;
	public float gravity;
	public float daySpeedMultiplier;
	public string atmosphere;
	public Color filter;
	public int numberOfChest;
	public List<Ore> oreList;
	public int[,] savedMapMatrix = null;
	public int[] savedMapHeight = null;
	public List<TileBase> tilesType;
	public Vector3 playerLastPosition;

	public Planet(string name, float seed, int galaxy, int stellarSystem, int planetID, PlanetType planetType, int horizontalSize, int verticalSize, float smoothness, int heightMultiplier, float seedCave, float caveWidth, float caveQuantity, float gravity, float daySpeedMultiplier, string atmosphere, Color filter, int numberOfChest, List<Ore> oreList, int[,] savedMapMatrix, int[] savedMapHeight, List<TileBase> tilesType, Vector3 playerLastPosition) {
		this.name = name;
		this.seed = seed;
		this.galaxy = galaxy;
		this.stellarSystem = stellarSystem;
		this.planetID = planetID;
		this.planetType = planetType;
		this.horizontalSize = horizontalSize;
		this.verticalSize = verticalSize;
		this.smoothness = smoothness;
		this.heightMultiplier = heightMultiplier;
		this.seedCave = seedCave;
		this.caveWidth = caveWidth;
		this.caveQuantity = caveQuantity;
		this.gravity = gravity;
		this.daySpeedMultiplier = daySpeedMultiplier;
		this.atmosphere = atmosphere;
		this.filter = filter;
		this.numberOfChest = numberOfChest;
		this.oreList = oreList;
		this.savedMapMatrix = savedMapMatrix;
		this.savedMapHeight = savedMapHeight;
		this.tilesType = tilesType;
		this.playerLastPosition = playerLastPosition;
	}

	public Planet() {
		name = "";
		seed = 0.0f;
		galaxy = 0;
		stellarSystem = 0;
		planetID = 0;
		planetType = new PlanetType();
		horizontalSize = 0;
		verticalSize = 0;
		smoothness = 0.0f;
		heightMultiplier = 0;
		seedCave = 0.0f;
		caveWidth = 0.0f;
		caveQuantity = 0.0f;
		gravity = 9.81f;
		daySpeedMultiplier = 1;
		atmosphere = "";
		filter = new Color();
		numberOfChest = 0;
		oreList = new List<Ore> ();
		savedMapMatrix = null;
		savedMapHeight = null;
		tilesType = new List<TileBase> ();
		playerLastPosition = new Vector3 (0,0,0);
	}

	public PlanetToJson formatPlanetToJson(){
		// Création de mapEncrypted 1D
		int tailleMatrix = 0;

		for (int i = 0; i < savedMapMatrix.GetLength (0); i++) 
			tailleMatrix += (savedMapHeight [i] - 1);

		int[] mapToEncrypt = new int[tailleMatrix];

		// Déconstruction du tableau complet à deux dimensions en un tableau a 1 Dimension
		int depth = 0;
		for (int j = 1; j < savedMapHeight [0]; j++)
			mapToEncrypt [j-1] = savedMapMatrix [0, j];
		
		for (int i = 1; i < savedMapMatrix.GetLength (0); i++) {
			depth += savedMapHeight [i-1] - 1;
			for (int j = 1; j < savedMapHeight [i]; j++) 
				mapToEncrypt [j - 1 + depth] = savedMapMatrix [i, j];
		}
		/*
		int[] compte = new int [9];
		for (int i = 0; i < 9; i++) {
			compte [i] = 0;
		}*/

		// Encryptage de mapToEncrypt et creation du tableau du nombre d'occurence
		List<int> nbOccurence = new List<int> ();
		List<int> mapEncrypted = new List<int> ();
		int nbOcc = 1;
		for (int i = 1; i < mapToEncrypt.GetLength (0); i++) {
			if (mapToEncrypt [i - 1] != mapToEncrypt [i]) {
				/*
				switch (mapToEncrypt [i - 1]) {
				case 0:
					compte [0]+=nbOcc;
					break;
				case 1:
					compte [1]+=nbOcc;
					break;
				case 2:
					compte [2]+=nbOcc;
					break;
				case 3:
					compte [3]+=nbOcc;
					break;
				case 4:
					compte [4]+=nbOcc;
					break;
				case 5:
					compte [5]+=nbOcc;
					break;
				case 6:
					compte [6]+=nbOcc;
					break;
				case 7:
					compte [7]+=nbOcc;
					break;
				case 8:
					compte [8]+=nbOcc;
					break;
				}*/
					
				mapEncrypted.Add (mapToEncrypt [i - 1]);
				nbOccurence.Add (nbOcc);
				nbOcc = 0;
			}
			nbOcc++;
		}
		/*
		for (int i = 0; i < 9; i++) {
			Debug.Log(compte [i]);
		}*/
			
		mapEncrypted.Add (mapToEncrypt [mapToEncrypt.GetLength (0) - 1]);
		nbOccurence.Add (nbOcc);

		return new PlanetToJson (mapEncrypted.ToArray(), savedMapHeight,nbOccurence.ToArray(), playerLastPosition, tilesType);
	}

	public void formatPlanetFromJson(PlanetToJson planetFromJson){
		if (savedMapHeight.GetLength (0) == 0 || savedMapMatrix == null) {
			int sizeInterpolation = Mathf.RoundToInt (horizontalSize * 0.05f);
			savedMapHeight = new int[horizontalSize + sizeInterpolation];
			savedMapMatrix = new int[horizontalSize + sizeInterpolation, (verticalSize + heightMultiplier * 2)];
		}

		// Création de mapDecrypted 1D
		int tailleMatrix = 0;

		for (int i = 0; i < savedMapMatrix.GetLength (0); i++)
			tailleMatrix += (planetFromJson.savedMapHeight [i] - 1);

		int[] mapDecrypted = new int[tailleMatrix];

		// Décryptage de savedMapMatrix avec le tableau du nombre d'occurence
		List<int> mapToDecrypt = new List<int> ();

		for (int i = 0; i < planetFromJson.savedMapMatrix.GetLength (0); i++)
			for (int j = 0; j < planetFromJson.nbOccurence [i]; j++) 
				mapToDecrypt.Add (planetFromJson.savedMapMatrix [i]);
			
		// Affectation du tableau décrypté à mapMatrix
		mapDecrypted = mapToDecrypt.ToArray ();

		// Reconstruction du tableau complet à deux dimensions
		int depth = 0;
		for (int j = 0; j < savedMapHeight [0]; j++) {
			if (j == 0) {
				savedMapMatrix [0, j] = 2;
			} else if (j < planetFromJson.savedMapHeight[0]){
				savedMapMatrix [0, j] = mapDecrypted [j];
			} else {
				savedMapMatrix [0, j] = 0;
			}
		}
			
		for (int i = 1; i < savedMapMatrix.GetLength (0); i++){
			depth += planetFromJson.savedMapHeight [i-1] - 1;
			for (int j = 0; j < savedMapMatrix.GetLength (1); j++) {
				if (j == 0) {
					savedMapMatrix [i, j] = 2;
				} else if (j < planetFromJson.savedMapHeight[i]){
					savedMapMatrix [i, j] = mapDecrypted [j + (depth-1)];
				} else {
					savedMapMatrix [i, j] = 0;
				}
			}
		}

		savedMapHeight = planetFromJson.savedMapHeight;
		tilesType = planetFromJson.tilesType;
		playerLastPosition = planetFromJson.playerLastPosition;
	}

	public Planet clone(){
		return new Planet((string)name.Clone(), seed, galaxy, stellarSystem, planetID, planetType.clone(), horizontalSize, verticalSize, smoothness, heightMultiplier,seedCave, caveWidth, caveQuantity, gravity, daySpeedMultiplier, (string)atmosphere.Clone(), filter,numberOfChest, new List<Ore>(oreList), savedMapMatrix, savedMapHeight,tilesType,playerLastPosition);
	}
}
