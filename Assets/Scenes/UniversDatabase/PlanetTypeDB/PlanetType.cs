using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class PlanetType {

	public string type;
	public TileBase ruleTile;
	public TileBase backgroundTile;
	public TileBase chestSprite;
	public TileBase unbreakableTile;
	public List<Flora> treeList;
	public string treeAssetsPath;

	public PlanetType(string type, TileBase ruleTile, TileBase backgroundTile, TileBase chestSprite, TileBase unbreakableTile, List<Flora> treeList, string treeAssetsPath) {
		this.type = type;
		this.ruleTile = ruleTile;
		this.backgroundTile = backgroundTile;
		this.chestSprite = chestSprite;
		this.unbreakableTile = unbreakableTile;
		this.treeList = treeList;
		this.treeAssetsPath = treeAssetsPath;
	}

	public PlanetType() {
		type = "";
		ruleTile = null;
		backgroundTile = null;
		chestSprite = null;
		unbreakableTile = null;
		treeList = new List<Flora> ();
		treeAssetsPath = "";
	}

	public PlanetType clone(){
		return new PlanetType (type, ruleTile, backgroundTile, chestSprite, unbreakableTile, new List<Flora>(treeList), treeAssetsPath);
	}
}
