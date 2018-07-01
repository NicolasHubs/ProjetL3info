using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayExp : EventTrigger
{
    Text expText;
    Text playerLevelText;
    private PlayerInventory pi;

    void Start()
    {
        pi = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        expText = GameObject.Find("ExpProgression").GetComponent<Text>();
        playerLevelText = GameObject.Find("PlayerLevel").GetComponent<Text>();
    }

    void Update()
    {
        playerLevelText.text = "Level " + pi.currentLevel;
    }

    public void DisplayInfo()
    {
        expText.text = "" + Mathf.RoundToInt(pi.currentExp) + " / " + Mathf.RoundToInt(pi.expToLevelUp) + "   -   " + Mathf.RoundToInt(pi.percentageExp * 100) + "%";
    }

    public void HideInfo()
    {
        expText.text = "";
    }
}
