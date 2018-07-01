using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	private float cameraHeight;
	private float cameraWidth;

	// Use this for initialization
	void Start () {

		//cameraHeight = 2 * Camera.main.orthographicSize;
		//cameraWidth = cameraHeight * Camera.main.aspect;

		//transform.position = new Vector3 (cameraWidth / 2, cameraHeight / 2, this.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(0, Time.deltaTime*1.5f, 0));
		Vector3 cameraPos = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
		transform.position = new Vector3 (cameraPos.x, cameraPos.y, this.transform.position.z);
    }
}
