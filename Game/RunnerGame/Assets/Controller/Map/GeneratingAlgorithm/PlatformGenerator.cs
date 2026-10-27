using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates platforms and hazard blocks ahead of the player.
/// </summary>
public class PlatformGenerator : MonoBehaviour {
	private const float HazardSectionStep = 1f;

	/// <summary>
	/// Default platform prefab.
	/// </summary>
	public GameObject thePlatform; 
	/// <summary>
	/// Lava prefab reference used by the generator.
	/// </summary>
	public GameObject lava;
	/// <summary>
	/// Position that triggers generation of additional platforms.
	/// </summary>
	public Transform generationPoint; 
	/// <summary>
	/// Gap distance selected for the next platform.
	/// </summary>
	public float distanceBetween;
	private float platformWidth; 
	/// <summary>
	/// Smallest randomized distance between generated platforms.
	/// </summary>
	public float distanceBetweenMin; 
	/// <summary>
	/// Largest randomized distance between generated platforms.
	/// </summary>
	public float distanceBetweenMax;

	/// <summary>
	/// Platform prefabs available for generation.
	/// </summary>
	public GameObject[] thePlatforms; 
	/// <summary>
	/// Lava platform prefabs indexed by their height.
	/// </summary>
	public GameObject[] lavaPlatforms;
	/// <summary>
	/// Ice platform prefabs indexed by their height.
	/// </summary>
	public GameObject[] icePlatforms;
	private int platformSelector; 
	private float[] platformWidths; 
	private int gapChoice; 
	private int hazardBlockChance; 
	private int chooseHazardBlock;


	/// <summary>
	/// Most recently selected platform prefab.
	/// </summary>
	public GameObject lastPlatform; 
	private ObjectPooler theObjectPool;

	/// <summary>
	/// Creates or configures the shared pool before platform generation begins.
	/// </summary>
	void Awake () {
		theObjectPool = GetComponent<ObjectPooler> ();
		if (theObjectPool == null) {
			theObjectPool = gameObject.AddComponent<ObjectPooler> ();
		}

		List<GameObject> platformPrefabs = new List<GameObject> ();
		platformPrefabs.AddRange (thePlatforms);
		platformPrefabs.AddRange (lavaPlatforms);
		platformPrefabs.AddRange (icePlatforms);
		theObjectPool.InitializePools (platformPrefabs, ObjectPooler.InitialPlatformPoolSize);
	}

	/// <summary>
	/// Allocates platform-width storage for the configured platform prefabs.
	/// </summary>
	void Start () {
		//platformWidth = thePlatform.GetComponent<BoxCollider2D> ().size.x;
		platformWidths = new float[thePlatforms.Length];
	}
	
	/// <summary>
	/// Generates a new platform section when the generation point is reached.
	/// </summary>
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
				theObjectPool.SpawnPooledObject (thePlatforms[platformSelector], transform.position, transform.rotation);
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
	/// Selects a platform prefab based on the height of the last generated platform.
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
	/// Spawns a lava section using a height compatible with the previous platform.
	/// </summary>
	private void spawnLavaBlock(){

		int chooseLavaHeight = Random.Range (0, 2);
		int selectLava = (int)lastPlatform.GetComponent < BoxCollider2D> ().size.y;
		
		if(chooseLavaHeight == 1 && selectLava >= 2){
			selectLava--;	 
		}
		int t = Random.Range (1, 3);
		for (; t < 3; t++) {
			transform.position = new Vector3 (transform.position.x + HazardSectionStep, transform.position.y, 0);
			theObjectPool.SpawnPooledObject (lavaPlatforms [selectLava - 1], transform.position, transform.rotation);
		}
		lastPlatform = lavaPlatforms [selectLava-1];
	}
		

	/// <summary>
	/// Spawns a normal platform after a lava section.
	/// </summary>
	private void spawnBlockAfterLava(){

		int chooseBlockHeight = Random.Range (0, 2);
		int selectPlatform = (int) lastPlatform.GetComponent < BoxCollider2D> ().size.y;

		if(chooseBlockHeight == 1 && selectPlatform < 5  ){
			selectPlatform++;	
		}
		transform.position = new Vector3 (transform.position.x + HazardSectionStep, transform.position.y, 0); 
		theObjectPool.SpawnPooledObject (thePlatforms[selectPlatform-1], transform.position, transform.rotation);
		lastPlatform = thePlatforms[selectPlatform-1];
	}

	/// <summary>
	/// Gets the last platform prefab selected by the generator.
	/// </summary>
	/// <returns>The last platform prefab.</returns>
	public GameObject getLastPlatform(){
		return lastPlatform; 
	}

	/// <summary>
	/// Spawns an ice section using a height compatible with the previous platform.
	/// </summary>
	private void spawnIceBlock(){
		int chooseIceHeight = Random.Range (0, 2);
		int selectIce = (int)lastPlatform.GetComponent < BoxCollider2D> ().size.y;

		if(chooseIceHeight == 1 && selectIce >= 2){
			selectIce--;	 
		}
		int t = Random.Range (1, 4);
		for (; t < 4; t++) {
			transform.position = new Vector3 (transform.position.x + HazardSectionStep, transform.position.y, 0);
			theObjectPool.SpawnPooledObject (icePlatforms [selectIce - 1], transform.position, transform.rotation);
		}
		lastPlatform = icePlatforms [selectIce-1];
	}
}
