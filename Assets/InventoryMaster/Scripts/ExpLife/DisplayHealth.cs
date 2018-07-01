using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayHealth : EventTrigger
{
    Text healthText;
    private PlayerInventory pi;

    void Start()
    {
        pi = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        healthText = GameObject.Find("HealthProgression").GetComponent<Text>();
    }

    public void DisplayInfo()
    {
        healthText.text = "" + Mathf.RoundToInt(pi.currentHealth) + " / " + Mathf.RoundToInt(pi.maxHealth) + "   -   " + Mathf.RoundToInt(pi.percentageHp * 100) + "%";
    }

    public void HideInfo()
    {
        healthText.text = "";
    }
}
