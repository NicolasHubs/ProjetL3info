using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public class GenerateChunks : MonoBehaviour {

	public const float maxViewDst = 25;
	public Transform viewer;

	public static Vector2 viewerPosition;

	List<GameObject> chunkList = new List<GameObject> ();
	int locationArrayCharacter;

	public GameObject chunk;
	int chunkWidth;

	[Range(3,50)]
	public int numChunks;

	[Tooltip("To set a random seed value, set the seed to 0")]
	public float seed;

	[HideInInspector]
	public bool special;
	[HideInInspector]
	public float a;
	[HideInInspector]
	public float b;

	private int rightChunkStartedPos;
	private int leftChunkEndedPos;

	void Start () {
		special = false;
		chunkWidth = chunk.GetComponent<GenerateChunk> ().width;
		if (seed == 0)
			seed = UnityEngine.Random.Range (-100000f, 100000f);
		
		viewer.position = new Vector3 (chunkWidth * 1.5f, viewer.position.y, viewer.position.z);
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);

		Generate ();

		locationArrayCharacter = Mathf.RoundToInt (Mathf.Floor(viewer.position.x / chunkWidth));

		Debug.Log ("locationArrayCharacter = " + locationArrayCharacter);

		chunkList [locationArrayCharacter].SetActive (true);

		if (locationArrayCharacter < numChunks && locationArrayCharacter > 0) {
			chunkList [locationArrayCharacter - 1].SetActive (true);
			chunkList [locationArrayCharacter + 1].SetActive (true);
			Debug.Log ("1");
		} else if (locationArrayCharacter == numChunks) { // Nb de chunk >= 3 sinon wtf
			chunkList [locationArrayCharacter - 1].SetActive (true);
			chunkList [0].transform.position = new Vector3(numChunks*chunkWidth,0f);
			Debug.Log ("2");
		} else if (locationArrayCharacter == 0){
			chunkList [locationArrayCharacter + 1].SetActive (true);
			chunkList [numChunks - 1].transform.position = new Vector3 (-chunkWidth, 0f);
			Debug.Log ("3");
		}
	
		rightChunkStartedPos = (locationArrayCharacter+1) * chunkWidth;
		leftChunkEndedPos = locationArrayCharacter * chunkWidth;
	}

	public void Generate () {
		for (int i = 0; i < numChunks-1; i++) {
			GameObject newChunk = Instantiate(chunk, new Vector3(i*chunkWidth, 0f), Quaternion.identity) as GameObject;
			newChunk.SetActive (false);
			chunkList.Add (newChunk);
			if(i == 0) b = newChunk.GetComponent<GenerateChunk>().a;
			if(i == numChunks-2) a = newChunk.GetComponent<GenerateChunk>().b;
		}

		special = true;

		GameObject specialChunk = Instantiate(chunk, new Vector3((numChunks-1)*chunkWidth, 0f), Quaternion.identity) as GameObject;
		specialChunk.SetActive (false);
		chunkList.Add (specialChunk);
	}

	void Update() {
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);
		UpdateVisibleChunks();
	}
		
	void UpdateVisibleChunks() {
		if (viewerPosition.x > rightChunkStartedPos) {
			chunkList [mod(locationArrayCharacter - 1,numChunks)].SetActive (false); // => -1%4 = 3
			locationArrayCharacter += 1;
			rightChunkStartedPos += chunkWidth;
			leftChunkEndedPos += chunkWidth;
			chunkList [mod(locationArrayCharacter + 1,numChunks)].transform.position = new Vector3(rightChunkStartedPos,0f);
			chunkList [mod(locationArrayCharacter + 1,numChunks)].SetActive (true);
		} else if (viewerPosition.x < leftChunkEndedPos) {
			chunkList [mod(locationArrayCharacter + 1,numChunks)].SetActive (false);
			locationArrayCharacter -= 1;
			rightChunkStartedPos -= chunkWidth;
			leftChunkEndedPos -= chunkWidth;
			chunkList [mod(locationArrayCharacter - 1,numChunks)].transform.position = new Vector3(leftChunkEndedPos-chunkWidth,0f);
			chunkList [mod(locationArrayCharacter - 1,numChunks)].SetActive (true);
		}
	}

	int mod(int x, int y){
		return ((x % y) + y) % y;
	}
}
