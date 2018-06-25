using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent (typeof (Rigidbody2D))]
public class WalkingAnimalController : PhysicsObject {
	
	public float speed;
	public float jumpTakeOffSpeed = 7;
	private Transform player;
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private Vector2 unitCollider;
	private float startingPosRaycastY;
	private Tilemap world;

	// Initialisation
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> (); 
		animator = GetComponent<Animator> ();
		world = GameObject.FindGameObjectWithTag ("FrontGround").GetComponent<Tilemap>();
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		Vector3 colliderSizeRes = Collider2DSize.GetColliderSize (GetComponents<Collider2D> ());

		unitCollider.x = colliderSizeRes.x;
		unitCollider.y = colliderSizeRes.y;
		startingPosRaycastY = colliderSizeRes.z;
	}

	protected override void ComputeVelocity() {
		Vector3 currentPosition = transform.position;
		Vector2 move = Vector2.zero;
		Vector2 a = new Vector2 (player.position.x, 0.0f);
		Vector2 b = new Vector2 (currentPosition.x, 0.0f);
		float dist = Vector2.Distance (a, b);
	
		if (dist > 0.1f) {
			if (currentPosition.x < player.position.x)
				move = Vector2.right;
			else if (currentPosition.x > player.position.x)
				move = Vector2.left;
			
			Vector2 startingPos = new Vector2 (currentPosition.x, currentPosition.y + startingPosRaycastY);

			if (move.x > 0.01f) {
				if (spriteRenderer.flipX == false) 
					spriteRenderer.flipX = true;
				startingPos.x = startingPos.x + (unitCollider.x / 2) + (world.cellSize.x/2);
			} else if (move.x < -0.01f) {
				if (spriteRenderer.flipX == true) 
					spriteRenderer.flipX = false;
				startingPos.x = startingPos.x - (unitCollider.x / 2) - (world.cellSize.x/2);
			}

			Vector2 startingPos2 = new Vector2 (startingPos.x, currentPosition.y + startingPosRaycastY - unitCollider.y);
			Vector2 startingPos3 = new Vector2 (currentPosition.x + (unitCollider.x/2)*move.x, currentPosition.y + startingPosRaycastY - unitCollider.y/2);

			RaycastHit2D hit2dFromTop = Physics2D.Raycast (startingPos, Vector2.down, unitCollider.y);
			RaycastHit2D hit2dFromBot = Physics2D.Raycast (startingPos2, Vector2.up, unitCollider.y);
			RaycastHit2D hit2dfromMid = Physics2D.Raycast (startingPos3, move, world.cellSize.x/2);

			//Debug.DrawRay (startingPos, Vector2.down*unitCollider.y, Color.green);
			//Debug.DrawRay (startingPos2, Vector2.up*unitCollider.y, Color.red);
			//Debug.DrawRay (startingPos3, move*world.cellSize.x, Color.blue);

			if (grounded && (hit2dFromTop.collider != null || hit2dFromBot.collider != null || hit2dfromMid.collider != null)) {
				velocity.y = jumpTakeOffSpeed;
			}

		}
		animator.SetBool ("grounded", grounded);
		animator.SetFloat ("velocityX", Mathf.Abs (velocity.x) / speed);

		targetVelocity = move * speed;
	}
}
