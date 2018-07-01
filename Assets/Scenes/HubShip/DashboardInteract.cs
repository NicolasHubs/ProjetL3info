using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashboardInteract : MonoBehaviour {

	[Tooltip("The time of the animation (Blinking)")]
	public float animationTime = 0.5f;

	[Header("Animation's Sprites")]

	public Sprite screen01;
	public Sprite screen02;
	public Sprite selectedScreen01;
	public Sprite selectedScreen02;
	public GameObject dashboard;
	public GameObject labelInfo;
    public GameObject Menu;
	[HideInInspector]
	public bool isCharIn = false;
	private bool spriteNumber01 = true;

	private float currentTime = 0.0f;

	private SpriteRenderer sr;

	// Use this for initialization
	void Start () {
		sr = this.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(currentTime >= animationTime) {
			currentTime = 0.0f;
			spriteNumber01 = !spriteNumber01;
		} else currentTime += Time.deltaTime;

		UpdateSprite();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (dashboard.activeSelf)
                dashboard.SetActive(false);
            Menu.SetActive(!Menu.activeSelf);
        }
    }

	private void UpdateSprite(){
		if (isCharIn) {
			sr.sprite = spriteNumber01 ? selectedScreen01 : selectedScreen02;
			labelInfo.SetActive (true);
			if (Input.GetKeyDown (KeyCode.E)) {
				dashboard.SetActive(!dashboard.activeSelf);
			}
		} else {
			sr.sprite = spriteNumber01? screen01 : screen02;
			labelInfo.SetActive (false);
		}
		
	}

	void OnTriggerEnter2D (Collider2D other) {
		isCharIn = true;
	}

	void OnTriggerStay2D (Collider2D other) {
		
	}

	void OnTriggerExit2D (Collider2D other) {
		isCharIn = false;
	}
}
