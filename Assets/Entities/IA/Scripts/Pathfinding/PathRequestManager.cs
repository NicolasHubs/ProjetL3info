using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

public class PathRequestManager : MonoBehaviour {

	Queue<PathResult> results = new Queue<PathResult> ();

	static PathRequestManager instance;
	PathfindingTMP pathfinding;

	void Awake() {
		instance = this;
		pathfinding = GetComponent<PathfindingTMP>();
	}

	void Update(){
		if (results.Count > 0) {
			int itemsInQueue = results.Count;
			lock (results) {
				for (int i = 0; i < itemsInQueue; i++) {
					PathResult result = results.Dequeue ();
					result.callback (result.path, result.success);
				}
			}
		}
	}

	public static void RequestPath(PathRequest request) {
		ThreadStart threadStart = delegate {
			instance.pathfinding.FindPath (request, instance.FinishedProcessingPath);
		};
		threadStart.Invoke ();
	}

	public void FinishedProcessingPath(PathResult result) {
		lock (results)
			results.Enqueue (result);
	}
}

public struct PathRequest {
	public Vector3 pathStart;
	public Vector3 pathEnd;
	public bool grounded;
	public Vector2 unitCollider;
	public Action<Vector3[], bool> callback;

	public PathRequest(Vector3 _start, Vector3 _end, bool _grounded, Vector2 _unitCollider, Action<Vector3[], bool> _callback) {
		pathStart = _start;
		pathEnd = _end;
		grounded = _grounded;
		unitCollider = _unitCollider;
		callback = _callback;
	}
}

public struct PathResult {
	public Vector3[] path;
	public bool success;
	public Action<Vector3[], bool> callback;

	public PathResult (Vector3[] path, bool success, Action<Vector3[], bool> callback){
		this.path = path;
		this.success = success;
		this.callback = callback;
	}
}