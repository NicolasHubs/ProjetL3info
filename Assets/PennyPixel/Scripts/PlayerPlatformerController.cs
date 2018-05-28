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
    
    private GameObject _player;
    private GameObject _inventory;
    private ItemDataBaseList _database;

    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

     void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _inventory = GameObject.FindGameObjectWithTag("MainInventory");
        if (_player != null)
            _database = (ItemDataBaseList)Resources.Load("ItemDatabase");
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
            Vector3 characterPos = this.transform.position;
            characterPos.y += this.transform.localScale.y / 2;
            float distMax = Vector2.Distance (mouseWorldPosition, characterPos);

			if (distMax <= 1f) {
				GroundGenerator scriptGen = frontground.transform.parent.GetComponent<GroundGenerator> ();
				UnityEngine.Tilemaps.Tilemap world = frontground.GetComponent<UnityEngine.Tilemaps.Tilemap> ();
				Vector3Int v = world.WorldToCell (mouseWorldPosition);

                // if we click on an actual block (i.e. not empty)
                if (_database.getItemByID(scriptGen.mapMatrix[v.x, v.y]).itemID > 0 && !_inventory.activeSelf)
                {
                    // creates a gameObject to be displayed
                    GameObject go = (GameObject)Instantiate(_database.getItemByID(scriptGen.mapMatrix[v.x, v.y]).itemModel);
                    // adds script so it can be picked up
                    go.AddComponent<PickUpItem>();
                    go.GetComponent<PickUpItem>().item = _database.getItemByID(scriptGen.mapMatrix[v.x, v.y]);
                    mouseWorldPosition.z = 0;
                    go.transform.localPosition = mouseWorldPosition;
                    go.tag = "BrokenObject";

                    v.x = ((v.x % scriptGen.numberOfColumns) + scriptGen.numberOfColumns) % scriptGen.numberOfColumns;
                    scriptGen.mapMatrix[v.x, v.y] = 0;

                    scriptGen.mapHeight[v.x] -= 1;
                }
			}
		}
    }
}