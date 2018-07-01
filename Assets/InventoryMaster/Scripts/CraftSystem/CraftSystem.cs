using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using System.Collections.Generic;

    public class CraftSystem : MonoBehaviour
    {
        [SerializeField]
        public int finalSlotPositionX;
        [SerializeField]
        public int finalSlotPositionY;
        [SerializeField]
        public int leftArrowPositionX;
        [SerializeField]
        public int leftArrowPositionY;
        [SerializeField]
        public int rightArrowPositionX;
        [SerializeField]
        public int rightArrowPositionY;
        [SerializeField]
        public int leftArrowRotation;
        [SerializeField]
        public int rightArrowRotation;

        public Image finalSlotImage;
        public Image arrowImage;

        //List<CraftSlot> slots = new List<CraftSlot>();
        public List<Item> itemInCraftSystem = new List<Item>();
        public List<GameObject> itemInCraftSystemGameObject = new List<GameObject>();
        BlueprintDatabase blueprintDatabase;
        public List<Item> possibleItems = new List<Item>();
        public List<bool> possibletoCreate = new List<bool>();

        public Slider mainSlider;
        public InputField input;
        public int itemIndex;
        public bool minMaxEq = false;
        public GameObject inv;

        int indexJ;
        int indexAmount;

        //PlayerScript PlayerstatsScript;

        // Use this for initialization
        void Start()
        {
            blueprintDatabase = (BlueprintDatabase)Resources.Load("BlueprintDatabase");
            inv = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().inventory;
            mainSlider = GameObject.FindGameObjectWithTag("Slider").transform.GetComponent<Slider>();
            input = GameObject.FindGameObjectWithTag("CraftItemNumber").GetComponent<InputField>();
            //playerStatsScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        }

#if UNITY_EDITOR
        [MenuItem("Master System/Create/Craft System")]
        public static void menuItemCreateInventory()
        {
            GameObject Canvas = null;
            if (GameObject.FindGameObjectWithTag("Canvas") == null)
            {
                GameObject inventory = new GameObject();
                inventory.name = "Inventories";
                Canvas = (GameObject)Instantiate(Resources.Load("Prefabs/Canvas - Inventory") as GameObject);
                Canvas.transform.SetParent(inventory.transform, true);
                GameObject panel = (GameObject)Instantiate(Resources.Load("Prefabs/Panel - CraftSytem") as GameObject);
                panel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                panel.transform.SetParent(Canvas.transform, true);
                GameObject draggingItem = (GameObject)Instantiate(Resources.Load("Prefabs/DraggingItem") as GameObject);
                Instantiate(Resources.Load("Prefabs/EventSystem") as GameObject);
                draggingItem.transform.SetParent(Canvas.transform, true);
                panel.AddComponent<CraftSystem>();
            }
            else
            {
                GameObject panel = (GameObject)Instantiate(Resources.Load("Prefabs/Panel - CraftSystem") as GameObject);
                panel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, true);
                panel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                panel.AddComponent<CraftSystem>();
                DestroyImmediate(GameObject.FindGameObjectWithTag("DraggingItem"));
                GameObject draggingItem = (GameObject)Instantiate(Resources.Load("Prefabs/DraggingItem") as GameObject);
                draggingItem.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, true);
            }
        }
