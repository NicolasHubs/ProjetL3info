using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Flora {
	
	public string name;
	public string treeType;
	public int sizeX;
	public int sizeY;
	public TileBase tree;
	public string treePath;

	public Flora(string name, string treeType, int sizeX, int sizeY, TileBase tree, string treePath) {
		this.name = name;
		this.treeType = treeType;
		this.sizeX = sizeX;
		this.sizeY = sizeY;
		this.tree = tree;
		this.treePath = treePath;
	}

	public Flora() {
		name = "";
		treeType = "";
		sizeX = 0;
		sizeY = 0;
		tree = null;
		treePath = "";
	}

	public Flora clone(){
		return new Flora (name, treeType, sizeX, sizeY, tree, treePath);
	}
}
