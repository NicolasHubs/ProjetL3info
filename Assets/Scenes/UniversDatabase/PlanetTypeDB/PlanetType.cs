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
	/*
	public List <string> weatherList;
	public bool [] isOnPlanet;
	*/
	public PlanetType(string type, TileBase ruleTile, TileBase backgroundTile, TileBase chestSprite, TileBase unbreakableTile) {
		this.type = type;
		this.ruleTile = ruleTile;
		this.backgroundTile = backgroundTile;
		this.chestSprite = chestSprite;
		this.unbreakableTile = unbreakableTile;
	}

	public PlanetType() {
		type = "";
		ruleTile = null;
		backgroundTile = null;
		chestSprite = null;
		unbreakableTile = null;
	}

	public PlanetType clone(){
		return new PlanetType (type, ruleTile, backgroundTile, chestSprite, unbreakableTile);
	}
}
