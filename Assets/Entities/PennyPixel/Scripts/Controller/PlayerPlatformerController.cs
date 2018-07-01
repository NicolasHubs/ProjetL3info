using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPlatformerController : PhysicsObject {

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;
    //private GameObject frontground;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private GameObject _player;
    public GameObject _inventory;
    public GameObject _craftSystem;
    public GameObject _equipmentSystem;
    public ItemDataBaseList _database;
    private CanvasGroup visible;

    private bool m_FacingRight = true;

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
            _database = (ItemDataBaseList)Resources.Load("ItemDatabase");
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpTakeOffSpeed;
            animator.SetBool("Jump", true);
        }
        else if (Input.GetButtonDown("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
            animator.SetBool("Jump", true);
        }
        else
            animator.SetBool("Jump", false);

        if (move.x != 0)
        {
            if (move.x < 0 && m_FacingRight)
            {
                m_FacingRight = false;
                Vector3 theScale = transform.GetChild(0).localScale;
                theScale.x *= -1;
                for (int i = 0; i < transform.childCount - 2; i++)
                    transform.GetChild(i).localScale = theScale;
            }
            if (move.x > 0 && !m_FacingRight)
            {
                m_FacingRight = true;
                Vector3 theScale = transform.GetChild(0).localScale;
                theScale.x *= -1;
                for (int i = 0; i < transform.childCount - 2; i++)
                    transform.GetChild(i).localScale = theScale;
            }
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }

        /*animator.SetBool ("grounded", grounded);
        animator.SetFloat ("velocityX", Mathf.Abs (velocity.x) / maxSpeed);*/

        targetVelocity = move * maxSpeed;

        if (!("Hub".Equals(SceneManager.GetActiveScene().name)) && (Input.GetButton("Fire1") || Input.GetMouseButtonDown(0)))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 characterPos = this.transform.position;
            characterPos.y += this.transform.localScale.y / 2;
            float distMax = Vector2.Distance(mouseWorldPosition, characterPos);
            GameObject frontground = GameObject.FindGameObjectWithTag("FrontGround");
            PlanetGenerator scriptGen = frontground.transform.parent.GetComponent<PlanetGenerator>();
            UnityEngine.Tilemaps.Tilemap world = frontground.GetComponent<UnityEngine.Tilemaps.Tilemap>();
            Vector3Int v = world.WorldToCell(mouseWorldPosition);
            v.x = ((v.x % scriptGen.planet.savedMapMatrix.GetLength(0)) + scriptGen.planet.savedMapMatrix.GetLength(0)) % scriptGen.planet.savedMapMatrix.GetLength(0);

            if (scriptGen.planet.savedMapMatrix[v.x, v.y] != 0 && scriptGen.planet.savedMapMatrix[v.x, v.y] < 100 && scriptGen.planet.savedMapMatrix[v.x, (v.y) + 1]  < 100)
            {
                if (distMax <= 2f && v.y < scriptGen.maximalHeight - 10 && scriptGen.planet.savedMapMatrix[v.x, v.y] != scriptGen.unbreakableTileID && _database.getItemByID(scriptGen.planet.savedMapMatrix[v.x, v.y]).itemID > 0
                   && !_inventory.activeSelf && !_equipmentSystem.activeSelf && !_craftSystem.activeSelf)
                {
                    // creates a gameObject to be displayed
                    GameObject go = (GameObject)Instantiate(_database.getItemByTileBase(scriptGen.planet.tilesType[scriptGen.planet.savedMapMatrix[v.x, v.y]]).itemModel);
                    // adds script so it can be picked up
                    go.AddComponent<PickUpItem>();
                    go.GetComponent<PickUpItem>().item = _database.getItemByTileBase(scriptGen.planet.tilesType[scriptGen.planet.savedMapMatrix[v.x, v.y]]);
                    mouseWorldPosition.z = 0;
                    go.transform.localPosition = mouseWorldPosition;
                    scriptGen.planet.savedMapMatrix[v.x, v.y] = 0;
                    //scriptGen.planet.savedMapHeight[v.x] -= 1;
                }
            }
        }
    }


       /* if (!("Hub".Equals(SceneManager.GetActiveScene().name)) && (Input.GetButton("Fire1") || Input.GetMouseButtonDown (0))) {
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
		}*/
    
}