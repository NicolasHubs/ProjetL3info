using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TPInteract : MonoBehaviour {

	[Header("Sprites")]

	public Sprite pad;
	public Sprite selectedPad;
	public GameObject labelInfo;
	public GameObject TPtrue;
	public GameObject TPfalse;
	public TextMeshProUGUI text;
	[HideInInspector]
	public bool isCharIn = false;
	private Planet planet;
	private SpriteRenderer sr;
	private bool planetSelected;
	private bool loadingFinished;
	// Use this for initialization
	void Start () {
		sr = this.GetComponent<SpriteRenderer>();
		planet = new Planet ();
		planetSelected = false;
		loadingFinished = false;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateSprite();
	}

	private void UpdateSprite(){
		sr.sprite = isCharIn? selectedPad : pad;
		if (isCharIn) {
			labelInfo.SetActive (true);
			if (Input.GetKeyDown (KeyCode.E) && planetSelected) {
				if (!loadingFinished)
					Play ();
			}
		} else {
			labelInfo.SetActive (false);
		}
	}

	public void Play(){
		StartCoroutine (LoadGameScene ());
		loadingFinished = true;
	}
	private AsyncOperation result;
	IEnumerator LoadGameScene(){
		result = Scenes.LoadAsync ("Planet",planet);
		while (!result.isDone) {
			float progress = Mathf.Clamp01 (result.progress / 0.9f);
			yield return null;
		}
		yield return null;
	}

	public void setPlanet(Planet planet) {
		this.planet = planet;
		planetSelected = true;
		TPtrue.SetActive (true);
		TPfalse.SetActive (false);
		text.text = "Press \"E\" to interact";
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
