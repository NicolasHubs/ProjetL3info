using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPlatformerController : PhysicsObject {

    public float maxSpeed;
    public float jumpTakeOffSpeed = 7;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Use this for initialization
    void Awake () 
    {
        spriteRenderer = GetComponent<SpriteRenderer> (); 
        animator = GetComponent<Animator> ();
    }

    protected override void ComputeVelocity()
	{			
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis ("Horizontal");

        if (Input.GetButtonDown ("Jump") && grounded) {
            velocity.y = jumpTakeOffSpeed;
        } else if (Input.GetButtonUp ("Jump")) {
            if (velocity.y > 0) {
                velocity.y = velocity.y * 0.5f;
            }
        }

        if(move.x > 0.01f) {
            if(spriteRenderer.flipX == true) {
                spriteRenderer.flipX = false;
            }
        } else if (move.x < -0.01f) {
            if(spriteRenderer.flipX == false) {
                spriteRenderer.flipX = true;
            }
        }

        animator.SetBool ("grounded", grounded);
        animator.SetFloat ("velocityX", Mathf.Abs (velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;

		if (!("Hub".Equals(SceneManager.GetActiveScene().name)) && (Input.GetButton("Fire1") || Input.GetMouseButtonDown (0))) {
			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector3 characterPos = this.transform.position;
			characterPos.y += this.transform.localScale.y/2;
			float distMax = Vector2.Distance (mouseWorldPosition, characterPos);
			GameObject frontground = GameObject.FindGameObjectWithTag("FrontGround").gameObject;
			PlanetGenerator scriptGen = frontground.transform.parent.GetComponent<PlanetGenerator> ();
			UnityEngine.Tilemaps.Tilemap world = frontground.GetComponent<UnityEngine.Tilemaps.Tilemap> ();
			Vector3Int v = world.WorldToCell (mouseWorldPosition);
			v.x = ((v.x % scriptGen.planet.savedMapMatrix.GetLength(0)) + scriptGen.planet.savedMapMatrix.GetLength(0)) % scriptGen.planet.savedMapMatrix.GetLength(0);
			if (distMax <= 1f && v.y < scriptGen.maximalHeight - 10 && scriptGen.planet.savedMapMatrix [v.x, v.y] != scriptGen.unbreakableTileID) {
				scriptGen.planet.savedMapMatrix [v.x, v.y] = 0;
			}
		}
    }
}