using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursor : MonoBehaviour {

    public int position;
    public Vector3 temp;

    public int currentId;

    public int slotsInTotal;
    public float distMax;

    private ItemDataBaseList _database;
    static InputManager inputManagerDatabase = null;

    public Vector3 mouseWorldPosition;
    public Vector3 characterPos;
    public Vector3Int v;
    public Vector3Int vCharacPos;
    public bool canCreate;
    public GameObject inventory;

    public GroundGenerator scriptGen;

    public UnityEngine.Tilemaps.Tilemap world;

    // Use this for initialization
    void Start() {
        _database = (ItemDataBaseList)Resources.Load("ItemDatabase");
        inputManagerDatabase = (InputManager)Resources.Load("InputManager");
        inventory = GameObject.FindGameObjectWithTag("MainInventory");
        position = 1;
        currentId = 0;
	}

    // Update is called once per frame
    void Update()
    {
        float value = Input.GetAxis("Mouse ScrollWheel");
        if (value > 0 && position < 9)
        {
            temp = transform.position;
            temp.x += 55;
            transform.position = temp;
            position += 1;
            currentId += 1;
        }
        else if (value < 0 && position > 1)
        {
            temp = transform.position;
            temp.x -= 55;
            transform.position = temp;
            position -= 1;
            currentId -= 1;
        }

        createBloc();
    }

     public void initialisationDesVars()
     {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        characterPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        characterPos.y += GameObject.FindGameObjectWithTag("Player").transform.localScale.y / 2;
        scriptGen = GameObject.FindGameObjectWithTag("FrontGround").transform.parent.GetComponent<GroundGenerator>();
        UnityEngine.Tilemaps.Tilemap world = GameObject.FindGameObjectWithTag("FrontGround").GetComponent<UnityEngine.Tilemaps.Tilemap>();
        v = world.WorldToCell(mouseWorldPosition);
        vCharacPos = world.WorldToCell(characterPos); // Used for collisions when creating a block
        distMax = Vector2.Distance(mouseWorldPosition, characterPos);

        if ((v.x == vCharacPos.x - 1 || v.x == vCharacPos.x) &&
            (v.y == vCharacPos.y - 2 || v.y == vCharacPos.y - 1 || v.y == vCharacPos.y || v.y == vCharacPos.y + 1))
            canCreate = false;
        else
            canCreate = true;
     }

     public void createBloc()
     {
        if (transform.parent.GetChild(1).GetChild(currentId).childCount != 0 && !inventory.activeSelf && Input.GetKeyDown(inputManagerDatabase.BuildKeyCode))
        {
            initialisationDesVars();
            if ((_database.getItemByID(scriptGen.mapMatrix[(v.x) - 1, v.y]).itemID > 0 || _database.getItemByID(scriptGen.mapMatrix[(v.x) + 1, v.y]).itemID > 0 ||
                  _database.getItemByID(scriptGen.mapMatrix[v.x, (v.y) - 1]).itemID > 0 || _database.getItemByID(scriptGen.mapMatrix[v.x, (v.y) + 1]).itemID > 0)
                  && _database.getItemByID(scriptGen.mapMatrix[v.x, v.y]).itemID == 0 && (distMax <= 3f) && canCreate)
            {
                scriptGen.mapMatrix[v.x, v.y] = transform.parent.GetChild(1).GetChild(currentId).GetChild(0).GetComponent<ItemOnObject>().item.itemID;
                transform.parent.GetChild(1).GetChild(currentId).GetChild(0).GetComponent<ConsumeItem>().consumeIt();  //Consumme l'objet une fois posé sur la map
            }
        }
        
    }
}
