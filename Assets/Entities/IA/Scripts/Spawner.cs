using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;


public class Spawner : MonoBehaviour {
	private List<GameObject> enemies;

	[Tooltip("Max number of enemies alive.")]
	public int maxEnemies;

	[HideInInspector]
	public List<GameObject> enemySpawnList;

	private int randomEnemy;
	private float spawnWait;
	private string[] filterList = new string[] {
		"#FFFFFF",
		"#FFA1A1",
		"#FFF39C",
		"#A1FF96",
		"#90FFFA",
		"#9589FF",
		"#FF7E7E"
	};
	private float time;
	private float interpolationDayPeriod = 10f;
	private float interpolationNightPeriod = 5f;
	private Transform player;
	private PlanetGenerator planetScript;
	private Tilemap frontGround;

	void Start () {
		enemySpawnList = new List<GameObject> ();

		enemies = new List<GameObject>();
		foreach (Object t in Resources.LoadAll("AIprefab/Dumb", typeof(GameObject)))
			enemies.Add ((GameObject)t);
		
		frontGround = GameObject.FindGameObjectWithTag("FrontGround").gameObject.GetComponent<Tilemap>();

		player = GameObject.FindGameObjectWithTag ("Player").transform;
		planetScript = GameObject.FindGameObjectWithTag ("grid").GetComponent<PlanetGenerator> ();
		time = 0.0f;
	}

	void Update () {
		time += Time.deltaTime;
		if (CycleJourNuit.timeOfDay < 12000 || CycleJourNuit.timeOfDay >= 75000) { // Night Time
			if (time >= interpolationNightPeriod) {
				CleanNonVisibleEnemies ();
				StopCoroutine (EnemySpawn ());
				StartCoroutine (EnemySpawn ());

				time = 0.0f;
			}
		} else if (time >= interpolationDayPeriod) {
			if (Random.Range (0, 10) == 9 && enemySpawnList.Count < 4) { // Other
				CleanNonVisibleEnemies ();
				StopCoroutine (EnemySpawn ());
				StartCoroutine (EnemySpawn ());
			}
			time = 0.0f;
		}
	}

	public void CleanNonVisibleEnemies(){
		if (enemySpawnList.Count > 0) {
			List <GameObject> enemyToDelete = new List<GameObject> ();
			foreach (GameObject enemy in enemySpawnList) {
				Vector3Int enemyCellPos = frontGround.WorldToCell (enemy.transform.position);
				Vector3Int playerCellPos = frontGround.WorldToCell (player.transform.position);
				if (!(enemyCellPos.x >= (playerCellPos.x - planetScript.fieldOfRender.x - planetScript.additionalSight) &&
					enemyCellPos.x <= (playerCellPos.x + planetScript.fieldOfRender.x + planetScript.additionalSight) &&
					enemyCellPos.y >= (playerCellPos.y - planetScript.fieldOfRender.y - planetScript.additionalSight) &&
					enemyCellPos.y <= (playerCellPos.y + planetScript.fieldOfRender.y + planetScript.additionalSight)) || frontGround.GetTile(enemyCellPos) != null) {
					enemyToDelete.Add (enemy);

				}
			}
			DeleteEntity (enemyToDelete);
		}
	}

	public void DeleteEntity(List<GameObject> enemyToDeleteList){
		foreach (GameObject enemy in enemyToDeleteList) {
			Destroy (enemy);
			enemySpawnList.Remove (enemy);
		}
	}

