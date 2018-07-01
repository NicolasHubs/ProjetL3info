using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ButtonAction : MonoBehaviour {
    public Text textJouer;
    public Text textQuitter;
    public GameObject parent;
    private PlanetGenerator pg;

    // Use this for initialization
    void Start () {
        if(SceneManager.GetActiveScene().name.Equals("Planet"))
            pg = GameObject.FindGameObjectWithTag("grid").GetComponent<PlanetGenerator>();

    }
    
    public void Jouer()
    {
        if(SceneManager.GetActiveScene().name.Equals("MainMenu"))
            SceneManager.LoadScene("Hub");
        else
        {
            parent.SetActive(false);
        }

    }

    public void Quitter()
    {
        if (SceneManager.GetActiveScene().name.Equals("MainMenu") || SceneManager.GetActiveScene().name.Equals("Hub"))
            Application.Quit();
        else if (SceneManager.GetActiveScene().name.Equals("Planet"))
        {
            pg.savePlanet(pg.planet);
            Scenes.Load("Hub");
        }
    }
}
