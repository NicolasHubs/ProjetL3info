using UnityEngine;
using UnityEngine.UI;


    public class PlayerInventory : MonoBehaviour
    {
        public GameObject inventory;
        public GameObject characterSystem;
        public GameObject craftSystem;
        private Inventory craftSystemInventory;
        private CraftSystem cS;
        private Inventory mainInventory;
        private Inventory characterSystemInventory;
        private Tooltip toolTip;

        private InputManager inputManagerDatabase;
        private BlueprintDatabase blueprintDatabase;

        //Variables used for FullInventory()
        bool full = false;
        bool neverEquals;
        int inf;
        public int brokenItemID = 0;

        Image hpImage;
        //Image manaImage;
        Image expImage;

        //max global stats
        public float maxHealth = 100;
        /*float maxMana = 100;*/
        float maxDamage = 100;
        float maxArmor = 100;

        //current global stats
        public float currentHealth = 100;
        /*float currentMana = 100;*/
        public float currentDamage = 0;
        public float currentArmor = 0;

        //experience and levels
        public int maxLevel = 100;
        public int expToLevelUp = 2000;
        public int currentLevel = 1;
        public float currentExp = 0;

        public float percentageHp;
        public float percentageExp;

        //used for the death of the player
        public PhysicsObject po;

        int normalSize = 3;

        public void OnEnable()
        {
            Inventory.ItemEquip += OnBackpack;
            Inventory.UnEquipItem += UnEquipBackpack;

            Inventory.ItemEquip += OnGearItem;
            Inventory.ItemConsumed += OnConsumeItem;
            Inventory.UnEquipItem += OnUnEquipItem;

            Inventory.ItemEquip += EquipWeapon;
            Inventory.UnEquipItem += UnEquipWeapon;
        }

        public void OnDisable()
        {
            Inventory.ItemEquip -= OnBackpack;
            Inventory.UnEquipItem -= UnEquipBackpack;

            Inventory.ItemEquip -= OnGearItem;
            Inventory.ItemConsumed -= OnConsumeItem;
            Inventory.UnEquipItem -= OnUnEquipItem;

            Inventory.UnEquipItem -= UnEquipWeapon;
            Inventory.ItemEquip -= EquipWeapon;
        }

        void EquipWeapon(Item item)
        {
            if (item.itemType == ItemType.Weapon)
            {
                //add the weapon if you equip the weapon
            }
        }

        void UnEquipWeapon(Item item)
        {
            if (item.itemType == ItemType.Weapon)
            {
                //delete the weapon if you unequip the weapon
            }
        }

        void OnBackpack(Item item)
        {
            if (item.itemType == ItemType.Backpack)
            {
                for (int i = 0; i < item.itemAttributes.Count; i++)
                {
                    if (mainInventory == null)
                        mainInventory = inventory.GetComponent<Inventory>();
                    mainInventory.sortItems();
                    if (item.itemAttributes[i].attributeName == "Slots")
                        changeInventorySize(item.itemAttributes[i].attributeValue);
                }
            }
        }

        void UnEquipBackpack(Item item)
        {
            if (item.itemType == ItemType.Backpack)
                changeInventorySize(normalSize);
        }

        void changeInventorySize(int size)
        {
            dropTheRestItems(size);

            if (mainInventory == null)
                mainInventory = inventory.GetComponent<Inventory>();
            if (size == 3)
            {
                mainInventory.width = 3;
                mainInventory.height = 1;
                mainInventory.updateSlotAmount();
                mainInventory.adjustInventorySize();
            }
            if (size == 6)
            {
                mainInventory.width = 3;
                mainInventory.height = 2;
                mainInventory.updateSlotAmount();
                mainInventory.adjustInventorySize();
            }
            else if (size == 12)
            {
                mainInventory.width = 4;
                mainInventory.height = 3;
                mainInventory.updateSlotAmount();
                mainInventory.adjustInventorySize();
            }
            else if (size == 16)
            {
                mainInventory.width = 4;
                mainInventory.height = 4;
                mainInventory.updateSlotAmount();
                mainInventory.adjustInventorySize();
            }
            else if (size == 24)
            {
                mainInventory.width = 6;
                mainInventory.height = 4;
                mainInventory.updateSlotAmount();
                mainInventory.adjustInventorySize();
            }
        }

        void dropTheRestItems(int size)
        {
            if (size < mainInventory.ItemsInInventory.Count)
            {
                for (int i = size; i < mainInventory.ItemsInInventory.Count; i++)
                {
                    GameObject dropItem = (GameObject)Instantiate(mainInventory.ItemsInInventory[i].itemModel);
                    dropItem.AddComponent<PickUpItem>();
                    dropItem.GetComponent<PickUpItem>().item = mainInventory.ItemsInInventory[i];
                    dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition;
                }
            }
        }

        void Start()
        {
            hpImage = GameObject.Find("CurrentHp").GetComponent<Image>();
            expImage = GameObject.Find("CurrentExp").GetComponent<Image>();
            po = gameObject.GetComponent<PhysicsObject>();
            blueprintDatabase = (BlueprintDatabase)Resources.Load("BlueprintDatabase");

            if (inputManagerDatabase == null)
                inputManagerDatabase = (InputManager)Resources.Load("InputManager");

            if (craftSystem != null)
                cS = craftSystem.GetComponent<CraftSystem>();

            if (GameObject.FindGameObjectWithTag("Tooltip") != null)
                toolTip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>();
            if (inventory != null)
                mainInventory = inventory.GetComponent<Inventory>();
            if (characterSystem != null)
                characterSystemInventory = characterSystem.GetComponent<Inventory>();
            if (craftSystem != null)
                craftSystemInventory = craftSystem.GetComponent<Inventory>();
        }

        public void ApplyDamage(float TheDamage)
        {
            currentHealth = currentHealth - (TheDamage - ((currentArmor * TheDamage) / 100));

            if (currentHealth <= 0)
                Dead();
        }

        public void Dead()
        {
            //po.isDead = true;
        }

        public void GainExp(float TheExp)
        {
            currentExp += TheExp;

            if (currentExp >= expToLevelUp)
            {
                currentLevel += 1;
                currentExp = currentExp % expToLevelUp;
                expToLevelUp += currentLevel * 100;
            }
        }

        public void OnConsumeItem(Item item)
        {
            for (int i = 0; i < item.itemAttributes.Count; i++)
            {
                if (item.itemAttributes[i].attributeName == "Health")
                {
                    if ((currentHealth + item.itemAttributes[i].attributeValue) > maxHealth)
                        currentHealth = maxHealth;
                    else
                        currentHealth += item.itemAttributes[i].attributeValue;
                }

                /*if (item.itemAttributes[i].attributeName == "Mana")
                {
                    if ((currentMana + item.itemAttributes[i].attributeValue) > maxMana)
                        currentMana = maxMana;
                    else
                        currentMana += item.itemAttributes[i].attributeValue;
                }*/

                if (item.itemAttributes[i].attributeName == "Armor")
                {
                    if ((currentArmor + item.itemAttributes[i].attributeValue) > maxArmor)
                        currentArmor = maxArmor;
                    else
                        currentArmor += item.itemAttributes[i].attributeValue;
                }

                if (item.itemAttributes[i].attributeName == "Damage")
                {
                    if ((currentDamage + item.itemAttributes[i].attributeValue) > maxDamage)
                        currentDamage = maxDamage;
                    else
                        currentDamage += item.itemAttributes[i].attributeValue;
                }
            }
        }

        public void OnGearItem(Item item)
        {
            for (int i = 0; i < item.itemAttributes.Count; i++)
            {
                if (item.itemAttributes[i].attributeName == "Health")
                    currentHealth += item.itemAttributes[i].attributeValue;
                /*if (item.itemAttributes[i].attributeName == "Mana")
                    currentMana += item.itemAttributes[i].attributeValue;*/
                if (item.itemAttributes[i].attributeName == "Armor")
                    currentArmor += item.itemAttributes[i].attributeValue;
                if (item.itemAttributes[i].attributeName == "Damage")
                    currentDamage += item.itemAttributes[i].attributeValue;
            }
        }

        public void OnUnEquipItem(Item item)
        {
            for (int i = 0; i < item.itemAttributes.Count; i++)
            {
                if (item.itemAttributes[i].attributeName == "Health")
                    currentHealth -= item.itemAttributes[i].attributeValue;
                /*if (item.itemAttributes[i].attributeName == "Mana")
                    currentMana -= item.itemAttributes[i].attributeValue;*/
                if (item.itemAttributes[i].attributeName == "Armor")
                    currentArmor -= item.itemAttributes[i].attributeValue;
                if (item.itemAttributes[i].attributeName == "Damage")
                    currentDamage -= item.itemAttributes[i].attributeValue;
            }
        }

        // Update is called once per frame
        void Update()
        {
            percentageHp = ((currentHealth * 100) / maxHealth) / 100;
            hpImage.fillAmount = percentageHp;
            percentageExp = ((currentExp * 100) / expToLevelUp) / 100;
            expImage.fillAmount = percentageExp;

            //Tests damage
            if (Input.GetKeyDown(KeyCode.L))
                ApplyDamage(10);

            if (Input.GetKeyDown(inputManagerDatabase.CharacterSystemKeyCode))
            {
                if (!characterSystem.activeSelf)
                {
                    characterSystemInventory.openInventory();
                }
                else
                {
                    if (toolTip != null)
                        toolTip.deactivateTooltip();
                    characterSystemInventory.closeInventory();
                }
            }

            if (Input.GetKeyDown(inputManagerDatabase.InventoryKeyCode))
            {
                if (!inventory.activeSelf)
                {
                    mainInventory.openInventory();
                }
                else
                {
                    if (toolTip != null)
                        toolTip.deactivateTooltip();
                    if (GameObject.FindGameObjectWithTag("PanelAccueil").transform.childCount > 0)
                        GameObject.FindGameObjectWithTag("PanelAccueil").transform.GetComponent<RecettePanelAffichage>().close();
                    mainInventory.closeInventory();
                }
            }

            if (Input.GetKeyDown(inputManagerDatabase.CraftSystemKeyCode))
            {
                if (!craftSystem.activeSelf)
                {
                    craftSystemInventory.openInventory();
                }
                else
                {
                    if (cS != null)
                        cS.backToInventory();
                    if (toolTip != null)
                        toolTip.deactivateTooltip();
                    craftSystemInventory.closeInventory();
                    for (int i = 0; i < blueprintDatabase.blueprints.Count; i++)
                    {
                        blueprintDatabase.blueprints[i].amountOfFinalItem = 1;
                    }
                }
            }
        }

        /* Checking if the inventory is full or not, in different cases */
        public bool FullInventory()
        {
            full = false;
            neverEquals = true;
            
            if (mainInventory.ItemsInInventory.Count.Equals(inventory.transform.GetChild(1).childCount))
            {
                //Check in the context of the craft of an item
                if (craftSystem.transform.GetChild(3).gameObject.activeSelf && craftSystem.activeSelf)
                {
                    for (int i = 0; i < mainInventory.ItemsInInventory.Count; i++)
                    {
                        if (craftSystem.transform.GetChild(3).GetComponent<CraftResultSlot>().itemGameObject.GetComponent<ItemOnObject>().item.itemID.Equals(mainInventory.ItemsInInventory[i].itemID))
                        {
                            neverEquals = false;
                            if (int.Parse(inventory.transform.GetChild(1).GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>().text) >= mainInventory.ItemsInInventory[i].maxStack)
                            {
                                full = true;
                            }
                            else
                            {
                                full = false;
                                craftSystem.transform.GetChild(3).GetComponent<CraftResultSlot>().itemGameObject.GetComponent<ItemOnObject>().item.itemValue = Mathf.RoundToInt(craftSystem.GetComponent<CraftSystem>().mainSlider.value);
                                break;
                            }
                        }
                    }
                }
                //Checking in the context of an item being picked up
                if (!inventory.activeSelf && !craftSystem.activeSelf && !characterSystem.activeSelf)
                {
                    if (brokenItemID > 0)
                    {
                        for (int j = 0; j < mainInventory.ItemsInInventory.Count; j++)
                        {
                            if (brokenItemID.Equals(mainInventory.ItemsInInventory[j].itemID))
                            {
                                neverEquals = false;
                                if (mainInventory.ItemsInInventory[j].itemValue.Equals(mainInventory.ItemsInInventory[j].maxStack))
                                    full = true;
                                else
                                {
                                    full = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                return full || neverEquals;
            }
            else
                return false;
        }
    }
