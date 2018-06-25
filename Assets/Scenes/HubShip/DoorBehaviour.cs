using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour {

	private enum Statut {Opened, Closed, Opening, Closing};

	[Tooltip("The time for the door to open/close in second")]
	public float animationTime = 0.5f;

	[Tooltip("The relative Y of the opened door")]
	public float deltaY = 1.05f;

	[Tooltip("The time the door will wait before closing")]
	public float closingTimer = 2.0f;

	private Vector3 closedPosition;
	private Vector3 openedPosition;

	private bool isCharIn = false;
	private Statut doorStatue = Statut.Closed;

	private float currentclosingTimer = 0.0f;
	private float animCurrentTime = 0.0f;

	// Use this for initialization
	void Start () {
		closedPosition = this.transform.position;
		openedPosition = this.transform.position;
		openedPosition = new Vector3(closedPosition.x, closedPosition.y + deltaY, closedPosition.z);
	}
	
	// Update is called once per frame
	void Update () {
		

		if(isCharIn){
			
			switch(doorStatue){
				case Statut.Opened : resetTimer(); break;
				case Statut.Closed : opening(); break;
				case Statut.Opening : break;
				case Statut.Closing : reOpening(); break;
				default : defaultMessage(); break;
			}
		} else {
			
			switch(doorStatue){
				case Statut.Opened : waitTimer(); break;
				case Statut.Closed : break;
				case Statut.Opening : break;
				case Statut.Closing : break;
				default : defaultMessage(); break;
			}
		}

		doorAnimationUpdate();

	}


	private void doorAnimationUpdate(){
		
		if(doorStatue == Statut.Opening){
			this.transform.position = new Vector3(this.transform.position.x, closedPosition.y + deltaY * (animCurrentTime/animationTime),this.transform.position.z);
			animCurrentTime += Time.deltaTime;
			if(animCurrentTime >= animationTime) {
				doorStatue = Statut.Opened;
				animCurrentTime = 0.0f;
				currentclosingTimer = 0.0f;
			}
		} else if (doorStatue == Statut.Closing){
			this.transform.position = new Vector3(this.transform.position.x, openedPosition.y - deltaY * (animCurrentTime/animationTime),this.transform.position.z);
			animCurrentTime += Time.deltaTime;
			if(animCurrentTime >= animationTime) {
				doorStatue = Statut.Closed;
				animCurrentTime = 0.0f;
			}
		}

	}


	private void opening(){
		doorStatue = Statut.Opening;
		animCurrentTime = 0.0f;

	}

	private void reOpening(){
		doorStatue = Statut.Opening;
		animCurrentTime = animationTime - animCurrentTime;


	}

	private void resetTimer(){
		currentclosingTimer = 0.0f;
	}


	private void waitTimer(){
		currentclosingTimer += Time.deltaTime;
		if(currentclosingTimer >= closingTimer) doorStatue = Statut.Closing;
	}

	private void defaultMessage(){
		Debug.Log("This is a default message, it is not supposed to be prompt");
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
