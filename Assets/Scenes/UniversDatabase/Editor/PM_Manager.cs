using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;

public class PM_Manager : EditorWindow {

	static FloraList globalFloraList = null;
	static PlanetTypeList globalPlanetTypeList = null;
	static WeatherList globalWeatherList = null;
	static OreList globalOreList = null;
	static PlanetList globalPlanetList = null;
	static GalaxyList globalGalaxyList = null;

	[MenuItem("Planet System/Planet Manager")]
	static void Init()
	{
		EditorWindow.GetWindow(typeof(PM_Manager));

		Object floraDatabase = Resources.Load("Databases/FloraDatabase");
		if (floraDatabase == null)
			globalFloraList = CreateFloraDatabase.createFloraDatabase ();
		else
			globalFloraList = (FloraList)Resources.Load("Databases/FloraDatabase");

		Object planetTypeDatabase = Resources.Load("Databases/PlanetTypeDatabase");
		if (planetTypeDatabase == null)
			globalPlanetTypeList = CreatePlanetTypeDatabase.createPlanetTypeDatabase ();
		else
			globalPlanetTypeList = (PlanetTypeList)Resources.Load("Databases/PlanetTypeDatabase");
	
		Object weatherDatabase = Resources.Load("Databases/WeatherDatabase");
		if (weatherDatabase == null)
			globalWeatherList = CreateWeatherDatabase.createWeatherDatabase ();
		else
			globalWeatherList = (WeatherList)Resources.Load("Databases/WeatherDatabase");

		Object oreDatabase = Resources.Load("Databases/OreDatabase");
		if (oreDatabase == null)
			globalOreList = CreateOreDatabase.createOreDatabase ();
		else
			globalOreList = (OreList)Resources.Load("Databases/OreDatabase");

		Object planetDatabase = Resources.Load("Databases/PlanetDatabase");
		if (planetDatabase == null)
			globalPlanetList = CreatePlanetDatabase.createPlanetDatabase ();
		else
			globalPlanetList = (PlanetList)Resources.Load("Databases/PlanetDatabase");

		Object galaxyDatabase = Resources.Load("Databases/GalaxyDatabase");
		if (galaxyDatabase == null)
			globalGalaxyList = CreateGalaxyDatabase.createGalaxyDatabase();
		else
			globalGalaxyList = (GalaxyList)Resources.Load("Databases/GalaxyDatabase");
		
	}

	//bool showFaunaDataBase = false;
	bool showOreDataBase = false;
	//bool showItemsDataBase = false;
	bool showFloraDataBase = false;
	bool showPlanetTypeDataBase = false;
	bool showWeatherDataBase = false;
	bool showGalaxyDataBase = false;

	Vector2 scrollPosition;

	List<List<bool>> manageFlora = new List<List<bool>>();
	List<bool> managePlanetType = new List<bool>();
	List<bool> manageWeather = new List<bool>();
	List<bool> manageOre = new List<bool>();
	List<List<List<bool>>> manageGalaxy = new List<List<List<bool>>>();

	public static int tot = 0;

	void allDatabaseBoolFalse(){
		//showFaunaDataBase = false;
		showOreDataBase = false;
		//showItemsDataBase = false;
		showFloraDataBase = false;
		showPlanetTypeDataBase = false;
		showWeatherDataBase = false;
		showGalaxyDataBase = false;
	}

	void OnGUI() {
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Quick launch")) {
			Scenes.Load("Planet", loadFromJson (globalGalaxyList.galaxyList[0].stellarSystemList[0].planetList[0]));
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Flora")) {
			//allDatabaseBoolFalse ();
			showPlanetTypeDataBase = false;
			showWeatherDataBase = false;
			showGalaxyDataBase = false;
			showOreDataBase = false;
			showFloraDataBase = !showFloraDataBase;
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Ore")) {
			//allDatabaseBoolFalse ();
			showPlanetTypeDataBase = false;
			showWeatherDataBase = false;
			showGalaxyDataBase = false;
			showFloraDataBase = false;
			showOreDataBase = !showOreDataBase;
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		/*
		if (GUILayout.Button("Items")) {
			allDatabaseBoolFalse ();
			showItemsDataBase = !showItemsDataBase;
		}*/
		if (GUILayout.Button("Planet types")) {
			//allDatabaseBoolFalse ();
			showWeatherDataBase = false;
			showOreDataBase = false;
			showGalaxyDataBase = false;
			showFloraDataBase = false;
			showPlanetTypeDataBase = !showPlanetTypeDataBase;
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Weather")) {
			//allDatabaseBoolFalse ();
			showOreDataBase = false;
			showPlanetTypeDataBase = false;
			showGalaxyDataBase = false;
			showFloraDataBase = false;
			showWeatherDataBase = !showWeatherDataBase;
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("GalaxyManager")) {
			//allDatabaseBoolFalse ();
			showPlanetTypeDataBase = false;
			showWeatherDataBase = false;
			showFloraDataBase = false;
			showGalaxyDataBase = !showGalaxyDataBase;
			showOreDataBase = false;
		}
		EditorGUILayout.EndHorizontal();

		if (showFloraDataBase)
			FloraDatabase ();
		
		if (showPlanetTypeDataBase)
			PlanetTypeDataBase();

		if (showWeatherDataBase)
			WeatherDataBase ();

		if (showOreDataBase)
			OreDataBase ();

		if (showGalaxyDataBase)
			GalaxyDataBase ();
	}

	public string[] toolbarWeatherStrings = new string[] { "Create weather", "Weather list" };
	private string weatherTextField = "";

	void WeatherDataBase() {
		EditorGUILayout.BeginVertical("Box");

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarWeatherStrings, GUILayout.Width(position.width - 18));
		GUILayout.EndHorizontal();

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

		GUILayout.Space(10);

