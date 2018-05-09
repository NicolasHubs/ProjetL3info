using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public class GenerateChunks : MonoBehaviour {

	public const float maxViewDst = 25;
	public Transform viewer;

	public static Vector2 viewerPosition;

	List<GameObject> columnList = new List<GameObject> ();
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

	private float rightEndPos;
	private float leftEndPos;

	void Start () {
		special = false;
		chunkWidth = chunk.GetComponent<GenerateChunk> ().width;
		if (seed == 0)
			seed = UnityEngine.Random.Range (-100000f, 100000f);
		
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.y);

		Generate ();

		foreach (GameObject t in GameObject.FindGameObjectsWithTag("Column")) {
			t.SetActive (false);
			columnList.Add (t);
		}
		locationArrayCharacter = Mathf.RoundToInt(Mathf.Floor(viewerPosition.x))*2;

		for (int i = locationArrayCharacter-chunkWidth ; i<=locationArrayCharacter + chunkWidth; i++){
			columnList [i].SetActive (true);
		}
		rightEndPos = (Mathf.Abs (viewerPosition.x) % 1) >= 0.5f ? Mathf.Ceil(viewerPosition.x) - 0f: Mathf.Ceil(viewerPosition.x) - 0.5f;

		leftEndPos = (Mathf.Abs (viewerPosition.x) % 1) > 0.5f ?  Mathf.Floor(viewerPosition.x) + 0.5f :  Mathf.Floor(viewerPosition.x) + 0f;
	}

	public void Generate () {
		for (int i = 0; i < numChunks-1; i++) {
			GameObject newChunk = Instantiate(chunk, new Vector3(i*chunkWidth, 0f), Quaternion.identity) as GameObject;
			if(i == 0) b = newChunk.GetComponent<GenerateChunk>().a;
			if(i == numChunks-2) a = newChunk.GetComponent<GenerateChunk>().b;
		}

		special = true;
		GameObject specialChunk = Instantiate(chunk, new Vector3((numChunks-1)*chunkWidth, 0f), Quaternion.identity) as GameObject;
	}
		
	void UpdateVisibleChunks() {
		if (viewerPosition.x >= rightEndPos) {
			columnList [mod(locationArrayCharacter - chunkWidth,numChunks*chunkWidth*2)].SetActive (false); // => -1%4 = 3
			locationArrayCharacter += 1;
			rightEndPos += 0.5f;
			leftEndPos += 0.5f;
			columnList [mod(locationArrayCharacter + chunkWidth,numChunks*chunkWidth*2)].transform.position = new Vector3(rightEndPos + (chunkWidth/2) - 0.5f,0f);
			columnList [mod(locationArrayCharacter + chunkWidth,numChunks*chunkWidth*2)].SetActive (true);
		} else if (viewerPosition.x < leftEndPos) {
			columnList [mod(locationArrayCharacter + chunkWidth,numChunks*chunkWidth*2)].SetActive (false);
			locationArrayCharacter -= 1;
			rightEndPos -= 0.5f;
			leftEndPos -= 0.5f;
			columnList [mod(locationArrayCharacter - chunkWidth,numChunks*chunkWidth*2)].transform.position = new Vector3(leftEndPos - (chunkWidth/2),0f);
			columnList [mod(locationArrayCharacter - chunkWidth,numChunks*chunkWidth*2)].SetActive (true);
		}
	}

	int mod(int x, int y){
		return ((x % y) + y) % y;
	}

	void Update() {
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.y);
		UpdateVisibleChunks ();
	}
}
