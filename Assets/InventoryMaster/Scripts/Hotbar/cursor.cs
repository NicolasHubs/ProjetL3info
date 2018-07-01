using System.Collections;
using System.Collections.Generic;
using HeroEditor.Common.Enums;
using Assets.HeroEditor.Common.CharacterScripts;
using UnityEngine;

public class cursor : MonoBehaviour
{

    public int position;
    public Vector3 temp;

    public int currentId;

    public int slotsInTotal;
    public float distMax;

    private ItemDataBaseList _database;
    private GameObject _player;
    static InputManager inputManagerDatabase = null;

    public Vector3 mouseWorldPosition;
    public Vector3 characterPos;
    public Vector3Int v;
    public Vector3Int vCharacPos;
    public bool canCreate;
    public GameObject inventory;

    private GameObject recettePanelParent;

    public Character Character;

    private Item itm;

    public PlanetGenerator scriptGen;

    public UnityEngine.Tilemaps.Tilemap world;

    // Use this for initialization
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _database = (ItemDataBaseList)Resources.Load("ItemDatabase");
        inputManagerDatabase = (InputManager)Resources.Load("InputManager");
        recettePanelParent = GameObject.FindGameObjectWithTag("RecettePanelCanvas").transform.GetChild(0).gameObject;
        //print("panel : " + recettePanelParent.GetComponent<CanvasGroup>().alpha);
        position = 1;
        currentId = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (recettePanelParent.GetComponent<CanvasGroup>().alpha == 0)
        {   
            //Gestion du curseur
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

            if (transform.parent.GetChild(1).GetChild(currentId).childCount != 0)
            {
                itm = transform.parent.GetChild(1).GetChild(currentId).GetChild(0).GetComponent<ItemOnObject>().item;
                if (itm.itemType == ItemType.MeleeWeapon || itm.itemType == ItemType.FireWeapon1 || itm.itemType == ItemType.FireWeapon2)
                {
                    _player.GetComponent<CharacterFlip>().armed = true;
                    switch (itm.itemType)
                    {
                        case ItemType.MeleeWeapon:
                            WeaponEquip();
                            break;
                        case ItemType.FireWeapon1:
                            FireWeaponEquip1();
                            break;
                        case ItemType.FireWeapon2:
                            FireWeaponEquip2();
                            break;
                    }
                }
                if (itm.itemType == ItemType.CanBeBuilt)
                {
                    Character.Animator.SetBool("CreateBloc", true);
                    UnequipWeapon();
                    createBloc();
                }
            }
            else
            {
                Character.Animator.SetBool("CreateBloc", false);
                UnequipWeapon();
            }

        }
    }

    public void initialisationDesVars()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        characterPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        characterPos.y += GameObject.FindGameObjectWithTag("Player").transform.localScale.y / 2;
        scriptGen = GameObject.FindGameObjectWithTag("FrontGround").transform.parent.GetComponent<PlanetGenerator>();
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
            Item blocToPlace = transform.parent.GetChild(1).GetChild(currentId).GetChild(0).GetComponent<ItemOnObject>().item;
            if (((_database.getItemByID(scriptGen.planet.savedMapMatrix[(v.x) - 1, v.y]) != null && _database.getItemByID(scriptGen.planet.savedMapMatrix[(v.x) - 1, v.y]).itemID > 0) ||
                 (_database.getItemByID(scriptGen.planet.savedMapMatrix[(v.x) - 1, v.y]) != null && _database.getItemByID(scriptGen.planet.savedMapMatrix[(v.x) + 1, v.y]).itemID > 0) ||
                 (_database.getItemByID(scriptGen.planet.savedMapMatrix[(v.x) - 1, v.y]) != null && _database.getItemByID(scriptGen.planet.savedMapMatrix[v.x, (v.y) - 1]).itemID > 0) ||
                 (_database.getItemByID(scriptGen.planet.savedMapMatrix[(v.x) - 1, v.y]) != null && _database.getItemByID(scriptGen.planet.savedMapMatrix[v.x, (v.y) + 1]).itemID > 0)) &&
                 _database.getItemByID(scriptGen.planet.savedMapMatrix[v.x, v.y]).itemID == 0 && (distMax <= 3f) && canCreate)
            {
                int i = 0;
                while (i < scriptGen.planet.tilesType.Count)
                {
                    if (blocToPlace.assetTile.Equals(scriptGen.planet.tilesType[i]))
                        break;
                    i++;
                }


                scriptGen.planet.savedMapMatrix[v.x, v.y] = i;
                transform.parent.GetChild(1).GetChild(currentId).GetChild(0).GetComponent<ConsumeItem>().consumeIt();  //Consumme l'objet une fois posé sur la map
            }
        }

    }

    public void WeaponEquip()
    {
        Character.Animator.SetBool("SwapToH", false);
        Character.Animator.SetBool("SwapToGun", false);
        //Character.Animator.SetBool("SwapToKnife", false);
        Character.Animator.SetBool("SwapToSub", false);
        Character.WeaponType = WeaponType.Melee1H;
        Character.Animator.SetBool("SwapToKnife", true);
    }

    public void FireWeaponEquip1()
    {
        Character.Animator.SetBool("SwapToH", false);
        //Character.Animator.SetBool("SwapToGun", false);
        Character.Animator.SetBool("SwapToKnife", false);
        Character.Animator.SetBool("SwapToSub", false);
        Character.WeaponType = WeaponType.Firearms1H;
        Character.Firearm = Character.Firearm3;
        Character.Animator.SetBool("SwapToGun", true);
    }

    public void FireWeaponEquip2()
    {
        Character.Animator.SetBool("SwapToH", false);
        //Character.Animator.SetBool("SwapToSub", false);
        Character.Animator.SetBool("SwapToGun", false);
        Character.Animator.SetBool("SwapToKnife", false);
        Character.WeaponType = WeaponType.Firearms2H;
        Character.Firearm = Character.Firearm2;
        Character.Animator.SetBool("SwapToSub", true);

    }


    public void UnequipWeapon()
    {
        Character.Animator.SetBool("SwapToH", false);
        Character.Animator.SetBool("SwapToGun", false);
        Character.Animator.SetBool("SwapToKnife", false);
        Character.Animator.SetBool("SwapToSub", false);
        Character.WeaponType = WeaponType.Melee1H;
        Character.Animator.SetBool("SwapToH", true);
        _player.GetComponent<CharacterFlip>().armed = false;
    }

}
