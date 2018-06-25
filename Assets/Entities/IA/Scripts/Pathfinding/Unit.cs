using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class Unit : PhysicsAIpathfinding {

	public float maxSpeed = 2;
	public float jumpTakeOffSpeed = 7;
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	public Tilemap backGroundWorld;
	public TileBase tileNextWaypoint;
	private Transform target;
	private Vector2 unitCollider;
	private Vector2 previousMove;
	private Vector3[] path;
	private WorldGrid worldGrid;
	private Vector2 move;
	private float startingPosRaycastY; // StartingYpos for the raycast of the unit in FollowPath function

	void Awake () {
		worldGrid = GameObject.FindGameObjectWithTag("grid").GetComponent<WorldGrid> ();
		spriteRenderer = GetComponent<SpriteRenderer> (); 
		animator = GetComponent<Animator> ();
		unitCollider = Vector2.zero;
		target = GameObject.FindGameObjectWithTag ("Player").transform;

		Vector3 colliderSizeRes = Collider2DSize.GetColliderSize (GetComponents<Collider2D> ());

		unitCollider.x = colliderSizeRes.x;
		unitCollider.y = colliderSizeRes.y;
		startingPosRaycastY = colliderSizeRes.z;

		previousMove = Vector2.zero;
		move = Vector2.zero;
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
		if (pathSuccessful) {
			path = newPath;
			RenderPath (newPath);
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
		animator.SetBool ("grounded", grounded);
		animator.SetFloat ("velocityX", Mathf.Abs (velocity.x) / maxSpeed);
		targetVelocity = move * maxSpeed;
		move = Vector2.zero;
	}

	void RenderPath(Vector3[] path){
		backGroundWorld.ClearAllTiles ();
		for (int i = 0; i < path.GetLength (0); i++) {
			Node frog = worldGrid.NodeFromWorldPoint (path[i]);
			//Node penny = worldGrid.NodeFromWorldPoint (target.position);
			backGroundWorld.SetTile (new Vector3Int (frog.gridX, frog.gridY,0), tileNextWaypoint);
		}
	}

	IEnumerator FollowPath() {
		Vector3 nextWaypoint = path[0];
		Vector3 currentPosition = transform.position;
		//Debug.DrawRay (currentPosition, new Vector3(0,0.005f,0), Color.red);
		//Debug.DrawRay (target.position, new Vector3(0,0.005f,0), Color.red);
		Node nextNode = worldGrid.NodeFromWorldPoint(nextWaypoint);
		Node currentNode = worldGrid.NodeFromWorldPoint(currentPosition);

		if (nextNode.gridX != currentNode.gridX || nextWaypoint.y <= currentPosition.y) {
		if (currentPosition.x < (nextWaypoint.x + ((previousMove == Vector2.right) ? worldGrid.world.cellSize.x : 0)))
			move = Vector2.right;
		else if (currentPosition.x > nextWaypoint.x)
			move = Vector2.left;
		}
		previousMove = move;

		Vector2 startingPos = new Vector2 (currentPosition.x, currentPosition.y + startingPosRaycastY);

		if (move.x > 0.01f) {
			if (spriteRenderer.flipX == false) 
				spriteRenderer.flipX = true;
			startingPos.x = startingPos.x + (unitCollider.x / 2) + (worldGrid.world.cellSize.x/2);
		} else if (move.x < -0.01f) {
			if (spriteRenderer.flipX == true) 
				spriteRenderer.flipX = false;
			startingPos.x = startingPos.x - (unitCollider.x / 2) - (worldGrid.world.cellSize.x/2);
		}
		Vector2 startingPos2 = new Vector2 (startingPos.x, currentPosition.y + startingPosRaycastY - unitCollider.y);
		Vector2 startingPos3 = new Vector2 (currentPosition.x + (unitCollider.x/2)*move.x, currentPosition.y + startingPosRaycastY - unitCollider.y/2);

		RaycastHit2D hit2dFromTop = Physics2D.Raycast (startingPos, Vector2.down, unitCollider.y);
		RaycastHit2D hit2dFromBot = Physics2D.Raycast (startingPos2, Vector2.up, unitCollider.y);
		RaycastHit2D hit2dfromMid = Physics2D.Raycast (startingPos3, move, worldGrid.world.cellSize.x/2);

		//Debug.DrawRay (startingPos, Vector2.down*unitCollider.y, Color.green);

		if (grounded && (hit2dFromTop.collider != null || hit2dFromBot.collider != null || hit2dfromMid.collider != null || (nextWaypoint.y > currentPosition.y && worldGrid.NodeFromWorldPoint (new Vector2 (currentPosition.x + (worldGrid.world.cellSize.x) * move.x, currentPosition.y - worldGrid.world.cellSize.x)).walkable))) {
			velocity.y = jumpTakeOffSpeed;
		}

		yield return null;

	}

	public void Update() {
		PathRequestManager.RequestPath(new PathRequest(transform.position,target.position, grounded,unitCollider, OnPathFound));
	}
}
