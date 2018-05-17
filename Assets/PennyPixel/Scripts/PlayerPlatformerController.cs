using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPlatformerController : PhysicsObject {

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;
	public GameObject frontground;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Inventory _inventory;
    private GameObject _player;

    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

     void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
            _inventory = _player.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
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

        if(move.x > 0.01f)
        {
            if(spriteRenderer.flipX == true) {
                spriteRenderer.flipX = false;
            }
        } 
        else if (move.x < -0.01f)
        {
            if(spriteRenderer.flipX == false)
            {
                spriteRenderer.flipX = true;
            }
        }

        animator.SetBool ("grounded", grounded);
        animator.SetFloat ("velocityX", Mathf.Abs (velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;

		if (Input.GetButton("Fire1") || Input.GetMouseButtonDown (0)) {
			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			float distMax = Vector2.Distance (mouseWorldPosition, this.transform.position);
			if (distMax <= 1f) {
				//RaycastHit2D hit2d = Physics2D.Raycast (transform.position, mouseWorldPosition - transform.position, distMax);
				GroundGenerator scriptGen = frontground.transform.parent.GetComponent<GroundGenerator> ();
				//GameObject clickedGameObject = hit2d.collider.gameObject;
				UnityEngine.Tilemaps.Tilemap world = frontground.GetComponent<UnityEngine.Tilemaps.Tilemap> ();
				Vector3Int v = world.WorldToCell (mouseWorldPosition);

                bool check = _inventory.checkIfItemAllreadyExist(scriptGen.getMatrix(v.x, v.y), 1); //Value pas utile pour le moment      
                if ( (check) && (v.y < scriptGen.maximalHeight - 10) && scriptGen.getMatrix(v.x, v.y) > 0)
                {
                    v.x = ((v.x % scriptGen.numberOfColumns) + scriptGen.numberOfColumns) % scriptGen.numberOfColumns;
                    scriptGen.setMatrix(v.x, v.y, 0);
                }
                else if(_inventory.ItemsInInventory.Count < (_inventory.width * _inventory.height) && (v.y < scriptGen.maximalHeight - 10) && scriptGen.getMatrix(v.x, v.y) > 0) 
                {
                    _inventory.addItemToInventory(scriptGen.getMatrix(v.x, v.y), 1);
                    _inventory.updateItemList();
                    _inventory.stackableSettings();
                    v.x = ((v.x % scriptGen.numberOfColumns) + scriptGen.numberOfColumns) % scriptGen.numberOfColumns;
                    scriptGen.setMatrix(v.x, v.y, 0);
                }
			}
		}
    }
}