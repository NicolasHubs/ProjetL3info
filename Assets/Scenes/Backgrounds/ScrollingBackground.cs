using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour {


	public float paralaxSpeed;
	public bool IsInSpace;

	private float backgroundSize;
	private Transform cameraTransform;
	private Transform[] layers;
	private float viewZone = 10;
	private int leftIndex;
	private int rightIndex;
	private float lastCameraX;

	private float time;
	private float interpolationPeriod = 0.01f;

	private void Start(){
		bool error = false;

		if (transform.childCount == 3 && this.transform.localScale.x == 1) {
			cameraTransform = Camera.main.transform;
			lastCameraX = cameraTransform.position.x;
			layers = new Transform[transform.childCount];
			for (int i = 0; i < transform.childCount; i++)
				layers [i] = transform.GetChild (i);

			backgroundSize = Mathf.Abs(layers [2].position.x - layers[1].position.x);

			float a = layers [0].position.x;
			float b = (layers [1].position.x - backgroundSize);

			if (a != b)
				error = true;
			else {
				leftIndex = 0;
				rightIndex = layers.Length - 1;
				time = 0.0f;
			}
		} else {
			error = true;
		}

		if (error) {
			if (transform.childCount != 3)
				Debug.Log ("The paralax system needs exactly 3 childs and it has only " + transform.childCount + " childs.");
			
			if (this.transform.localScale.x != 1)
				Debug.Log ("The paralax system needs parent with a local Scale = 1 and actually its value is " + this.transform.localScale.x);

			if (layers [0].position.x != layers [1].position.x - backgroundSize)
				Debug.Log ("Some childs x's position are not correctly set.");
			
			enabled = false;
		}
	}

	private void Update(){
		
		float deltaX = cameraTransform.position.x - lastCameraX;

		this.transform.position = new Vector3 (this.transform.position.x + deltaX * paralaxSpeed, GameObject.FindGameObjectWithTag ("Player").transform.position.y, this.transform.position.z);

		if (IsInSpace) {
			time += Time.deltaTime;
			if (time >= interpolationPeriod) {
				this.transform.position += Vector3.left * 0.01f;
				time = 0.0f;
			}
			this.transform.position -= Vector3.right * (deltaX * paralaxSpeed);
		} 

		lastCameraX = cameraTransform.position.x;

		if (cameraTransform.position.x < (layers [leftIndex].transform.position.x + viewZone))
			ScrollLeft ();
		if (cameraTransform.position.x > (layers [rightIndex].transform.position.x - viewZone))
			ScrollRight ();


	}

	private void ScrollLeft(){
		layers [rightIndex].position = new Vector3(layers [leftIndex].position.x - backgroundSize, this.transform.position.y,this.transform.position.z);
		leftIndex = rightIndex;
		rightIndex--;
		if (rightIndex < 0)
			rightIndex = layers.Length - 1;
	}

	private void ScrollRight(){
		layers [leftIndex].position = new Vector3(layers [rightIndex].position.x + backgroundSize, this.transform.position.y,this.transform.position.z);
		rightIndex = leftIndex;
		leftIndex++;
		if (leftIndex == layers.Length)
			leftIndex = 0;
	}
}
