using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Optionally spawns a random collectible above a platform.
/// </summary>
public class CollectableController : MonoBehaviour {

	/// <summary>
	/// Collectible prefabs available for spawning.
	/// </summary>
	public GameObject[] collectables; 
	/// <summary>
	/// Platform associated with this collectible controller.
	/// </summary>
	public GameObject platform;
	private GameManager gameManger; 
	/// <summary>
	/// Chooses whether to spawn a collectible and selects its prefab.
	/// </summary>
	void OnEnable () {
		gameManger = FindAnyObjectByType<GameManager> (); 

		int spawnChance = Random.Range (0, 11);
		if (spawnChance < 4) {
			spawnCollectable (chooseRandomCollectable ());
		}

	}
	
	/// <summary>
	/// Updates the component each frame.
	/// </summary>
	void Update () {
		
	}
		

	/// <summary>
	/// Selects a collectible prefab at random.
	/// </summary>
	/// <returns>A randomly selected collectible prefab.</returns>
	private GameObject chooseRandomCollectable(){
		int select = Random.Range (0, 3);
		return collectables[select]; 
	}


	/// <summary>
	/// Instantiates a collectible at a random height above this platform.
	/// </summary>
	/// <param name="collect">The collectible prefab to instantiate.</param>
	private void spawnCollectable(GameObject collect){
		int heigth = Random.Range (0 , 4);
		Vector3 vec = new Vector3 (0,heigth + this.GetComponent< BoxCollider2D>().size.y,0);
		Instantiate (collect, transform.position + vec ,transform.rotation);
	}
}