	public Vector2Int WorldCameraSize(){
		float cameraHeight = 2 * Camera.main.orthographicSize;
		float cameraWidth = cameraHeight * Camera.main.aspect;

		cameraHeight /= frontGround.cellSize.y;
		cameraWidth /= frontGround.cellSize.x;
		int worldCameraheight = Mathf.CeilToInt(cameraHeight+2);
		int worldCameraWidth = Mathf.CeilToInt(cameraWidth+2);

		/*
		Vector3Int playerPosition = world.WorldToCell (player.position);
		world.ClearAllTiles ();

		for (int x = 0; x < worldCameraWidth; x++) {
			world.SetTile (new Vector3Int (Mathf.RoundToInt(playerPosition.x - (worldCameraWidth/2) + x), Mathf.RoundToInt(playerPosition.y - (worldCameraheight/2)), 0), toto);
			world.SetTile (new Vector3Int (Mathf.RoundToInt(playerPosition.x - (worldCameraWidth/2) + x), Mathf.RoundToInt(playerPosition.y + (worldCameraheight/2)), 0), toto);
		}

		for (int y = 0; y <= worldCameraheight; y++) {
			world.SetTile (new Vector3Int (Mathf.RoundToInt(playerPosition.x - (worldCameraWidth/2)), Mathf.RoundToInt (playerPosition.y - (worldCameraheight / 2) + y), 0), toto);
			world.SetTile (new Vector3Int (Mathf.RoundToInt(playerPosition.x + (worldCameraWidth/2)), Mathf.RoundToInt(playerPosition.y - (worldCameraheight/2) + y), 0), toto);
		}*/

		return new Vector2Int (worldCameraWidth, worldCameraheight);
	}

	private Vector3 FindSpawnPosition(){

		int leftOrRightSide = Random.Range(0,2)*2-1; // Left : -1 | Right : 1
		Vector2Int worldCamera = WorldCameraSize();
		Vector3 enemySpawnPosition = Vector3.zero;
		Vector3Int playerPosition = frontGround.WorldToCell (player.position);

		int startX = Mathf.RoundToInt (playerPosition.x + ((worldCamera.x / 2) + 1)*leftOrRightSide);

		int i = startX;
		int x = 0;

		if(i < 0) {
			x = Mathf.Abs(i) - 1;
			int quotient = x / planetScript.planet.savedMapMatrix.GetLength(0);
			x = (planetScript.planet.savedMapMatrix.GetLength(0) - 1) - (x - planetScript.planet.savedMapMatrix.GetLength(0) * quotient);
		} else {
			int quotient = i / planetScript.planet.savedMapMatrix.GetLength(0);
			x = i - planetScript.planet.savedMapMatrix.GetLength(0) * quotient;
		}

		int distEndYCameraToHeightMap = Mathf.Abs(planetScript.planet.savedMapHeight[x]-playerPosition.y);
		int endY = Mathf.RoundToInt(playerPosition.y + (worldCamera.y/2) + distEndYCameraToHeightMap);

		for (int j = playerPosition.y; j < endY; j++){
			if (planetScript.planet.savedMapMatrix [x, j] == 0) {
				enemySpawnPosition = frontGround.CellToWorld(new Vector3Int (i, j, 0));
				break;
			}
		}
		return enemySpawnPosition;
	}

	IEnumerator EnemySpawn() {
		if (enemySpawnList.Count < maxEnemies) {
			
			Vector3 enemySpawnPosition = FindSpawnPosition ();

			if (enemySpawnPosition != Vector3.zero) {
				int rdmFilter = 0;
				randomEnemy = Random.Range (0, enemies.Count);

				GameObject enemy = Instantiate (enemies [randomEnemy], enemySpawnPosition, gameObject.transform.rotation) as GameObject;
				enemy.transform.SetParent (GameObject.FindGameObjectWithTag ("AIgroup").transform);

				/******   MODIFIER   ******/

				// Color filter
				if (filterList.GetLength (0) > 0) {
					if (Random.Range (0, 9) >= 6) {
						rdmFilter = Random.Range (0, filterList.Length);
						Color filter = new Color ();
						ColorUtility.TryParseHtmlString ((string)filterList [rdmFilter].Clone (), out filter);
						enemy.GetComponent<SpriteRenderer> ().color = filter;
					}
				}

				// Scale
				if (Random.Range (0, 20) >= 19) {
					Vector3 scale = enemy.transform.localScale;
					float scaleAdd = Random.Range (-1.5f, 2f);

					enemy.transform.localScale += new Vector3 (scaleAdd, scaleAdd, 0);
				}

				// Speed

				// Jump height

				// Attack

				// Health

				// Defense

				// Name modifier ?

				// Loot 

				// Attack rate

				// Critical rate ?

				enemySpawnList.Add (enemy);
			} else {
				Debug.Log ("Aucune position disponible");
			}
		}

		yield return null;
	}

}
