using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPInteract : MonoBehaviour {

	[Header("Sprites")]

	public Sprite pad;
	public Sprite selectedPad;

	[HideInInspector]
	public bool isCharIn = false;

	private SpriteRenderer sr;

	// Use this for initialization
	void Start () {
		sr = this.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateSprite();
	}


	private void UpdateSprite(){
		sr.sprite = isCharIn? selectedPad : pad;
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
