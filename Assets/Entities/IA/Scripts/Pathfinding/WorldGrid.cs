using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class WorldGrid : MonoBehaviour {
	
	public Tilemap world;
	float nodeRadius;
	public Node[,] worldGrid;
	float nodeDiameter;
	int gridSizeX, gridSizeY;
	
	void Awake() {
		nodeRadius = world.cellSize.x /2;
		nodeDiameter = nodeRadius * 2;
		gridSizeX = world.size.x;
		gridSizeY = world.size.y;
		CreateGrid();
	}

	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

	void CreateGrid() {
		worldGrid = new Node[gridSizeX,gridSizeY];
		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector3 worldPoint = new Vector3 (x * nodeDiameter, y * nodeDiameter, 0.0f);
				//Debug.DrawRay (worldPoint, new Vector3(0,0.005f,0), Color.green, 50f);
				bool walkable = world.GetTile(new Vector3Int(x,y,0)) == null ? true : false;
				worldGrid[x,y] = new Node(walkable,worldPoint,x,y);
			}
		}
	}

	// Peut être optimisé
	public List<Node> GetNeighbours(Node node, Vector2 unitCollider) {
		List<Node> neighbours = new List<Node>();

		bool biggerThanOneCellX = (unitCollider.x >= world.cellSize.x)? true : false;
		bool biggerThanOneCellY = (unitCollider.y >= world.cellSize.y)? true : false;

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkY >= 0 && checkY < gridSizeY && checkX >= 0 && checkX < gridSizeX) {
					if (biggerThanOneCellX && x == 0 && !((checkX >= 1)? worldGrid [checkX - 1, checkY].walkable : false) && !((checkX < gridSizeX - 1)? worldGrid [checkX + 1, checkY].walkable : false))
						continue;

					if (biggerThanOneCellY && y == 0 && !((checkY >= 1)? worldGrid [checkX, checkY - 1].walkable : false) && !((checkY < gridSizeY - 1)? worldGrid [checkX, checkY + 1].walkable : false))
						continue;
					
					if (x == 1 && y == 1 || x == 1 && y == -1 || x == -1 && y == 1 || x == -1 && y == -1) {
						if (!worldGrid [checkX, checkY + (y*-1)].walkable && !worldGrid [checkX + (x*-1), checkY].walkable)
							continue;
					}

					neighbours.Add(worldGrid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}
	
	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		int x = Mathf.FloorToInt((worldPosition.x) / nodeDiameter);
		int y = Mathf.FloorToInt((worldPosition.y) / nodeDiameter);

		return worldGrid[x,y];
	}
}