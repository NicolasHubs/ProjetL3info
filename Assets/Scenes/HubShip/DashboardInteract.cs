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
	}


	private void UpdateSprite(){
		if(isCharIn) 
			sr.sprite = spriteNumber01? selectedScreen01 : selectedScreen02;
		else 
			sr.sprite = spriteNumber01? screen01 : screen02;
		
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
