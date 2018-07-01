using UnityEngine;
using UnityEngine.UI;

    public class CraftResultSlot : MonoBehaviour
    {
        CraftSystem craftSystem;
        public int temp = 0;
        public GameObject itemGameObject;
        BlueprintDatabase blueprintDatabase;
        public GameObject mainInventory;
        bool full;
        bool neverEquals;
        int inf;

        private GameObject _player;

        // Use this for initialization
        void Start()
        {
            craftSystem = transform.parent.GetComponent<CraftSystem>();
            blueprintDatabase = (BlueprintDatabase)Resources.Load("BlueprintDatabase");
            _player = GameObject.FindGameObjectWithTag("Player");
            itemGameObject = (GameObject)Instantiate(Resources.Load("Prefabs/Item") as GameObject);
            itemGameObject.transform.SetParent(this.gameObject.transform);
            itemGameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
            itemGameObject.GetComponent<DragItem>().enabled = false;
            itemGameObject.SetActive(false);
            itemGameObject.transform.GetChild(1).GetComponent<Text>().enabled = true;
          //  itemGameObject.transform.GetChild(1).GetComponent<RectTransform>().localPosition= new Vector2(GameObject.FindGameObjectWithTag("MainInventory").GetComponent<Inventory>().positionNumberX, GameObject.FindGameObjectWithTag("MainInventory").GetComponent<Inventory>().positionNumberY);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                print("full --> " + full);
                print("neverEquals --> " + neverEquals);
                print("inf --> " + inf);

            }
            if (craftSystem.possibleItems.Count != 0 && !(_player.GetComponent<PlayerInventory>().FullInventory()))
            {
                while (craftSystem.possibleItems.Count <= temp)
                    temp--;
                itemGameObject.GetComponent<ItemOnObject>().item = craftSystem.possibleItems[temp];
                itemGameObject.SetActive(true);
                if (craftSystem.minMaxEq)
                    craftSystem.mainSlider.gameObject.SetActive(false);
                else
                    craftSystem.mainSlider.gameObject.SetActive(true);
            }
            else
            {
                itemGameObject.SetActive(false);
                craftSystem.mainSlider.gameObject.SetActive(false);
            }
        }
    }
    

