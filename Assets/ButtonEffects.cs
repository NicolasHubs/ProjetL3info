using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEffects : EventTrigger {

	// Use this for initialization
	void Start () {
		
	}

    public void DisplayEffects()
    {
        transform.GetChild(1).GetComponent<CanvasGroup>().alpha = 1;
        transform.GetChild(0).GetComponent<Text>().fontSize = 36;
    }

    public void RemoveEffects()
    {
        transform.GetChild(1).GetComponent<CanvasGroup>().alpha = 0;
        transform.GetChild(0).GetComponent<Text>().fontSize = 33;
    }
}