		if (toolbarInt == 0) {
			GUI.color = Color.white;

			GUILayout.BeginVertical("Box", GUILayout.Width(position.width - 23));

			try {
				weatherTextField = EditorGUILayout.TextField("Weather name", weatherTextField , GUILayout.Width(position.width - 30));
			}
			catch { }
			GUILayout.EndVertical();

			GUI.color = Color.green;
			GUI.SetNextControlName("Add Weather");
			if (GUILayout.Button("Add Weather", GUILayout.Width(position.width - 23))) {
				if (!("".Equals (weatherTextField.Trim ())) && !weatherAlreadyCreated (weatherTextField)) {
					addWeather (weatherTextField);
					weatherTextField = "";
				} else {
					GUI.color = Color.white;
					GUILayout.Label("There is no weather in the Database");
				}
				GUI.FocusControl ("Add Weather"); 
			}
		} else if (toolbarInt == 1) {
			if (globalWeatherList == null)
				globalWeatherList = (WeatherList)Resources.Load("Databases/WeatherDatabase");
			if (globalWeatherList.weatherList.Count == 0) {
				GUILayout.Label("There is no Weather in the Database!");
			} else {
				GUILayout.BeginVertical();

				for (int i = 0; i < globalWeatherList.weatherList.Count; i++) {
					try {
						manageWeather.Add(false);
						GUI.color = Color.white;
						GUILayout.BeginVertical("Box");
						manageWeather[i] = EditorGUILayout.Foldout(manageWeather[i], "" + globalWeatherList.weatherList[i].name);

						if (manageWeather[i]) {
							EditorUtility.SetDirty(globalWeatherList);       

							globalWeatherList.weatherList[i].name = EditorGUILayout.TextField("Weather name", globalWeatherList.weatherList[i].name, GUILayout.Width(position.width - 45));
							EditorUtility.SetDirty(globalWeatherList);   

							GUI.color = Color.red;                                                                                           
							if (GUILayout.Button("Delete Weather")) {
								globalWeatherList.weatherList.RemoveAt(i);
								EditorUtility.SetDirty(globalWeatherList);
							}
						}

						GUILayout.EndVertical();
					}
					catch { }

				}
				GUILayout.EndVertical();
			}
		}

		EditorGUILayout.EndScrollView();

