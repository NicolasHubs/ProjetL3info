using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[Serializable]
public class PlanetToJson {

	public int[] savedMapMatrix = null;
	public int[] savedMapHeight = null;
	public int[] nbOccurence = null;
	public List<TileBase> tilesType;

	public PlanetToJson(int[] savedMapMatrix, int[] savedMapHeight, int[] nbOccurence, List<TileBase> tilesType) {
		this.savedMapMatrix = savedMapMatrix;
		this.savedMapHeight = savedMapHeight;
		this.nbOccurence = nbOccurence;
		this.tilesType = tilesType;
	}

	public PlanetToJson() {
		savedMapMatrix = null;
		savedMapHeight = null;
		nbOccurence = null;
		tilesType = new List<TileBase> ();
	}

	public PlanetToJson clone(){
		return new PlanetToJson(savedMapMatrix, savedMapHeight, nbOccurence,tilesType);
	}
}
