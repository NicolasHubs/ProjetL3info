using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

[RequireComponent (typeof (Rigidbody2D))]
public class FlyingAnimalController : MonoBehaviour {

	// SpriteRenderer of the enemy
	private SpriteRenderer spriteRenderer;

	// What to chase?
	private Transform target;

	// How many times each second we will update our path
	public float updateRate = 2f;

	// Caching
	private Rigidbody2D rb;

	// The AI's speed per second
	public float speed = 300f;
	public ForceMode2D fMode;

	private bool grounded;

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer> (); 
		rb = GetComponent<Rigidbody2D> ();

		target = GameObject.FindGameObjectWithTag ("Player").transform;

		if (target == null) {
			Debug.LogError ("No player found ?");
			return;
		}
	}

	void FixedUpdate () {
		// Direction to the next waypoint
		Vector3 dir = (target.position - transform.position).normalized * speed * Time.fixedDeltaTime;

		// Always look in the good direction
		if (dir.x > 0.01f) {
			if (spriteRenderer.flipX == false) 
				spriteRenderer.flipX = true;
		} else if (dir.x < -0.01f) {
			if (spriteRenderer.flipX == true) 
				spriteRenderer.flipX = false;
		}

		// Move the AI
		rb.AddForce (dir, fMode);
	}
}