#endif

        void Update()
        {
            ListWithItem();
            SliderManagement();
            InputFieldManagement();
        }

        public void setImages()
        {
            finalSlotImage = transform.GetChild(3).GetComponent<Image>();
            arrowImage = transform.GetChild(4).GetComponent<Image>();

            Image image = transform.GetChild(5).GetComponent<Image>();
            image.sprite = arrowImage.sprite;
            image.color = arrowImage.color;
            image.material = arrowImage.material;
            image.type = arrowImage.type;
            image.fillCenter = arrowImage.fillCenter;
        }

        public void setArrowSettings()
        {
            RectTransform leftRect = transform.GetChild(4).GetComponent<RectTransform>();
            RectTransform rightRect = transform.GetChild(5).GetComponent<RectTransform>();

            leftRect.localPosition = new Vector3(leftArrowPositionX, leftArrowPositionY, 0);
            rightRect.localPosition = new Vector3(rightArrowPositionX, rightArrowPositionY, 0);

            leftRect.eulerAngles = new Vector3(0, 0, leftArrowRotation);
            rightRect.eulerAngles = new Vector3(0, 0, rightArrowRotation);
        }

        public void setPositionFinalSlot()
        {
            RectTransform rect = transform.GetChild(3).GetComponent<RectTransform>();
            rect.localPosition = new Vector3(finalSlotPositionX, finalSlotPositionY, 0);
        }

        public int getSizeX()
        {
            return (int)GetComponent<RectTransform>().sizeDelta.x;
        }

        public int getSizeY()
        {
            return (int)GetComponent<RectTransform>().sizeDelta.y;
        }

        public void backToInventory()
        {
            int length = itemInCraftSystem.Count;
            for (int i = 0; i < length; i++)
            {
                inv.GetComponent<Inventory>().addItemToInventory(itemInCraftSystem[i].itemID, itemInCraftSystem[i].itemValue);
                Destroy(itemInCraftSystemGameObject[i]);
            }

            itemInCraftSystem.Clear();
            itemInCraftSystemGameObject.Clear();
        }

        public void ListWithItem()
        {
            itemInCraftSystem.Clear();
            possibleItems.Clear();
            possibletoCreate.Clear();
            itemInCraftSystemGameObject.Clear();

            for (int i = 0; i < transform.GetChild(1).childCount; i++)
            {
                Transform trans = transform.GetChild(1).GetChild(i);
                if (trans.childCount != 0)
                {
                    itemInCraftSystem.Add(trans.GetChild(0).GetComponent<ItemOnObject>().item);
                    itemInCraftSystemGameObject.Add(trans.GetChild(0).gameObject);
                }
            }

            for (int k = 0; k < blueprintDatabase.blueprints.Count; k++)
            {
                int amountOfTrue = 0;
                for (int z = 0; z < blueprintDatabase.blueprints[k].ingredients.Count; z++)
                {
                    for (int d = 0; d < itemInCraftSystem.Count; d++)
                    {
                        if (blueprintDatabase.blueprints[k].ingredients[z] == itemInCraftSystem[d].itemID && blueprintDatabase.blueprints[k].amount[z] <= itemInCraftSystem[d].itemValue)
                        {
                            amountOfTrue++;
                            break;
                        }
                    }
                    if (amountOfTrue == blueprintDatabase.blueprints[k].ingredients.Count)
                    {
                        possibleItems.Add(blueprintDatabase.blueprints[k].finalItem);
                        possibleItems[possibleItems.Count - 1].itemValue = blueprintDatabase.blueprints[k].amountOfFinalItem;
                        possibletoCreate.Add(true);
                    }
                }
            }
        }

        /* Managing the behaviour of the slider */
        private void SliderManagement()
        {
            //adjusting the amount of the final item to the value we choose with the slider
            for (int i = 0; i < blueprintDatabase.blueprints.Count; i++)
            {
                if (blueprintDatabase.blueprints[i].finalItem.Equals(transform.GetChild(3).GetChild(0).GetComponent<ItemOnObject>().item))
                {
                    blueprintDatabase.blueprints[i].amountOfFinalItem = Mathf.RoundToInt(mainSlider.value);
                    itemIndex = i;
                }
            }

            /*setting the slider at false if the amount of item we are crafting is 1
             * setting it back at true if we have more than 1*/
            if (itemInCraftSystem.Count > 0)
            {
                for (int j = 0; j < itemInCraftSystem.Count; j++)
                {
                    if (itemInCraftSystem[j].itemID.Equals(blueprintDatabase.blueprints[itemIndex].ingredients[0]))
                    {
                        MaxPossibleNumber(itemIndex, j);
                        if (mainSlider.maxValue.Equals(mainSlider.minValue))
                        {
                            minMaxEq = true;
                            blueprintDatabase.blueprints[itemIndex].amountOfFinalItem = 1;
                        }
                        else
                        {
                            minMaxEq = false;
                        }
                    }
                }
            }
            else
                mainSlider.maxValue = 1;
        }

        /* Managing the behaviour of the inputField */
        private void InputFieldManagement()
        {
            if (!mainSlider.IsActive())
                input.gameObject.SetActive(false);
            //if you have an item to craft
            else
            {
                input.gameObject.SetActive(true);
                //if the inputField is not selected, take the slider value
                if (!input.isFocused)
                {
                    input.text = "" + mainSlider.value;
                    input.textComponent.text = "" + mainSlider.value;
                }
                else
                {
                    /*if the inputField is selected and its value is empty (e.g. we deleted the value)
                     * puts the minimum value*/
                    if (input.text.Equals(""))
                    {
                        mainSlider.value = mainSlider.minValue;
                        GameObject.FindGameObjectWithTag("ResultSlot").GetComponent<CraftResultSlot>().itemGameObject.GetComponent<ItemOnObject>().item.itemValue = Mathf.RoundToInt(mainSlider.minValue);
                        input.text = "" + mainSlider.value;
                        input.textComponent.text = "" + mainSlider.value;
                    }
                    /*if we try to enter more than what we can craft
                     * puts the maximum value*/
                    if (int.Parse(input.text) > mainSlider.maxValue)
                    {
                        mainSlider.value = mainSlider.maxValue;
                        GameObject.FindGameObjectWithTag("ResultSlot").GetComponent<CraftResultSlot>().itemGameObject.GetComponent<ItemOnObject>().item.itemValue = Mathf.RoundToInt(mainSlider.maxValue);
                        input.text = "" + mainSlider.maxValue;
                        input.textComponent.text = "" + mainSlider.maxValue;
                    }
                    /*if everything is fine
                     * puts the slider value to the entered value*/
                    else
                    {
                        mainSlider.value = int.Parse(input.text);
                        GameObject.FindGameObjectWithTag("ResultSlot").GetComponent<CraftResultSlot>().itemGameObject.GetComponent<ItemOnObject>().item.itemValue = int.Parse(input.text);
                    }
                }
            }
        }

        /*Searching what's the maximum amount of the final item we can craft
         * depending on the number of resources we have and the remaining
         * places we have in the main inventory*/
        private void MaxPossibleNumber(int i, int j)
        {
            int amountIndex = 0;
            int numberOfFreeSlot = 0;
            int remainingStacks = 0;

            //if the final item has more than 1 ingredient
            if (blueprintDatabase.blueprints[itemIndex].ingredients.Count > 1)
            {
                MultipleIngredientsRecipeCalculation();
                j = indexJ;
                amountIndex = indexAmount;
            }

            //maximum amount with resources only
            float maximumAmountWithResources = Mathf.Floor(itemInCraftSystem[j].itemValue / blueprintDatabase.blueprints[itemIndex].amount[amountIndex]);

            //maximum amount with main inventory's remaining places
            for (int h = 0; h < inv.transform.GetChild(1).childCount; h++)
            {
                //if slot is empty then we can put maxStack of the item
                if (inv.transform.GetChild(1).GetChild(h).childCount == 0)
                    numberOfFreeSlot++;
                //else we look if the item is already in the main inventory
                else
                {
                    if (inv.transform.GetChild(1).GetChild(h).GetChild(0).GetComponent<ItemOnObject>().item.itemID.Equals(blueprintDatabase.blueprints[itemIndex].finalItem.itemID))
                    {
                        //if it is, we check if all the stacks are filled (i.e. maxStack is reached)
                        if (inv.transform.GetChild(1).GetChild(h).GetChild(0).GetComponent<ItemOnObject>().item.itemValue != inv.transform.GetChild(1).GetChild(h).GetChild(0).GetComponent<ItemOnObject>().item.maxStack)
                            //if not, we have some more places
                            remainingStacks = inv.transform.GetChild(1).GetChild(h).GetChild(0).GetComponent<ItemOnObject>().item.maxStack - inv.transform.GetChild(1).GetChild(h).GetChild(0).GetComponent<ItemOnObject>().item.itemValue;
                    }
                }
            }
            float maximumAmountWithInventory = numberOfFreeSlot * GameObject.FindGameObjectWithTag("ResultSlot").transform.GetChild(0).GetComponent<ItemOnObject>().item.maxStack + remainingStacks;

            //adjust the slider's maxValue based on the minimum out of the 2 maximum's
            if (maximumAmountWithResources < maximumAmountWithInventory)
                mainSlider.maxValue = maximumAmountWithResources;
            else
                mainSlider.maxValue = maximumAmountWithInventory;
        }

        /*Searching what's the maximum amount of the final item
         * that we can craft if there are multiple ingredients,
         * according to the number of resources we have and
         * the amount of each one we need to craft the final item*/
        private void MultipleIngredientsRecipeCalculation()
        {
            int max = int.MaxValue;
            int actualNumber = int.MaxValue;
            indexJ = 0;
            indexAmount = 0;

            //for each ingredient
            for (int i = 0; i < blueprintDatabase.blueprints[itemIndex].ingredients.Count; i++)
            {
                for (int j = 0; j < itemInCraftSystem.Count; j++)
                {
                    //is it in the craftSystem ?
                    if (itemInCraftSystem[j].itemID.Equals(blueprintDatabase.blueprints[itemIndex].ingredients[i]))
                    {
                        //if yes, what's the amount of final item we can craft thanks to the resources we have
                        //with this ingredient
                        actualNumber = itemInCraftSystem[j].itemValue / blueprintDatabase.blueprints[itemIndex].amount[i];
                    }
                    //looking for the maximum amount we can craft including all amount we have and need for all ingredients
                    if (actualNumber < max)
                    {
                        max = actualNumber;
                        indexJ = j;
                        indexAmount = i;
                    }
                }
            }
        }

        public void deleteItems(Item item)
        {
            for (int i = 0; i < blueprintDatabase.blueprints.Count; i++)
            {
                if (blueprintDatabase.blueprints[i].finalItem.Equals(item))
                {
                    for (int k = 0; k < blueprintDatabase.blueprints[i].ingredients.Count; k++)
                    {
                        for (int z = 0; z < itemInCraftSystem.Count; z++)
                        {
                            if (itemInCraftSystem[z].itemID == blueprintDatabase.blueprints[i].ingredients[k])
                            {
                                if (itemInCraftSystem[z].itemValue == blueprintDatabase.blueprints[i].amount[k] * Mathf.FloorToInt(mainSlider.value))
                                {
                                    itemInCraftSystem.RemoveAt(z);
                                    Destroy(itemInCraftSystemGameObject[z]);
                                    itemInCraftSystemGameObject.RemoveAt(z);
                                    ListWithItem();
                                    break;
                                }
                                else if (itemInCraftSystem[z].itemValue >= blueprintDatabase.blueprints[i].amount[k] * Mathf.FloorToInt(mainSlider.value))
                                {
                                    itemInCraftSystem[z].itemValue = itemInCraftSystem[z].itemValue - (blueprintDatabase.blueprints[i].amount[k] * Mathf.FloorToInt(mainSlider.value));
                                    ListWithItem();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
