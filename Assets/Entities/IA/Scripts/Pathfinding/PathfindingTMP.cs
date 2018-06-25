using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathfindingTMP : MonoBehaviour {

	WorldGrid worldGrid;

	void Awake() {
		worldGrid = GetComponent<WorldGrid> ();
	}

	public void FindPath(PathRequest request, Action<PathResult> callback) {

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		Node startNode = worldGrid.NodeFromWorldPoint (request.pathStart);
		Node targetNode = worldGrid.NodeFromWorldPoint (request.pathEnd);
		startNode.parent = startNode;
		int dist = GetDistance (startNode, targetNode);

		if (startNode.walkable && targetNode.walkable &&  dist > 40) {
			Heap<Node> openSet = new Heap<Node> (worldGrid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node> ();
			openSet.Add (startNode);
			while (openSet.Count > 0 && closedSet.Count < 350) {
				Node currentNode = openSet.RemoveFirst ();
				closedSet.Add (currentNode);
				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}

				foreach (Node neighbour in worldGrid.GetNeighbours(currentNode, request.unitCollider)) {
					if (!neighbour.walkable || closedSet.Contains (neighbour)) {
						continue;
					}

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance (neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains (neighbour))
							openSet.Add (neighbour);
						else
							openSet.UpdateItem (neighbour);
					}
				}
			}
		}

		if (pathSuccess) {
			waypoints = RetracePath (startNode, targetNode, request.unitCollider);
			pathSuccess = waypoints.Length > 0;
		}
		
		callback(new PathResult(waypoints, pathSuccess, request.callback));
	}

	Vector3[] RetracePath(Node startNode, Node endNode, Vector2 sizeOfUnit) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector3 [] waypoints = SimplifyPath (path);

		/*List<Vector3> waypoints = new List<Vector3> ();
		for (int i = 1; i < path.Count; i++) {
			waypoints.Add(path[i].worldPosition);
		}

		Vector3[] way = waypoints.ToArray ();*/
		Array.Reverse (waypoints);

		return waypoints;
	}

	Vector3[] SimplifyPath (List<Node> path){
		List<Vector3> waypoints = new List<Vector3> ();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++) {
			Vector2 directionNew = new Vector2 (path [i - 1].gridX - path [i].gridX, path [i - 1].gridY - path [i].gridY);
			if (directionNew != directionOld) {
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray ();
	}

	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14 * dstY + 10 * (dstX - dstY);
		return 14 * dstX + 10 * (dstY - dstX);
	}
}
