using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class GroundGenerator2 : MonoBehaviour {  

	public GameObject mainCamera;

	[Header("Specs of the planet")]

	[Range(48,3600)]
	public int numberOfBlocks = 100;

	[Tooltip("To set a random seed value, set the seed to 0")]
	public float seed;
	public int heightMultiplier;
	public int heightAddition;
	public float smoothness;
	[Space(10)]


	[Header("Tiles")]
	public UnityEngine.Tilemaps.TileBase grass;
	public UnityEngine.Tilemaps.TileBase dirt;
	public UnityEngine.Tilemaps.TileBase rock;

	[HideInInspector]
	public int a;
	[HideInInspector]
	public int b;

	private GameObject groundGrid;	
	private GameObject frontground;
	private GameObject background;

	private int prevChunkIndex;
	private int currentChunkIndex;
	private int nextChunkIndex;

	private int lastPrevChunkIndex;
	private int lastCurrentChunkIndex;
	private int lastNextChunkIndex;

	List<List<Vector3Int>> listGlobale = new List<List<Vector3Int>> ();

	//List<Vector3Int> list = new List<Vector3Int> ();

	Object lockObject = new Object();

	private Vector3Int [] tabo;
	//private List<Thread> workerThread = new List<Thread> ();

	// Use this for initialization
	void Start () {
		groundGrid = this.gameObject;
		if (seed == 0) seed = UnityEngine.Random.Range(-100000f, 100000f);

		//Instanciation et ajout des composants du frontground
		frontground = new GameObject("Frontground");
		frontground.AddComponent<UnityEngine.Tilemaps.Tilemap>();
		frontground.AddComponent<UnityEngine.Tilemaps.TilemapRenderer>();
		frontground.AddComponent<UnityEngine.Tilemaps.TilemapCollider2D>();
		frontground.AddComponent<Rigidbody2D>();
		frontground.AddComponent<CompositeCollider2D>();

		//Modification de certaines valeurs du frontground
		frontground.GetComponent<UnityEngine.Tilemaps.TilemapCollider2D>().usedByComposite = true;
		frontground.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
		frontground.transform.parent = groundGrid.transform;

		//Instanciation et ajout des composants du background
		background = new GameObject("Background");
		background.AddComponent<UnityEngine.Tilemaps.Tilemap>();
		background.AddComponent<UnityEngine.Tilemaps.TilemapRenderer>();
		
		//Modification de certaines valeurs du background
		background.transform.parent = groundGrid.transform;

		Thread t1 = new Thread (Generate1);
		//Thread t2 = new Thread (Generate2);
		//Thread t3 = new Thread (Generate3);
		//Thread t4 = new Thread (Generate4);

		t1.Start ();
		//t2.Start ();
		//t3.Start ();
		//t4.Start ();

		t1.Join ();
		//t2.Join ();
		//t3.Join ();
		//t4.Join ();

		/*foreach (Thread th in workerThread)
			th.Join ();*/

		foreach (List<Vector3Int> list in listGlobale)
			foreach (Vector3Int v in list)
				frontground.GetComponent<UnityEngine.Tilemaps.Tilemap> ().SetTile (v, rock);
		
		/*tabo = new Vector3Int[numberOfBlocks*(heightAddition+20)];
		Thread momo = new Thread (Generate);
		momo.Start ();
		momo.Join ();
		print (tabo.Length);
		for (int i = 0;i<tabo.Length;i++)
			frontground.GetComponent<UnityEngine.Tilemaps.Tilemap> ().SetTile (tabo[i], rock);*/

	}

	/*private void Generate(){
		List<Thread> mama = new List<Thread> ();
		int previous = 0;
		for (int i = 0; i < numberOfBlocks; i++) {
			int height = Mathf.RoundToInt(Mathf.PerlinNoise(seed, i / smoothness) * heightMultiplier) + heightAddition;
			previous += height;
			Thread t = new Thread (() => marcheplusvitestp1 (i, previous,height));
			t.Start ();
			mama.Add (t);
		}

		foreach (Thread t in mama)
			t.Join ();
	}

	public void marcheplusvitestp1(int i, int previous, int height)
	{
		if(i == 0) a = height;
		else if (i == numberOfBlocks-1) b = height;

		for (int j = 0; j < height - 1; j++)
			tabo.SetValue(new Vector3Int (i, j, 0),(j + previous));
	}*/

	private void Generate1(){
		List<Thread> workerThread = new List<Thread> ();

		//for(int i = 0; i < numberOfBlocks/4; i ++){
		for (int i = 0; i < numberOfBlocks; i++) {
			int height = Mathf.RoundToInt (Mathf.PerlinNoise (seed, i / smoothness) * heightMultiplier) + heightAddition;
			Thread t = new Thread (() => marcheplusvitestp (i, height));
			t.Start ();
			workerThread.Add (t);
		}

		foreach (Thread th in workerThread)
			th.Join();
	}

	/*private void Generate2(){
		List<Thread> workerThread = new List<Thread> ();

		for(int i = numberOfBlocks/4; i < numberOfBlocks/2; i++){
			Thread t = new Thread (() => marcheplusvitestp (i));
			t.Start ();
			workerThread.Add (t);
		}
		foreach (Thread th in workerThread)
			th.Join();
	}

	private void Generate3(){
		List<Thread> workerThread = new List<Thread> ();

		for(int i = numberOfBlocks/2; i < (numberOfBlocks*3)/4; i++){
			Thread t = new Thread (() => marcheplusvitestp (i));
			t.Start ();
			workerThread.Add (t);
		}

		foreach (Thread th in workerThread)
			th.Join();
	}

	private void Generate4(){
		List<Thread> workerThread = new List<Thread> ();

		for(int i = (numberOfBlocks*3)/4; i < numberOfBlocks; i++){
			Thread t = new Thread (() => marcheplusvitestp (i));
			t.Start ();
			workerThread.Add (t);
		}

		foreach (Thread th in workerThread)
			th.Join();
	}*/

	public void marcheplusvitestp(int i,int height)
	{
		List<Vector3Int> list = new List<Vector3Int> ();

		if(i == 0){
			a = height;
		} else if (i == numberOfBlocks-1) {
			b = height;
		}

		for (int j = 0; j < height - 1; j++)
				list.Add (new Vector3Int (i, j, 0));
		
		lock (lockObject) 
			listGlobale.Add (list);

		/*lock (lockObject)
			t.Abort ();*/
	}
	// Update is called once per frame
	void Update () {
		
	}

	private void UpdateChunksPosition(){
		
	}

	private void UpdateChunksVisibility() {

	}

	private void swapToRight(){

	}

	private void swapToLeft(){
		
	}
}
