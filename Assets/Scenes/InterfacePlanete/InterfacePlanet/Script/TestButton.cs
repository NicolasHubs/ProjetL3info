//To use this example, attach this script to an empty GameObject.
//Create two buttons (Create>UI>Button). Next, click your empty GameObject in the Hierarchy and click and drag each of your Buttons from the Hierarchy to the Your Button and "Your Second Button" fields in the Inspector.
//Click the Button in Play Mode to output the message to the console.

using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class TestButton : MenuManager
{
	static GalaxyList globalGalaxyList = null;
    
    static void Init()
	{
			globalGalaxyList = (GalaxyList)Resources.Load("Databases/GalaxyDatabase");
	}

    //Make sure to attach these Buttons in the Inspector
    public Button m_YourButton;
    public GameObject prefabButton;

    public RectTransform ParentPanelGalaxy;//Interface systeme ou iront les boutons des galaxys
    public RectTransform ParentPanelSysteme;//Interface systeme ou iront les boutons des sytèmes
    public RectTransform ParentPanelPlanete;//Interface planète ou iront les boutons planètes
    public RectTransform background;

	public Scrollbar scrollBarGalaxy;
	public Scrollbar scrollBarSystem;
	public Scrollbar scrollBarPlanet;

	public GameObject planetInfoChildList;
	public TPInteract TPscript;

    public List<List<List<Button>>> listGalaxy = new List<List<List<Button>>>();
    public int galaxyActuelle;
    public int systemeActuel;


    //Planete actuelle
    void Start() {
        Init();
        generationGalaxy();

        mainMenuAnim = MainMenu.GetComponent<Animator>();
        systemMenuAnim = SystemMenu.GetComponent<Animator>();
        planetMenuAnim = PlanetMenu.GetComponent<Animator>();
    }

    Button generationButton()
    {
        GameObject goButton = Instantiate(prefabButton);
        Button button = goButton.GetComponent<Button>();
        return button;
    }

    void generationGalaxy() {
		bool addScroll = false;
		int nbElemAdded = 0;
		Button Buttongalaxy = generationButton();
		for (int i = 0; i < globalGalaxyList.galaxyList.Count; i++)
        {
            //listGalaxy.Add(new List<List<Button>>());
            //galaxyActuelle = i;
            Buttongalaxy = generationButton();
			Buttongalaxy.GetComponentInChildren<Text> ().text = globalGalaxyList.galaxyList [i].name;
            Buttongalaxy.transform.SetParent(ParentPanelGalaxy, false);
			nbElemAdded++;
			List<StellarSystem> stellar = globalGalaxyList.galaxyList [i].stellarSystemList;
			string galaxyName = globalGalaxyList.galaxyList [i].name;
			Buttongalaxy.GetComponent<Button>().onClick.AddListener(() => generationSystem(stellar, galaxyName));
            Buttongalaxy.GetComponent<Button>().onClick.AddListener(OnGalaxyButtonClick);
			if (nbElemAdded > 6)
				addScroll = true;
        }

		if (addScroll) {
			ParentPanelGalaxy.GetComponent<RectTransform> ().offsetMin = new Vector2(0, (55.2f*(nbElemAdded-6))*-1);
		}
    }
		
	void generationSystem(List<StellarSystem> stellarSystem, string galaxyName)
    {
		if (!galaxyGroupName.activeSelf) 
			galaxyGroupName.SetActive (true);
		
		galaxyTextFieldName.text = galaxyName;

		if (!galaxyTextName.activeSelf)
			galaxyTextName.SetActive (true);
		
		ClearStellarList ();
		bool addScroll = false;
		int nbElemAdded = 0;
		Button buttonSysteme = generationButton();
		for(int i = 0; i < stellarSystem.Count; i++)
        {
            buttonSysteme = generationButton();
			buttonSysteme.GetComponentInChildren<Text> ().text = stellarSystem[i].name;
			buttonSysteme.transform.SetParent(ParentPanelSysteme, false);
			nbElemAdded++;
			List<Planet> planet = stellarSystem[i].planetList;
			string systemName = stellarSystem [i].name;
			buttonSysteme.GetComponent<Button>().onClick.AddListener(() => generationPlanete(planet,systemName));
            buttonSysteme.onClick.AddListener(OnSystemButtonClick);
			if (nbElemAdded > 6)
				addScroll = true;
        }

		if (addScroll) {
			ParentPanelSysteme.GetComponent<RectTransform> ().offsetMin = new Vector2(0, (55.2f*(nbElemAdded-6))*-1);
		}
    }

	void ClearStellarList(){
		if (ParentPanelSysteme.childCount != 0) {
			for (int i = 0; i < ParentPanelSysteme.childCount; i++) {
				Destroy (ParentPanelSysteme.GetChild (i).gameObject);
			}
		}
		ParentPanelSysteme.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, 0);
	}

	public void OnGoToThePlanetClick(){
		if (globalGalaxyList == null)
			globalGalaxyList = (GalaxyList)Resources.Load("Databases/GalaxyDatabase");

        if (systemTextName.activeSelf)
        {
            systemTextName.SetActive(false);
            systemGroupName.SetActive(false);
        }

        if (planetTextName.activeSelf)
            planetTextName.SetActive(false);

        if (sphere.activeSelf)
            sphere.SetActive(false);

        if (planetInfo.activeSelf)
            planetInfo.SetActive(false);

        if (galaxyTextName.activeSelf)
        {
            galaxyTextName.SetActive(false);
            galaxyGroupName.SetActive(false);
        }

        TPscript.setPlanet(loadFromJson(globalGalaxyList.galaxyList[a].stellarSystemList[b].planetList[c]));
        dashboard.SetActive(!dashboard.activeSelf);
    }

	Planet loadFromJson(Planet planet){
		if (globalGalaxyList == null)
			globalGalaxyList = (GalaxyList)Resources.Load("Databases/GalaxyDatabase");

		string filePath = Application.dataPath + "/Scenes/PlanetSaves/planetDataG" + planet.galaxy + "S" + planet.stellarSystem + "P" + planet.planetID + ".json";
		if (File.Exists (filePath)) {
			string dataAsJson = File.ReadAllText (filePath);
			globalGalaxyList.galaxyList [planet.galaxy].stellarSystemList [planet.stellarSystem].planetList [planet.planetID].formatPlanetFromJson (JsonUtility.FromJson<PlanetToJson> (dataAsJson));
			return planet;
		}
		return planet;
	}

	void generationPlanete(List<Planet> planetList, string systemName)
    {
		if (!systemGroupName.activeSelf) 
			systemGroupName.SetActive (true);
		
		systemTextFieldName.text = systemName;

		if (!systemTextName.activeSelf)
			systemTextName.SetActive (true);

		ClearPlanetList ();
		bool addScroll = false;
		int nbElemAdded = 0;
		Button buttonPlanete = generationButton();
		for (int i = 0; i < planetList.Count; i++)
        {
			buttonPlanete = generationButton();
			buttonPlanete.GetComponentInChildren<Text> ().text = planetList[i].name;
			buttonPlanete.transform.SetParent(ParentPanelPlanete, false);
			int galaxy = planetList [i].galaxy;
			int stellarSystem = planetList [i].stellarSystem;
			Material planetMaterial = randomPlanetMaterial(planetList[i].planetType.type, galaxy, stellarSystem, i);
			Planet planetSelected = planetList[i];
			buttonPlanete.onClick.AddListener(() => apparitionPlanete(planetMaterial,planetSelected));

			nbElemAdded++;
			if (nbElemAdded > 6)
				addScroll = true;
        }
		if (addScroll) {
			ParentPanelPlanete.GetComponent<RectTransform> ().offsetMin = new Vector2(0, (55.2f*(nbElemAdded-6))*-1);
		}
    }

	void ClearPlanetList(){
		if (ParentPanelPlanete.childCount != 0) {
			for (int i = 0; i < ParentPanelPlanete.childCount; i++) {
				Destroy (ParentPanelPlanete.GetChild (i).gameObject);
			}
		}
		scrollBarPlanet.value = 1;
		scrollBarPlanet.size = 1;
		ParentPanelPlanete.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, 0);
	}

	private Material randomPlanetMaterial(string planetType, int galaxy, int stellarSystem, int planetNumber){
		if (globalGalaxyList == null)
			globalGalaxyList = (GalaxyList)Resources.Load("Databases/GalaxyDatabase");
		
		if (globalGalaxyList.galaxyList [galaxy].stellarSystemList [stellarSystem].planetList [planetNumber].planetMaterial == null) {
			List<string> materialList = new List<string> ();

			foreach (Object o in Resources.LoadAll("PlanetsTextures/"+planetType,typeof(Material))) {
				materialList.Add (o.name);
			}
			
			int randomMaterial = UnityEngine.Random.Range (0, materialList.Count);

			globalGalaxyList.galaxyList [galaxy].stellarSystemList [stellarSystem].planetList [planetNumber].planetMaterial = (Material)Resources.Load ("PlanetsTextures/" + planetType + "/" + materialList [randomMaterial], typeof(Material));
		}

		return globalGalaxyList.galaxyList [galaxy].stellarSystemList [stellarSystem].planetList [planetNumber].planetMaterial;

	}

	private int a,b,c;

	void setPlanetPosition (int a, int b, int c){
		this.a = a;
		this.b = b;
		this.c = c;
	}

	void apparitionPlanete(Material planetMaterial, Planet planet)
    {
		if (globalGalaxyList == null)
			globalGalaxyList = (GalaxyList)Resources.Load("Databases/GalaxyDatabase");
		
		if (!planetInfo.activeSelf) 
			planetInfo.SetActive (true);
		
		setPlanetPosition(planet.galaxy,planet.stellarSystem,planet.planetID);
		SetPlanetInfo (planet);
		planetTextFieldName.text = planet.name;
		Renderer rend = new Renderer();
		rend = sphere.GetComponent<Renderer>();
		rend.material = planetMaterial;

		if (!sphere.activeSelf)
			sphere.SetActive (true);

		if (!planetTextName.activeSelf)
			planetTextName.SetActive (true);
    }

	void SetPlanetInfo(Planet planet){
		planetInfoChildList.transform.GetChild (0).GetChild (1).GetComponent<Text> ().text = planet.planetType.type;
		planetInfoChildList.transform.GetChild (1).GetChild (1).GetComponent<Text> ().text = planet.horizontalSize.ToString();
		planetInfoChildList.transform.GetChild (2).GetChild (1).GetComponent<Text> ().text = planet.verticalSize.ToString();
		planetInfoChildList.transform.GetChild (3).GetChild (1).GetComponent<Text> ().text = planet.gravity.ToString();
		planetInfoChildList.transform.GetChild (4).GetChild (1).GetComponent<Text> ().text = planet.caveWidth.ToString();
		planetInfoChildList.transform.GetChild (5).GetChild (1).GetComponent<Text> ().text = planet.smoothness.ToString();
		planetInfoChildList.transform.GetChild (6).GetChild (1).GetComponent<Text> ().text = planet.caveQuantity.ToString();
		planetInfoChildList.transform.GetChild (7).GetChild (1).GetComponent<Text> ().text = planet.heightMultiplier.ToString();
		planetInfoChildList.transform.GetChild (8).GetChild (1).GetComponent<Text> ().text = planet.daySpeedMultiplier.ToString();
		if (planet.oreList.Count > 0) {
			planetInfoChildList.transform.GetChild (9).GetChild (1).GetComponent<Text> ().text = planet.oreList [0].name;
		}
	}
}
