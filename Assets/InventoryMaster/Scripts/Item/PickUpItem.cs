using UnityEngine;
using System.Collections;
public class PickUpItem : MonoBehaviour
{
    public Item item;
    private Inventory _inventory;
    private GameObject _player;
    private float speedMultiplier;

    // Use this for initialization
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
            _inventory = _player.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(this.gameObject.transform.position, _player.transform.position);
        Vector3 newPos = this.gameObject.transform.position;
        Vector3 characPos = _player.transform.position;
        speedMultiplier = distance * 0.05f;

        if (newPos.x - characPos.x < 0)
        {
            newPos.x += speedMultiplier;
        }
        else if (newPos.x - characPos.x > 0)
        {
            newPos.x -= speedMultiplier;
        }

        this.gameObject.transform.position = newPos;

        if (_inventory != null && distance <= 0.3f /*Input.GetMouseButtonDown(0)*/)
        {
            /*if (distance <= 1f)
            {*/
                bool check = _inventory.checkIfItemAllreadyExist(item.itemID, item.itemValue);
                if (check)
                    Destroy(this.gameObject);
                else if (_inventory.ItemsInInventory.Count < (_inventory.width * _inventory.height))
                {
                    _inventory.addItemToInventory(item.itemID, item.itemValue);
                    _inventory.updateItemList();
                    _inventory.stackableSettings();
                    Destroy(this.gameObject);
                }
            //}
        }
    }

}