		EditorGUILayout.EndVertical();

	}

	public string[] toolbarFloraStrings = new string[] { "Create Flora", "Flora list" };
	private Flora newFlora = new Flora();

	void FloraDatabase()
	{
		EditorGUILayout.BeginVertical("Box");

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarFloraStrings, GUILayout.Width(position.width - 18));
		GUILayout.EndHorizontal();

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

		GUILayout.Space(10);

		if (toolbarInt == 0) {
			GUI.color = Color.white;
			GUILayout.BeginVertical("Box", GUILayout.Width(position.width - 23));
			try {
				newFlora.name = EditorGUILayout.TextField("Flora name", newFlora.name, GUILayout.Width(position.width - 30));
				newFlora.sizeX = EditorGUILayout.IntSlider("Flora X axis size", newFlora.sizeX, 1, 10, GUILayout.Width(position.width - 30));
				newFlora.sizeY = EditorGUILayout.IntSlider("Flora Y axis size", newFlora.sizeY, 1, 10, GUILayout.Width(position.width - 30));
				EditorGUILayout.LabelField("Trees' folder path : " + newFlora.treePath, GUILayout.Width (position.width - 30));
				if (GUILayout.Button("Choose your tree", GUILayout.Width(position.width - 23))) {
					newFlora.treePath = EditorUtility.OpenFilePanel("Overwrite with asset", Application.dataPath + "/Resources/TilePalette", "asset");
				}
			} catch { }
			GUILayout.EndVertical();

			GUI.color = Color.green;
			GUI.SetNextControlName("Add Flora");
			if (GUILayout.Button("Add Flora", GUILayout.Width(position.width - 23))) {
				if (floraValid()) {
					addFlora (newFlora.clone());
					clearFlora ();
				} else {
					GUILayout.Label("The Flora is not valid");
				}
				GUI.FocusControl ("Add Flora"); 
			}
		} else if (toolbarInt == 1) {
			if (globalFloraList == null)
				globalFloraList = (FloraList)Resources.Load("Databases/FloraDatabase");
			if (globalFloraList.floraList.Count == 0) {
				GUI.color = Color.white;
				GUILayout.Label("There is no Flora in the Database!");
			} else {
				GUI.color = Color.green;
				GUI.SetNextControlName("Reload assets");
				if (GUILayout.Button("Reload assets", GUILayout.Width(position.width - 23))) {
					reloadFloraTreeAssets ();
					GUI.FocusControl ("Reload assets");
				}

				GUILayout.Space (10);

				GUILayout.BeginVertical();
				List<string> floraType = new List<string> ();

				for (int i = 0; i < globalFloraList.floraList.Count; i++) {
					string treeType = globalFloraList.floraList [i].treeType;
					if (!floraType.Contains(treeType)){
						floraType.Add (treeType);
					}
				}

				for (int i = 0; i < floraType.Count; i++) {
					try {
						manageFlora.Add(new List<bool>());
						manageFlora[i].Add(false);
						GUI.color = Color.cyan;
						GUILayout.BeginVertical("Box");
						manageFlora[i][0] = EditorGUILayout.Foldout(manageFlora[i][0], "" + floraType[i]);

						if (manageFlora[i][0]) {

							for (int j = 1; j <= globalFloraList.floraList.Count; j++) {
								try {
									if (globalFloraList.floraList[j-1].treeType.Equals(floraType[i])){
										manageFlora[i].Add(false);
										GUI.color = Color.white;
										GUILayout.BeginVertical("Box");
										manageFlora[i][j] = EditorGUILayout.Foldout(manageFlora[i][j], "" + globalFloraList.floraList[j-1].name);

										if (manageFlora[i][j]) {
											EditorUtility.SetDirty(globalFloraList); 

											globalFloraList.floraList[j-1].name =  EditorGUILayout.TextField("Flora name", globalFloraList.floraList[j-1].name , GUILayout.Width(position.width - 30));

											EditorGUILayout.LabelField("Tree type : " + globalFloraList.floraList[i].treeType, GUILayout.Width (position.width - 30));

											globalFloraList.floraList[j-1].sizeX = EditorGUILayout.IntSlider("Flora X axis size", globalFloraList.floraList[j-1].sizeX, 1, 10, GUILayout.Width(position.width - 30));

											globalFloraList.floraList[j-1].sizeY = EditorGUILayout.IntSlider("Flora Y axis size", globalFloraList.floraList[j-1].sizeY, 1, 10, GUILayout.Width(position.width - 30));

											globalFloraList.floraList[j-1].tree = (TileBase)EditorGUILayout.ObjectField("Tree tile", globalFloraList.floraList[j-1].tree, typeof(TileBase), false, GUILayout.Width(position.width - 30)); 

											EditorGUILayout.LabelField("Tree path : " + globalFloraList.floraList[j-1].treePath, GUILayout.Width (position.width - 30));

											EditorUtility.SetDirty(globalFloraList);

											GUI.color = Color.red;
											if (GUILayout.Button("Delete Flora"))
											{
												globalFloraList.floraList.RemoveAt(j-1);
												EditorUtility.SetDirty(globalFloraList);
											}
										}
										GUILayout.EndVertical();
									}
								}
								catch { }
							}
						}
						GUILayout.EndVertical();
					}
					catch { }

				}
				GUILayout.EndVertical();
			}
		}

		EditorGUILayout.EndScrollView();

		EditorGUILayout.EndVertical();
	}

	public int toolbarInt = 0;
	public string[] toolbarPlanetTypeStrings = new string[] { "Create Planet type", "Planet type list" };
	private PlanetType newPlanetType = new PlanetType();
	public int index = 0;

	void PlanetTypeDataBase()
	{
		EditorGUILayout.BeginVertical("Box");

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarPlanetTypeStrings, GUILayout.Width(position.width - 18));
		GUILayout.EndHorizontal();

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

		GUILayout.Space(10);

		if (toolbarInt == 0) {
			GUI.color = Color.white;
			GUILayout.BeginVertical("Box", GUILayout.Width(position.width - 23));
			try {
				newPlanetType.type = EditorGUILayout.TextField("Planet type", newPlanetType.type , GUILayout.Width(position.width - 33));
				newPlanetType.ruleTile = (TileBase)EditorGUILayout.ObjectField("Rule tile", newPlanetType.ruleTile, typeof(TileBase), false, GUILayout.Width(position.width - 33)); 
				newPlanetType.backgroundTile = (TileBase)EditorGUILayout.ObjectField("Background tile", newPlanetType.backgroundTile, typeof(TileBase), false, GUILayout.Width(position.width - 33)); 
				newPlanetType.chestSprite = (TileBase)EditorGUILayout.ObjectField("Chest sprite", newPlanetType.chestSprite, typeof(TileBase), false, GUILayout.Width(position.width - 33)); 
				newPlanetType.unbreakableTile = (TileBase)EditorGUILayout.ObjectField("Unbreakable tile", newPlanetType.unbreakableTile, typeof(TileBase), false, GUILayout.Width(position.width - 33)); 
				EditorGUILayout.LabelField("Trees' folder path : " + newPlanetType.treeAssetsPath, GUILayout.Width (position.width - 33));

				if (GUILayout.Button("Choose tree's assets folder path", GUILayout.Width(position.width - 33))) {
					newPlanetType.treeAssetsPath = EditorUtility.OpenFolderPanel("Double clic on the folder which contains the assets", Application.dataPath + "/Resources/TilePalette", "");
				}
			} catch { }
			GUILayout.EndVertical();

			GUI.color = Color.green;
			GUI.SetNextControlName("Add Planet Type");
			if (GUILayout.Button("Add Planet Type", GUILayout.Width(position.width - 23))) {
				if (planetTypeValid()) {
					addPlanetType (newPlanetType.clone());
					clearPlanetType ();
				} else {
					GUILayout.Label("Le type de la planete n'est pas valide");
				}
				GUI.FocusControl ("Add Planet Type"); 
			}
		} else if (toolbarInt == 1) {
			if (globalPlanetTypeList == null)
				globalPlanetTypeList = (PlanetTypeList)Resources.Load("Databases/PlanetTypeDatabase");
			if (globalPlanetTypeList.planetTypeList.Count == 0) {
				GUI.color = Color.white;
				GUILayout.Label("There is no Planet type in the Database!");
			} else {

				GUI.color = Color.green;
				GUI.SetNextControlName("Reload assets");
				if (GUILayout.Button("Reload assets", GUILayout.Width(position.width - 23))) {
					reloadPlanetTypeTreeAssets ();
					GUI.FocusControl ("Reload assets");
				}

				GUILayout.Space (10);

				GUILayout.BeginVertical();
			
				for (int i = 0; i < globalPlanetTypeList.planetTypeList.Count; i++) {
					try {
						managePlanetType.Add(false);
						GUI.color = Color.white;
						GUILayout.BeginVertical("Box");
						managePlanetType[i] = EditorGUILayout.Foldout(managePlanetType[i], "" + globalPlanetTypeList.planetTypeList[i].type);
						
						if (managePlanetType[i]) {

							EditorUtility.SetDirty(globalPlanetTypeList); 
			
							globalPlanetTypeList.planetTypeList[i].type =  EditorGUILayout.TextField("Planet Type", globalPlanetTypeList.planetTypeList[i].type , GUILayout.Width(position.width - 45));

							globalPlanetTypeList.planetTypeList[i].ruleTile = (TileBase)EditorGUILayout.ObjectField("Rule tile", globalPlanetTypeList.planetTypeList[i].ruleTile, typeof(TileBase), false, GUILayout.Width(position.width - 33)); 

							globalPlanetTypeList.planetTypeList[i].backgroundTile = (TileBase)EditorGUILayout.ObjectField("Background tile", globalPlanetTypeList.planetTypeList[i].backgroundTile, typeof(TileBase), false, GUILayout.Width(position.width - 33)); 

							globalPlanetTypeList.planetTypeList[i].chestSprite = (TileBase)EditorGUILayout.ObjectField("Chest sprite", globalPlanetTypeList.planetTypeList[i].chestSprite, typeof(TileBase), false, GUILayout.Width(position.width - 33)); 

							globalPlanetTypeList.planetTypeList[i].unbreakableTile = (TileBase)EditorGUILayout.ObjectField("Unbreakable tile", globalPlanetTypeList.planetTypeList[i].unbreakableTile, typeof(TileBase), false, GUILayout.Width(position.width - 33)); 

							EditorGUILayout.LabelField("Trees' folder path :" + globalPlanetTypeList.planetTypeList[i].treeAssetsPath, GUILayout.Width (position.width - 33));

							EditorUtility.SetDirty(globalPlanetTypeList);

							GUI.color = Color.red;
							if (GUILayout.Button("Delete Planet Type"))
							{
								globalPlanetTypeList.planetTypeList.RemoveAt(i);
								EditorUtility.SetDirty(globalPlanetTypeList);
							}
						}

						GUILayout.EndVertical();
					}
					catch { }

				}
				GUILayout.EndVertical();
			}
		}

		EditorGUILayout.EndScrollView();
	
		EditorGUILayout.EndVertical();
	}

	public string[] toolbarOreStrings = new string[] { "Create Ore", "Ore list" };
	private Ore newOre = new Ore();

	void OreDataBase() {
		EditorGUILayout.BeginVertical("Box");

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarOreStrings, GUILayout.Width(position.width - 18));
		GUILayout.EndHorizontal();

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

		GUILayout.Space(10);

		if (toolbarInt == 0) {
			GUI.color = Color.white;

			GUILayout.BeginVertical("Box", GUILayout.Width(position.width - 23));

			try {
				newOre.name = EditorGUILayout.TextField("Ore's name", newOre.name , GUILayout.Width(position.width - 33));
				newOre.tile = (TileBase)EditorGUILayout.ObjectField("Tile", newOre.tile, typeof(TileBase), false, GUILayout.Width(position.width - 33));
				//newOre.tileSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", newOre.tileSprite, typeof(Sprite), false, GUILayout.Width(position.width - 33));
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Range of Ore's generation :",GUILayout.Width(position.width - 33));
				newOre.galaxyRange.x = EditorGUILayout.IntSlider("From Galaxy n°",newOre.galaxyRange.x, 0, newOre.galaxyRange.y, GUILayout.Width(position.width - 33));
				newOre.galaxyRange.y = EditorGUILayout.IntSlider("To Galaxy n°",newOre.galaxyRange.y, newOre.galaxyRange.x, 50, GUILayout.Width(position.width - 33));
				//newOre.depositQuantity = EditorGUILayout.Slider("Quantity",newOre.depositQuantity, 0, 1);
				//newOre.depositWidth = EditorGUILayout.Slider("To Galaxy n°",newOre.depositWidth, 5, 100);
			}
			catch { }
			GUILayout.EndVertical();

			GUI.color = Color.green;
			GUI.SetNextControlName("Add Ore");
			if (GUILayout.Button("Add Ore", GUILayout.Width(position.width - 23))) {
				if (oreValid()) {
					addOre (newOre.clone());
					clearOre ();
				} else {
					GUILayout.Label("Le type de la planete n'est pas valide");
				}
				GUI.FocusControl ("Add Ore"); 
			}
		} else if (toolbarInt == 1) {
			if (globalOreList == null)
				globalOreList = (OreList)Resources.Load("Databases/OreDatabase");
			if (globalOreList.oreList.Count == 0) {
				GUI.color = Color.white;
				GUILayout.Label("There is no Ore in the Database!");
			} else {
				GUILayout.BeginVertical();

				for (int i = 0; i < globalOreList.oreList.Count; i++) {
					try {
						manageOre.Add(false);
						GUI.color = Color.white;
						GUILayout.BeginVertical("Box");
						manageOre[i] = EditorGUILayout.Foldout(manageOre[i], "" + globalOreList.oreList[i].name);

						if (manageOre[i]) {
							EditorUtility.SetDirty(globalOreList);       

							globalOreList.oreList[i].name = EditorGUILayout.TextField("Ore's name", globalOreList.oreList[i].name , GUILayout.Width(position.width - 45));
							globalOreList.oreList[i].tile = (TileBase)EditorGUILayout.ObjectField("Tile", globalOreList.oreList[i].tile, typeof(TileBase), false, GUILayout.Width(position.width - 45));

							GUILayout.Space(10);
							EditorGUILayout.LabelField("Range of Ore's generation :",GUILayout.Width(position.width - 45));
							globalOreList.oreList[i].galaxyRange.x = EditorGUILayout.IntSlider("From Galaxy n°",globalOreList.oreList[i].galaxyRange.x, 0, globalOreList.oreList[i].galaxyRange.y, GUILayout.Width(position.width - 45));
							globalOreList.oreList[i].galaxyRange.y = EditorGUILayout.IntSlider("To Galaxy n°",globalOreList.oreList[i].galaxyRange.y, globalOreList.oreList[i].galaxyRange.x, 50, GUILayout.Width(position.width - 45));

							EditorUtility.SetDirty(globalOreList);   

							GUI.color = Color.red;                                                                                           
							if (GUILayout.Button("Delete Ore")) {
								globalOreList.oreList.RemoveAt(i);
								EditorUtility.SetDirty(globalOreList);
							}
						}

						GUILayout.EndVertical();
					}
					catch { }

				}
				GUILayout.EndVertical();
			}
		}

		EditorGUILayout.EndScrollView();

		EditorGUILayout.EndVertical();
	}

	void GalaxyDataBase() {
		EditorGUILayout.BeginVertical("Box");

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

		if (globalGalaxyList == null)
			globalGalaxyList = (GalaxyList)Resources.Load("Databases/GalaxyDatabase");

		GUI.color = Color.green;
		GUI.SetNextControlName("Regenerate Galaxy");
		if (GUILayout.Button("Regenerate Galaxy", GUILayout.Width(position.width - 23))) {
			clearGalaxy ();
			generateGalaxy (50);
			GUI.FocusControl ("Regenerate Galaxy");
		}
		GUILayout.Space(10);

		if (globalGalaxyList.galaxyList.Count == 0) {
			GUI.color = Color.white;
			GUILayout.Label("There is no Galaxy in the Database!");
		} else {
			GUILayout.BeginVertical();

			for (int i = 0; i < globalGalaxyList.galaxyList.Count; i++) {
				try {
					manageGalaxy.Add(new List<List<bool>>());
					manageGalaxy[i].Add(new List<bool>());
					manageGalaxy[i][0].Add(false);
					GUI.color = Color.yellow;
					GUILayout.BeginVertical("Box");
					manageGalaxy[i][0][0] = EditorGUILayout.Foldout(manageGalaxy[i][0][0], "" + globalGalaxyList.galaxyList[i].name);

					if (manageGalaxy[i][0][0]) {
						EditorUtility.SetDirty(globalGalaxyList);  

						globalGalaxyList.galaxyList [i].name = EditorGUILayout.TextField ("Galaxy name", globalGalaxyList.galaxyList [i].name, GUILayout.Width (position.width - 45));

						for (int j = 1; j <= globalGalaxyList.galaxyList[i].stellarSystemList.Count; j++) {
							manageGalaxy[i][j].Add(false);

							GUI.color = Color.cyan;
							GUILayout.BeginVertical("Box");
							manageGalaxy[i][j][0] = EditorGUILayout.Foldout(manageGalaxy[i][j][0], "" + globalGalaxyList.galaxyList[i].stellarSystemList[j-1].name);

							if (manageGalaxy[i][j][0]) {
								EditorUtility.SetDirty(globalGalaxyList);  

								globalGalaxyList.galaxyList[i].stellarSystemList[j-1].name = EditorGUILayout.TextField ("System name",  globalGalaxyList.galaxyList[i].stellarSystemList[j-1].name, GUILayout.Width (position.width - 45));

								for (int k = 1; k <= globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList.Count; k++) {
									manageGalaxy[i][j].Add(false);

									GUI.color = Color.white;
									GUILayout.BeginVertical("Box");
									manageGalaxy[i][j][k] = EditorGUILayout.Foldout(manageGalaxy[i][j][k], "" + globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].name);

									if (manageGalaxy[i][j][k]) {
										EditorUtility.SetDirty(globalGalaxyList);  

										// Planet name
										globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].name = EditorGUILayout.TextField ("Planet name", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].name, GUILayout.Width (position.width - 45));

										// Planet seed
										globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].seed = EditorGUILayout.Slider ("Seed", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].seed, -100000f, 100000f, GUILayout.Width (position.width - 45));

										// Galaxy
										EditorGUILayout.LabelField("Galaxy : " + globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].galaxy, GUILayout.Width (position.width - 45));

										// System
										EditorGUILayout.LabelField("System : " + globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].stellarSystem, GUILayout.Width (position.width - 45));

										// Planet ID
										EditorGUILayout.LabelField("Planet ID : " + globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].planetID, GUILayout.Width (position.width - 45));

										// Planet type
										EditorGUILayout.LabelField("Planet Type : " + globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].planetType.type, GUILayout.Width (position.width - 45));

										// Planet tree folder path
										EditorGUILayout.LabelField("Trees' folder path : " + globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].planetType.treeAssetsPath, GUILayout.Width (position.width - 45));

										// Horizontal size
										EditorGUILayout.LabelField("Horizontal size : " + globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].horizontalSize, GUILayout.Width (position.width - 45));

										// Vertical size
										EditorGUILayout.LabelField("Vertical size : " + globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].verticalSize, GUILayout.Width (position.width - 45));

										// Smoothness
										globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].smoothness = EditorGUILayout.Slider ("Smoothness", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].smoothness, 40f, 150f, GUILayout.Width (position.width - 45));

										// Height multiplier
										globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].heightMultiplier = EditorGUILayout.IntSlider ("Height multiplier", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].heightMultiplier, 30, 80, GUILayout.Width (position.width - 45));

										// Planet seed
										globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].seedCave = EditorGUILayout.Slider ("Seed", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].seedCave, -100000f, 100000f, GUILayout.Width (position.width - 45));

										// Cave width
										globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].caveWidth = EditorGUILayout.Slider ("Cave width", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].caveWidth, 15f, 60f, GUILayout.Width (position.width - 45));

										// Cave quantity
										globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].caveQuantity = EditorGUILayout.Slider ("Cave quantity", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].caveQuantity, 0.320f, 0.700f, GUILayout.Width (position.width - 45));

										// Gravity
										globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].gravity = EditorGUILayout.Slider ("Gravity", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].gravity, -9.81f, 19.62f, GUILayout.Width (position.width - 45));

										// Day speed multiplier
										globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].daySpeedMultiplier = EditorGUILayout.Slider ("Day speed multiplier", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].daySpeedMultiplier, 24f, 400f, GUILayout.Width (position.width - 45));

										// Number of chest
										globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].numberOfChest = EditorGUILayout.IntSlider ("Number of chest", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].numberOfChest, 5, 20, GUILayout.Width (position.width - 45));

										// Atmosphere
										globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].atmosphere = atmosphereTab[EditorGUILayout.Popup ("Atmosphere", getAtmosphereIndex (globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].atmosphere), atmosphereTab, GUILayout.Width (position.width - 45))];

										// Color filter
										globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].filter = EditorGUILayout.ColorField("Filter", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].filter);

										// Ore list
										EditorGUILayout.LabelField("Ore list (" + globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].oreList.Count + ") included on this planet : ", GUILayout.Width (position.width - 45));
										for (int l = 0; l < globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].oreList.Count; l++) {
											GUILayout.BeginVertical("Box");
											EditorGUILayout.LabelField("Name : " + globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].oreList[l].name, GUILayout.Width (position.width - 45));
											EditorGUILayout.LabelField("Seed deposit : " + globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].oreList[l].seedDeposit, GUILayout.Width (position.width - 45));
											globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].oreList[l].area = EditorGUILayout.IntSlider("Area of generation", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].oreList[l].area, 1, 3, GUILayout.Width(position.width - 45));
											globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].oreList[l].depositRarity = EditorGUILayout.Slider ("Deposit rarity", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].oreList[l].depositRarity, 1f, 1.9f, GUILayout.Width (position.width - 45));
											globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].oreList[l].depositWidth = EditorGUILayout.Slider ("Deposit width", globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1].oreList[l].depositWidth, 15f, 30f, GUILayout.Width (position.width - 45));
											GUILayout.EndVertical();
											GUILayout.Space(10);
										}

										EditorUtility.SetDirty(globalGalaxyList);

										GUI.color = Color.white;
										if (GUILayout.Button("Load this planet")) {
											if ("Planet".Equals(SceneManager.GetActiveScene().name)){
												Debug.Log("Sauvegarde de la planète " + GameObject.FindGameObjectWithTag("grid").GetComponent<PlanetGenerator>().planet.name + "...");
												savePlanet(GameObject.FindGameObjectWithTag("grid").GetComponent<PlanetGenerator>().planet);
												Debug.Log("La planète " + GameObject.FindGameObjectWithTag("grid").GetComponent<PlanetGenerator>().planet.name + " a été sauvegardée avec succès !");
												SceneManager.LoadScene("Hub");
											}

											Scenes.Load("Planet", loadFromJson (globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList[k-1]));
										}

										GUI.color = Color.red;
										if (GUILayout.Button("Delete Planet")) {
											globalGalaxyList.galaxyList[i].stellarSystemList[j-1].planetList.RemoveAt(k-1);
											EditorUtility.SetDirty(globalGalaxyList);
										}
									}

									GUILayout.EndVertical();
								}

								EditorUtility.SetDirty(globalGalaxyList); 
								GUI.color = Color.red;                                                                            
								if (GUILayout.Button("Delete System")) {
									globalGalaxyList.galaxyList[i].stellarSystemList.RemoveAt(j-1);
									EditorUtility.SetDirty(globalGalaxyList);
								}
							}

							GUILayout.EndVertical();
						}

						EditorUtility.SetDirty(globalGalaxyList); 
						GUI.color = Color.red;                                                                            
						if (GUILayout.Button("Delete Galaxy")) {
							globalGalaxyList.galaxyList.RemoveAt(i);
							EditorUtility.SetDirty(globalGalaxyList);
						}
					}

					GUILayout.EndVertical();
				}
				catch { }

			}
			GUILayout.EndVertical();
		}

		EditorGUILayout.EndScrollView();

		EditorGUILayout.EndVertical();
	}

	void saveToJson(Planet planet){
		PlanetToJson ptj = new PlanetToJson ();
		ptj = planet.formatPlanetToJson ();
		string dataAsJson = JsonUtility.ToJson (ptj);
		string filePath = Application.dataPath + "/Scenes/PlanetSaves/planetDataG" + planet.galaxy + "S" + planet.stellarSystem + "P" + planet.planetID + ".json";
		File.WriteAllText (filePath, dataAsJson);
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

	void savePlanet(Planet planetToSave){
		if (globalGalaxyList == null)
			globalGalaxyList = (GalaxyList)Resources.Load("Databases/GalaxyDatabase");
		
		if (globalGalaxyList.galaxyList [planetToSave.galaxy].stellarSystemList [planetToSave.stellarSystem].planetList [planetToSave.planetID].savedMapHeight.GetLength (0) == 0) 
			globalGalaxyList.galaxyList [planetToSave.galaxy].stellarSystemList [planetToSave.stellarSystem].planetList [planetToSave.planetID].savedMapHeight = new int[planetToSave.savedMapHeight.GetLength (0)];
		
		planetToSave.savedMapHeight.CopyTo(globalGalaxyList.galaxyList [planetToSave.galaxy].stellarSystemList [planetToSave.stellarSystem].planetList [planetToSave.planetID].savedMapHeight,0);

		int maximalHeight = GameObject.FindGameObjectWithTag ("grid").GetComponent<PlanetGenerator> ().maximalHeight;

		if (globalGalaxyList.galaxyList [planetToSave.galaxy].stellarSystemList [planetToSave.stellarSystem].planetList [planetToSave.planetID].savedMapMatrix == null) 
			globalGalaxyList.galaxyList [planetToSave.galaxy].stellarSystemList [planetToSave.stellarSystem].planetList [planetToSave.planetID].savedMapMatrix = new int[planetToSave.savedMapMatrix.GetLength(0),maximalHeight];

		for (int x = 0; x < planetToSave.savedMapMatrix.GetLength(0); x++)
			for (int y = 0; y < planetToSave.savedMapMatrix.GetLength(1); y++)
				globalGalaxyList.galaxyList [planetToSave.galaxy].stellarSystemList [planetToSave.stellarSystem].planetList [planetToSave.planetID].savedMapMatrix[x, y] = planetToSave.savedMapMatrix[x, y];

		globalGalaxyList.galaxyList [planetToSave.galaxy].stellarSystemList [planetToSave.stellarSystem].planetList [planetToSave.planetID].tilesType = planetToSave.tilesType;

		globalGalaxyList.galaxyList [planetToSave.galaxy].stellarSystemList [planetToSave.stellarSystem].planetList [planetToSave.planetID].playerLastPosition = new Vector3 (0, 0, 0);

		globalGalaxyList.galaxyList [planetToSave.galaxy].stellarSystemList [planetToSave.stellarSystem].planetList [planetToSave.planetID].playerLastPosition = planetToSave.playerLastPosition;
	
		saveToJson (globalGalaxyList.galaxyList [planetToSave.galaxy].stellarSystemList [planetToSave.stellarSystem].planetList [planetToSave.planetID]);
	}

	void clearGalaxy(){
		if (globalGalaxyList == null)
			globalGalaxyList = (GalaxyList)Resources.Load("Databases/GalaxyDatabase");
		
		EditorUtility.SetDirty (globalGalaxyList);
		globalGalaxyList.galaxyList.Clear ();
		EditorUtility.SetDirty(globalGalaxyList);
	}

	void generateGalaxy(int nbGalaxyToGenerate){
		if (globalGalaxyList == null)
			globalGalaxyList = (GalaxyList)Resources.Load("Databases/GalaxyDatabase");
		
		string filePath = Application.dataPath + "/Scenes/PlanetSaves";

		string[] filenames = Directory.GetFiles(filePath, "planetDataG*S*P*.json", SearchOption.TopDirectoryOnly);
		foreach (string fName in filenames)
			File.Delete(fName);
		
		for (int i = 0; i <nbGalaxyToGenerate; i++){
			Galaxy newGalaxy = new Galaxy();

			newGalaxy.name = PlanetNameGenerator.GenerateName ();
			newGalaxy.stellarSystemList = new List<StellarSystem> ();

			for (int j = 0; j < Random.Range (8, 13); j++) {
				newGalaxy.stellarSystemList.Add (new StellarSystem (PlanetNameGenerator.GenerateName (), new List<Planet> ()));
				for (int k = 0; k < Random.Range (6, 11); k++) {
					newGalaxy.stellarSystemList[j].planetList.Add (generatePlanet (i, j, k));
				}
			
			}
			EditorUtility.SetDirty (globalGalaxyList);
			globalGalaxyList.galaxyList.Add(newGalaxy);
			EditorUtility.SetDirty(globalGalaxyList);
		}

		int nbGalaxies = 0;
		int nbSystems = 0;
		int nbPlanets = 0;

		nbGalaxies = globalGalaxyList.galaxyList.Count;

		foreach (Galaxy g in globalGalaxyList.galaxyList) {
			nbSystems += g.stellarSystemList.Count;
			foreach (StellarSystem s in g.stellarSystemList)
				nbPlanets += s.planetList.Count;
		}

		foreach (Galaxy g in globalGalaxyList.galaxyList)
			nbSystems = nbSystems + g.stellarSystemList.Count;

		Debug.Log ("Result of the generation : ");
		Debug.Log ("  - Galaxies : " + nbGalaxies);
		Debug.Log ("  - Systems : " + nbSystems);
		Debug.Log ("  - Planets : " + nbPlanets);

	}

	private string[] atmosphereTab = new string[]{ "Normal", "Toxic", "Unbreathable", "Irradiated" };
	private string[] filterList = new string[] {
		"#FFFFFF",
		"#FFA1A1",
		"#FFF39C",
		"#A1FF96",
		"#90FFFA",
		"#9589FF",
		"#FF7E7E"
	};

	Planet generatePlanet(int galaxy, int stellarSystem, int planetID){
		if (globalPlanetTypeList == null)
			globalPlanetTypeList = (PlanetTypeList)Resources.Load("Databases/PlanetTypeDatabase");

		if (globalOreList == null)
			globalOreList = (OreList)Resources.Load("Databases/OreDatabase");

		int rdmIntValue;

		Planet newPlanet = new Planet();

		// Planet name
		newPlanet.name = PlanetNameGenerator.GenerateName ();

		// Planet seed
		newPlanet.seed = Random.Range(-100000f, 100000f);

		// Galaxy
		newPlanet.galaxy = galaxy;

		// System
		newPlanet.stellarSystem = stellarSystem;

		// Planet ID
		newPlanet.planetID = planetID;

		// Planet type
		rdmIntValue = Random.Range (0, globalPlanetTypeList.planetTypeList.Count);
		newPlanet.planetType = globalPlanetTypeList.planetTypeList [rdmIntValue].clone ();

		// Horizontal size
		newPlanet.horizontalSize = Random.Range (400, 12801);

		// Vertical size
		newPlanet.verticalSize = Random.Range (50, 301);

		// Smoothness
		newPlanet.smoothness = Random.Range (40f, 150f);

		// Height multiplier
		newPlanet.heightMultiplier = Random.Range (30, 81);

		// Cave width
		newPlanet.seedCave = Random.Range(-100000f, 100000f);

		// Cave width
		newPlanet.caveWidth = Random.Range (15f, 60f);

		// Cave quantity
		newPlanet.caveQuantity = Random.Range (0.320f, 0.700f);

		// Gravity
		if (Random.Range (0, 9) >= 7)
			newPlanet.gravity = Random.Range (-9.81f, 19.62f);
		
		// Day speed multiplier
		newPlanet.daySpeedMultiplier = Random.Range (24f, 400f);

		// Number of chest
		newPlanet.numberOfChest = Random.Range (5, 21);

		// Atmosphere
		if (atmosphereTab.Length != 0) {
			rdmIntValue = Random.Range (0, atmosphereTab.Length);
			newPlanet.atmosphere = (string)atmosphereTab [rdmIntValue].Clone ();
		} else 
			newPlanet.atmosphere = "Normal";
		
		// Color filter
		if (filterList.Length != 0) {
			if (Random.Range (0, 9) >= 6) {
				rdmIntValue = Random.Range (0, filterList.Length);
				ColorUtility.TryParseHtmlString ((string)filterList [rdmIntValue].Clone (), out newPlanet.filter);
			} else
				ColorUtility.TryParseHtmlString ("#FFFFFF", out newPlanet.filter);
		} else
			ColorUtility.TryParseHtmlString ("#FFFFFF", out newPlanet.filter);

		foreach (Ore ore in globalOreList.oreList)
			if (ore.galaxyRange.x <= newPlanet.galaxy && ore.galaxyRange.y >= newPlanet.galaxy) {
				Ore oreToAdd = ore.clone();

				if (Random.Range(0,20)>=18)
					oreToAdd.depositRarity = Random.Range (1.5f, 1.9f);
				else 
					oreToAdd.depositRarity = Random.Range (1.0f, 1.5f);
				
				oreToAdd.area = Random.Range (1, 4);
				oreToAdd.depositWidth = Random.Range (15f, 30f); 
				oreToAdd.seedDeposit = Random.Range(-100000f, 100000f);
				newPlanet.oreList.Add (oreToAdd.clone ());
			}
			
		return newPlanet;
	}

	void reloadPlanetTypeTreeAssets(){
		if (globalPlanetTypeList == null)
			globalPlanetTypeList = (PlanetTypeList)Resources.Load("Databases/PlanetTypeDatabase");

		for (int i = 0; i < globalPlanetTypeList.planetTypeList.Count;i++){
			if (!"".Equals (globalPlanetTypeList.planetTypeList[i].treeAssetsPath) && Directory.Exists (globalPlanetTypeList.planetTypeList[i].treeAssetsPath)) {
				if (globalFloraList == null)
					globalFloraList = (FloraList)Resources.Load("Databases/FloraDatabase");

				globalPlanetTypeList.planetTypeList[i].treeList = new List<Flora> ();
				string pathInResources = globalPlanetTypeList.planetTypeList[i].treeAssetsPath.Split(new string[] {"Resources/"}, System.StringSplitOptions.None)[1];
				string floraTypeSelected = pathInResources.Split(new string[] {"/"}, System.StringSplitOptions.None)[1];

				for (int j = 0; j < globalFloraList.floraList.Count; j++) {
					Flora flora = globalFloraList.floraList [j];
					if (flora.treeType.Equals (floraTypeSelected)) {
						//globalFloraList.floraList [j].treePath = globalPlanetTypeList.planetTypeList [i].treeAssetsPath + "/" + globalFloraList.floraList [j].name;
						globalPlanetTypeList.planetTypeList[i].treeList.Add (flora);
					}
				}
			}
		}
	}

	void reloadFloraTreeAssets(){
		if (globalFloraList == null)
			globalFloraList = (FloraList)Resources.Load ("Databases/FloraDatabase");

		for (int i = 0; i < globalFloraList.floraList.Count; i++) {
			if (!"".Equals (globalFloraList.floraList[i].treePath) && File.Exists (globalFloraList.floraList[i].treePath)) {
				newFlora.tree = Resources.Load<TileBase> (globalFloraList.floraList[i].treePath);
			}

			/*
			if (globalFloraList.floraList [i].sizeX == 3 && globalFloraList.floraList [i].sizeY == 7) {
				globalFloraList.floraList [i].sizeX = 4;
				globalFloraList.floraList [i].sizeY = 8;
			} else if (globalFloraList.floraList [i].sizeX == 3 && globalFloraList.floraList [i].sizeY == 3) {
				globalFloraList.floraList [i].sizeX = 4;
				globalFloraList.floraList [i].sizeY = 4;

			} else if (globalFloraList.floraList [i].sizeX == 7 && globalFloraList.floraList [i].sizeY == 7) {
				globalFloraList.floraList [i].sizeX = 8;
				globalFloraList.floraList [i].sizeY = 8;
			}*/

		}
	}

	void addPlanet(Planet newPlanet) {
		if (globalPlanetList == null)
			globalPlanetList = (PlanetList)Resources.Load("Databases/PlanetDatabase");
		
		EditorUtility.SetDirty (globalPlanetList);
		globalPlanetList.planetList.Add(newPlanet);
		EditorUtility.SetDirty(globalPlanetList);
	}

	private string HexConverter(Color color) {
		return "#" + ColorUtility.ToHtmlStringRGB(color);
	}


	bool oreValid() {
		if (globalOreList == null)
			globalOreList = (OreList)Resources.Load("Databases/OreDatabase");
		
		if ("".Equals (newOre.name.Trim ()))
			return false;

		foreach (Ore ore in globalOreList.oreList) {
			if (ore.name.ToLower().Equals (newOre.name.ToLower ())) {
				return false;
			}
		}

		if (newOre.galaxyRange.x < 0 || newOre.galaxyRange.y < 0 || newOre.galaxyRange.x > newOre.galaxyRange.y)
			return false;

		if (newOre.tile == null)
			return false;

		return true;
	}

	void clearOre() {
		newOre.name = "";
		newOre.tile = null;
		newOre.galaxyRange = new Vector2Int(0,0);
	}

	void addOre(Ore newOre) {
		if (globalOreList == null)
			globalOreList = (OreList)Resources.Load("Databases/OreDatabase");
		
		EditorUtility.SetDirty (globalOreList);
		globalOreList.oreList.Add(newOre);
		EditorUtility.SetDirty(globalOreList);
	}

	bool floraValid() {
		if (globalFloraList == null)
			globalFloraList = (FloraList)Resources.Load("Databases/FloraDatabase");

		if (newFlora.sizeX <= 0 || newFlora.sizeY <= 0)
			return false;

		if ("".Equals (newFlora.treeType))
			return false;

		if (!BuildTreeAsset ())
			return false;

		return true;
	}

	bool BuildTreeAsset(){
		bool res = false;
		if (!"".Equals (newFlora.treePath) && File.Exists (newFlora.treePath)) {
			string pathInResources = newFlora.treePath.Split (new string[] { "Resources/" }, System.StringSplitOptions.None) [1];
			newFlora.treeType = pathInResources.Split (new string[] { "/" }, System.StringSplitOptions.None) [1];
			newFlora.treePath = newFlora.treePath.Split(new string[] { "." }, System.StringSplitOptions.None) [0];
			pathInResources = pathInResources.Split (new string[] { "." }, System.StringSplitOptions.None) [0];

			if ("".Equals (newFlora.name.Trim ()))
				newFlora.name = pathInResources.Split (new string[] { "/" }, System.StringSplitOptions.None) [2];
					
			newFlora.tree = Resources.Load<TileBase> (pathInResources);

			if (newFlora.tree != null)
				res = true;
		}
		return res;
	}
		
	void clearFlora() {
		newFlora.name = "";
		newFlora.treeType = "";
		newFlora.sizeX = 0;
		newFlora.sizeY = 0;
		newFlora.tree = null;
	}

	void addFlora(Flora newFlora) {
		if (globalFloraList == null)
			globalFloraList = (FloraList)Resources.Load("Databases/FloraDatabase");

		EditorUtility.SetDirty (globalFloraList);
		globalFloraList.floraList.Add(newFlora);
		EditorUtility.SetDirty(globalFloraList);
	}

	bool planetTypeValid() {
		if (globalPlanetTypeList == null)
			globalPlanetTypeList = (PlanetTypeList)Resources.Load("Databases/PlanetTypeDatabase");
		
		if ("".Equals (newPlanetType.type.Trim ()))
			return false;

		foreach (PlanetType planetType in globalPlanetTypeList.planetTypeList) {
			if (planetType.type.ToLower().Equals (newPlanetType.type.ToLower ())) {
				return false;
			}
		}
		
		if (newPlanetType.ruleTile == null || newPlanetType.chestSprite == null || newPlanetType.backgroundTile == null || newPlanetType.unbreakableTile == null)
			return false;
		
		if ("".Equals (newPlanetType.treeAssetsPath))
			return false;

		if (!BuildTreeAssetsList ())
			return false;
		
		return true;
	}

	bool BuildTreeAssetsList() {
		bool res = false;
		if (!"".Equals (newPlanetType.treeAssetsPath) && Directory.Exists (newPlanetType.treeAssetsPath)) {
			if (globalFloraList == null)
				globalFloraList = (FloraList)Resources.Load("Databases/FloraDatabase");
			
			newPlanetType.treeList = new List<Flora> ();
			string pathInResources = newPlanetType.treeAssetsPath.Split(new string[] {"Resources/"}, System.StringSplitOptions.None)[1];
			string floraTypeSelected = pathInResources.Split(new string[] {"/"}, System.StringSplitOptions.None)[1];

			for (int i = 0; i < globalFloraList.floraList.Count; i++) {
				Flora flora = globalFloraList.floraList [i];
				if (flora.treeType.Equals (floraTypeSelected)) {
					newPlanetType.treeList.Add (flora);
				}
			}

			if (newPlanetType.treeList.Count > 0)
				res = true;
		}
		return res;
	}

	void clearPlanetType() {
		newPlanetType.type = "";
		newPlanetType.ruleTile = null;
		newPlanetType.chestSprite = null;
		newPlanetType.backgroundTile = null;
		newPlanetType.unbreakableTile = null;
		newPlanetType.treeList = new List<Flora> ();
		newPlanetType.treeAssetsPath = "";
		/*
		for (int i = 0; i < newPlanetType.weatherList.Count; i++)
			newPlanetType.isOnPlanet [i] = false;*/
	}

	void addPlanetType(PlanetType newPlanetType) {
		if (globalPlanetTypeList == null)
			globalPlanetTypeList = (PlanetTypeList)Resources.Load("Databases/PlanetTypeDatabase");
		
		EditorUtility.SetDirty (globalPlanetTypeList);
		globalPlanetTypeList.planetTypeList.Add(newPlanetType);
		EditorUtility.SetDirty(globalPlanetTypeList);
	}

	int getPlanetTypeIndex(string type){
		if (globalPlanetTypeList == null)
			globalPlanetTypeList = (PlanetTypeList)Resources.Load("Databases/PlanetTypeDatabase");

		int num = 0;
		foreach (PlanetType planetType in globalPlanetTypeList.planetTypeList) {
			if (planetType.type.Equals (type)) {
				break;
			}
			num++;
		}
		return num;
	}

	int getAtmosphereIndex(string word){
		int num = 0;
		foreach (string element in atmosphereTab) {
			if (element.Equals (word)) {
				break;
			}
			num++;
		}
		return num;
	}

	int getFilterListIndex(string word){
		int num = 0;
		foreach (string element in filterList) {
			if (element.Equals (word)) {
				break;
			}
			num++;
		}
		return num;
	}

	void addWeather(string weatherTextField) {
		if (globalWeatherList == null)
			globalWeatherList = (WeatherList)Resources.Load("Databases/WeatherDatabase");

		EditorUtility.SetDirty (globalWeatherList);
		Weather newWeather = new Weather();
		newWeather.name = weatherTextField;
		globalWeatherList.weatherList.Add(newWeather);
		EditorUtility.SetDirty(globalWeatherList);
	}

	bool weatherAlreadyCreated(string weatherTextField){
		if (globalWeatherList == null)
			globalWeatherList = (WeatherList)Resources.Load("Databases/WeatherDatabase");
		
		bool result = false;
		foreach (Weather weather in globalWeatherList.weatherList) {
			if (weather.name.ToLower().Equals (weatherTextField.ToLower ())) {
				result = true;
				break;
			}
		}
		return result;
	}
}
