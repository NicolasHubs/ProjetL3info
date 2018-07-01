using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	public GameObject MainMenu, SystemMenu, PlanetMenu;

	public Animator mainMenuAnim,systemMenuAnim, planetMenuAnim;
	public GameObject dashboard;
	public GameObject sphere;

	public GameObject planetTextName;
	public GameObject galaxyGroupName;
	public GameObject systemGroupName;

	public GameObject planetInfo;

	[HideInInspector]
	public GameObject galaxyTextName;
	[HideInInspector]
	public GameObject systemTextName;

	[HideInInspector]
	public Text planetTextFieldName;
	[HideInInspector]
	public Text galaxyTextFieldName;
	[HideInInspector]
	public Text systemTextFieldName;

	void Awake() {
		galaxyTextName = galaxyGroupName.transform.GetChild (1).gameObject;
		systemTextName = systemGroupName.transform.GetChild (1).gameObject;

		planetTextFieldName = planetTextName.GetComponent<Text> ();
		galaxyTextFieldName = galaxyTextName.GetComponent<Text> ();
		systemTextFieldName = systemTextName.GetComponent<Text> ();
	}

	// Use this for initialization
	void Start() {
		mainMenuAnim = MainMenu.GetComponent<Animator>();
		systemMenuAnim = SystemMenu.GetComponent<Animator>();
		planetMenuAnim = PlanetMenu.GetComponent<Animator>();
		/*
		planetTextFieldName = planetTextName.GetComponent<Text> ();
		galaxyTextFieldName = galaxyTextName.GetComponent<Text> ();
		systemTextFieldName = systemTextName.GetComponent<Text> ();
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// fonction des boutons du menu
	public void OnGalaxyBackButtonClick()
	{
		dashboard.SetActive(!dashboard.activeSelf);
	}

    // fonction des boutons du menu
    public void OnSystemBackButtonClick()
    {
		if (galaxyTextName.activeSelf) {
			galaxyTextName.SetActive (false);
			galaxyGroupName.SetActive (false);
		}
        systemMenuAnim.SetBool("isMovingIn", false);
        mainMenuAnim.SetBool("isMovingOut", false);
    }

	public void OnGalaxyButtonClick(){
        mainMenuAnim.SetBool("isMovingOut", true);
        systemMenuAnim.SetBool("isMovingIn", true);
    }

	public void OnSystemButtonClick(){
		systemMenuAnim.SetBool("isMovingOut",true);
        planetMenuAnim.SetBool("isMovingIn", true);
    }

    public void OnPlanetButtonClick(){
       // Selection de Planete, à voir.
    }

    public void OnPlanetBackButtonClick()
    {
		if (systemTextName.activeSelf) {
			systemTextName.SetActive (false);
			systemGroupName.SetActive (false);
		}

		if (planetTextName.activeSelf)
			planetTextName.SetActive (false);
		
		if (sphere.activeSelf)
			sphere.SetActive (false);

		if (planetInfo.activeSelf) 
			planetInfo.SetActive (false);
		
        planetMenuAnim.SetBool("isMovingIn", false);
        systemMenuAnim.SetBool("isMovingOut", false);
    }
}