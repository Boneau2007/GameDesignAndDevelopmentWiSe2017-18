using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour {

	[SerializeField]
	private int score;
	[SerializeField]
	private GameManager gameManager;

	// Use this for initialization
	void Start () {
		gameManager = FindObjectOfType<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		startCalculatingScore ();
	}

	/// <summary>
	/// Starts the calculating score. Invoked when the game starts. 
	/// </summary>
	private void startCalculatingScore(){
		if(gameManager.getGameStatus ()){
			score += 2;
			//Debug.Log ("Score:  " + score);
		}
	}

	public void addPointsFromCollectable(int points){
		score += points;
		Debug.Log ("100 points from collectable");
	}

	public int getScore() {
		return score;
	}
}
