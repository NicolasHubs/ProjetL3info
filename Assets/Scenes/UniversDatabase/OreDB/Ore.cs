using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[System.Serializable]
public class Ore {

	public string name;
	public TileBase tile;
	public Vector2Int galaxyRange;
	public int area;
	public float depositRarity;
	public float depositWidth;
	public float seedDeposit;

	public Ore(string name, TileBase tile, Vector2Int galaxyRange, int area, float depositRarity, float depositWidth, float seedDeposit) {
		this.name = name;
		this.tile = tile;
		this.galaxyRange = galaxyRange;
		this.area = area;
		this.depositRarity = depositRarity;
		this.depositWidth = depositWidth;
		this.seedDeposit = seedDeposit;
	}

	public Ore() {
		name = "";
		tile = null;
		galaxyRange = new Vector2Int(0,0);
		area = 0;
		depositRarity = 0.0f;
		depositWidth = 0.0f;
		seedDeposit = 0.0f;
	}

	public Ore clone(){
		return new Ore ((string)name.Clone(), tile, galaxyRange, area, depositRarity, depositWidth,seedDeposit);
	}
}
