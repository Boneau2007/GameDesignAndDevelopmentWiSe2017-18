using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour {

	public GameObject thePlatform; 
	public GameObject lava;
	public Transform generationPoint; 
	public float distanceBetween;
	private float platformWidth; 
	public float distanceBetweenMin; 
	public float distanceBetweenMax;

	public GameObject[] thePlatforms; 
	public GameObject[] lavaPlatforms;
	public GameObject[] icePlatforms;
	private int platformSelector; 
	private float[] platformWidths; 
	private int gapChoice; 
	private int hazardBlockChance; 
	private int chooseHazardBlock;


	public GameObject lastPlatform; 
	//public ObjectPooler theObjectPool; 

	// Use this for initialization
	void Start () {
		//platformWidth = thePlatform.GetComponent<BoxCollider2D> ().size.x;
		platformWidths = new float[thePlatforms.Length];
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x < generationPoint.position.x) {

			distanceBetween = (int) Random.Range (distanceBetweenMin, distanceBetweenMax);
			gapChoice = Random.Range (0, 6);
			selectPlatformToSpawn ();
			hazardBlockChance = Random.Range (0,11);
			chooseHazardBlock = Random.Range (0,11);

			if (hazardBlockChance <= 7) {
				if (gapChoice < 4) {
					transform.position = new Vector3 (transform.position.x + 1, transform.position.y, 0); 
				} else {
					transform.position = new Vector3 (transform.position.x + distanceBetween, transform.position.y, 0);
				}
				Instantiate (thePlatforms[platformSelector], transform.position, transform.rotation);
				lastPlatform = thePlatforms[platformSelector];

			} else {
				if (chooseHazardBlock < 4) {
					spawnLavaBlock (); 
				} else {
					spawnIceBlock (); 
				}
				spawnBlockAfterLava ();
			}
		}
	}


	/// <summary>
	/// Selects the platform which has to be spawned next. 
	/// depends on the last spwaned platform. prevent spawning obstacles,that can be oversome.
	/// </summary>
	private void selectPlatformToSpawn(){

		switch((int) lastPlatform.GetComponent<BoxCollider2D>().size.y){
		case 2:
			platformSelector = Random.Range (1,4);
			break; 
		case 3: 
			platformSelector = Random.Range (1,4);
			break; 
		case 4: 
			platformSelector = Random.Range (2,5);
			break;
		case 5: 
			platformSelector = Random.Range (3,5);
			break;
		default:
			platformSelector = Random.Range (0, 4);
			break;
		}
	
	}

	/// <summary>
	/// Spawns the lava block.
	/// </summary>
	private void spawnLavaBlock(){

		int chooseLavaHeight = Random.Range (0, 2);
		int selectLava = (int)lastPlatform.GetComponent < BoxCollider2D> ().size.y;
		
		if(chooseLavaHeight == 1 && selectLava >= 2){
			selectLava--;	 
		}
		int t = Random.Range (1, 3);
		for (; t < 3; t++) {
			transform.position = new Vector3 (transform.position.x + 1, transform.position.y, 0);
			Instantiate (lavaPlatforms [selectLava - 1], transform.position, transform.rotation);
		}
		lastPlatform = lavaPlatforms [selectLava-1];
	}
		

	/// <summary>
	/// Spawns a block after lava.
	/// </summary>
	private void spawnBlockAfterLava(){

		int chooseBlockHeight = Random.Range (0, 2);
		int selectPlatform = (int) lastPlatform.GetComponent < BoxCollider2D> ().size.y;

		if(chooseBlockHeight == 1 && selectPlatform < 5  ){
			selectPlatform++;	
		}
		transform.position = new Vector3 (transform.position.x + 1, transform.position.y, 0); 
		Instantiate (thePlatforms[selectPlatform-1], transform.position, transform.rotation);
		lastPlatform = thePlatforms[selectPlatform-1];
	}

	public GameObject getLastPlatform(){
		return lastPlatform; 
	}

	private void spawnIceBlock(){
		int chooseIceHeight = Random.Range (0, 2);
		int selectIce = (int)lastPlatform.GetComponent < BoxCollider2D> ().size.y;

		if(chooseIceHeight == 1 && selectIce >= 2){
			selectIce--;	 
		}
		int t = Random.Range (1, 4);
		for (; t < 4; t++) {
			transform.position = new Vector3 (transform.position.x + 1, transform.position.y, 0);
			Instantiate (icePlatforms [selectIce - 1], transform.position, transform.rotation);
		}
		lastPlatform = icePlatforms [selectIce-1];
	}
}
