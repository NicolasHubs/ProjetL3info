using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// GERER LES ERREURS D'AFFICHAGES POUR LE PANEL RECETTE 


    public class ButtonScript : MonoBehaviour
    {

        private CanvasGroup visible;
        private string temp;
        private string[] substrings;
        private int indice;
        private string recettes;
        private bool used;
        private int nbIngredients;
        private int hauteurRecettes;

        private int recetteIndice;
        private int indiceImageAjoutee;
        private int largeurMaxTemp;
        public int largeurMax;

        public int affichage;

        private bool addingScroll;

        private Item itm;
        private GameObject close;
        private Button closeButton;
        private BlueprintDatabase blueprint_database;
        private ItemDataBaseList item_database;
        public Inventory inventory;

        // Use this for initialization
        void Start()
        {
            used = false;
            affichage = 0;
            recetteIndice = 0;
            indiceImageAjoutee = 0;
            blueprint_database = (BlueprintDatabase)Resources.Load("BlueprintDatabase");
            item_database = (ItemDataBaseList)Resources.Load("ItemDatabase");
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < transform.parent.parent.GetChild(1).childCount; i++)
            {
                visible = transform.parent.GetChild(i).GetComponent<CanvasGroup>();
                if (transform.parent.parent.GetChild(1).GetChild(i).childCount == 0)
                    visible.alpha = 0;
                else
                    visible.alpha = 1;
            }
        }

        public void affichageRecettes()
        {
            addingScroll = false;
            recetteIndice = 0;
            largeurMax = 0;

            print(GameObject.FindGameObjectWithTag("PanelAccueil").GetComponent<RecettePanelAffichage>().canBeActive);
            if (GameObject.FindGameObjectWithTag("PanelAccueil").GetComponent<RecettePanelAffichage>().canBeActive)
            {
                //Initialisation du panelGeneral ainsi que du panelRecette 
                GameObject PanelGeneral = (GameObject)Instantiate(Resources.Load("Prefabs/RecettePanel") as GameObject);
                PanelGeneral.transform.SetParent(GameObject.FindGameObjectWithTag("PanelAccueil").transform);
                PanelGeneral.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                close = PanelGeneral.transform.GetChild(3).gameObject;
                closeButton = close.GetComponent<Button>();
                closeButton.onClick.AddListener(GameObject.FindGameObjectWithTag("PanelAccueil").GetComponent<RecettePanelAffichage>().close);

                //Récupération du bon slot
                indiceImageAjoutee = 0;
                temp = this.gameObject.name;
                char delimiter = ' ';
                substrings = temp.Split(delimiter);
                temp = substrings[1];
                indice = int.Parse(temp);
                itm = transform.parent.parent.GetChild(1).GetChild(indice).GetChild(0).GetComponent<ItemOnObject>().item;
                indice = itm.itemID;


                PanelGeneral.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = itm.itemIcon;
                PanelGeneral.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = itm.itemName;

                //parcours de la base de données des recettes 
                for (int j = 0; j < blueprint_database.blueprints.Count; j++)
                {
                    //Si un des ingrédients de la recette est notre item en question
                    if (blueprint_database.blueprints[j].finalItem.itemID == itm.itemID)
                    {
                        nbIngredients = blueprint_database.blueprints[j].ingredients.Count;
                        for (int ingredientsIndice = 0; ingredientsIndice < nbIngredients; ingredientsIndice++)
                        {
                            //création de l'image d'un item compris dans la recette
                            PanelGeneral.transform.GetChild(6).GetChild(ingredientsIndice).GetComponent<Image>().sprite = item_database.getItemByID(blueprint_database.blueprints[j].ingredients[ingredientsIndice]).itemIcon;
                            PanelGeneral.transform.GetChild(6).GetChild(ingredientsIndice).GetComponent<CanvasGroup>().alpha = 1;

                            //remplissage du texte indiquant le nombre d'items requis pour cette recette
                            if (blueprint_database.blueprints[j].amount[ingredientsIndice] > 1)
                                PanelGeneral.transform.GetChild(6).GetChild(ingredientsIndice).GetChild(0).GetComponent<Text>().text = blueprint_database.blueprints[j].amount[ingredientsIndice].ToString();
                        }
                    }
                }

                //parcours de la base de données des recettes 
                for (int j = 0; j < blueprint_database.blueprints.Count; j++)
                {
                    for (int k = 0; k < blueprint_database.blueprints[j].ingredients.Count; k++)
                    {
                        if (blueprint_database.blueprints[j].ingredients[k] == indice)
                            used = true;
                        if (used)
                        {
                            GameObject PanelRecette = Instantiate(Resources.Load("Prefabs/PanelRecette")) as GameObject;
                            PanelRecette.transform.SetParent(GameObject.FindGameObjectWithTag("RecettePanel").transform.GetChild(8).GetChild(0));
                            PanelRecette.GetComponent<RectTransform>().localPosition = new Vector3(0, -33.5f - ((recetteIndice - 2) * 67), 0);

                            nbIngredients = blueprint_database.blueprints[j].ingredients.Count;
                            for (int ingredientsIndice = 0; ingredientsIndice < nbIngredients; ingredientsIndice++)
                            {
                                //remplissage de l'image d'un item compris dans la recette
                                PanelGeneral.transform.GetChild(8).GetChild(0).GetChild(recetteIndice).GetChild(ingredientsIndice + 4).GetComponent<Image>().sprite = item_database.getItemByID(blueprint_database.blueprints[j].ingredients[ingredientsIndice]).itemIcon;
                                PanelGeneral.transform.GetChild(8).GetChild(0).GetChild(recetteIndice).GetChild(ingredientsIndice + 4).GetComponent<CanvasGroup>().alpha = 1;

                                //remplissage du titre de la recette 
                                PanelGeneral.transform.GetChild(8).GetChild(0).GetChild(recetteIndice).GetChild(1).GetComponent<Text>().text = blueprint_database.blueprints[j].finalItem.itemName;

                                //remplissage du type de l'item
                                PanelGeneral.transform.GetChild(8).GetChild(0).GetChild(recetteIndice).GetChild(2).GetComponent<Text>().text = blueprint_database.blueprints[j].finalItem.itemType.ToString();

                                //remplissage du texte indiquant le nombre d'items requis pour cette recette
                                if (blueprint_database.blueprints[j].amount[ingredientsIndice] > 1)
                                    PanelGeneral.transform.GetChild(8).GetChild(0).GetChild(recetteIndice).GetChild(ingredientsIndice + 4).GetChild(0).GetComponent<Text>().text = blueprint_database.blueprints[j].amount[ingredientsIndice].ToString();

                            }

                            //création de l'image de l'item final de la recette 
                            PanelGeneral.transform.GetChild(8).GetChild(0).GetChild(recetteIndice).GetChild(0).GetChild(0).GetComponent<Image>().sprite = blueprint_database.blueprints[j].finalItem.itemIcon;
                            recetteIndice++;
                            if (recetteIndice == 5)
                                addingScroll = true;
                            indiceImageAjoutee++;
                        }

                        used = false;
                        if (largeurMaxTemp > largeurMax)
                            largeurMax = largeurMaxTemp;
                    }
                }

                //Si le nombre de recettes est supérieur à 4 --> Ajout de la du scroll et de la scrollBar 
                if (addingScroll)
                {
                    GameObject.FindGameObjectWithTag("scrollBar").GetComponent<CanvasGroup>().alpha = 1;
                    GameObject.FindGameObjectWithTag("RecettePanel").transform.GetChild(8).GetChild(0).GetComponent<RectTransform>().offsetMin = new Vector2(0, -(67 * (recetteIndice - 4)));

                }
                else
                {
                    PanelGeneral.transform.GetChild(8).GetComponent<RectTransform>().localPosition += new Vector3(2.5f, 0, 0);
                }

            }
            GameObject.FindGameObjectWithTag("PanelAccueil").GetComponent<RecettePanelAffichage>().canBeActive = false;
        }
    }
