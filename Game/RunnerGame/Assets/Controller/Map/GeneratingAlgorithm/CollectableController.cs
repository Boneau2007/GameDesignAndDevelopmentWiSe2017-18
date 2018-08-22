using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour {

	public GameObject[] collectables; 
	public GameObject platform;
	private GameManager gameManger; 
	// Use this for initialization
	void Start () {
		gameManger = FindObjectOfType<GameManager> (); 

		int spawnChance = Random.Range (0, 11);
		if (spawnChance < 4) {
			spawnCollectable (chooseRandomCollectable ());
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
		

	private GameObject chooseRandomCollectable(){
		int select = Random.Range (0, 3);
		return collectables[select]; 
	}


	/// <summary>
	/// Spawns the collectable. Collectable gets random selected by the chooseRandomCollectable function. 
	/// The colelctable needs an offset in y position. therefor we use the boxcollider.y value. 
	/// </summary>
	/// <param name="collect">Collect.</param>
	private void spawnCollectable(GameObject collect){
		int heigth = Random.Range (0 , 4);
		Vector3 vec = new Vector3 (0,heigth + this.GetComponent< BoxCollider2D>().size.y,0);
		Instantiate (collect, transform.position + vec ,transform.rotation);
	}
}
