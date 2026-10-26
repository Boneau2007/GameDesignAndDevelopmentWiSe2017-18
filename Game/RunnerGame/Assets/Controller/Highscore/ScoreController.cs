using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks score earned over time and from collectible pickups.
/// </summary>
public class ScoreController : MonoBehaviour {

	/// <summary>
	/// Current score for the run.
	/// </summary>
	[SerializeField]
	private int score;
	/// <summary>
	/// Game manager used to determine whether the run is active.
	/// </summary>
	[SerializeField]
	private GameManager gameManager;

	/// <summary>
	/// Finds the game manager used to read the run state.
	/// </summary>
	void Start () {
		gameManager = FindObjectOfType<GameManager> ();
	}
	
	/// <summary>
	/// Unused frame update callback.
	/// </summary>
	void Update () {
		
	}

	/// <summary>
	/// Adds passive score while the run is active.
	/// </summary>
	void FixedUpdate() {
		startCalculatingScore ();
	}

	/// <summary>
	/// Increases the score while the game manager reports an active run.
	/// </summary>
	private void startCalculatingScore(){
		if(gameManager.getGameStatus ()){
			score += 2;
			//Debug.Log ("Score:  " + score);
		}
	}

	/// <summary>
	/// Adds points earned from a collectible to the current score.
	/// </summary>
	/// <param name="points">The number of points to add.</param>
	public void addPointsFromCollectable(int points){
		score += points;
		Debug.Log ("100 points from collectable");
	}

	/// <summary>
	/// Gets the current score.
	/// </summary>
	/// <returns>The current score in points.</returns>
	public int getScore() {
		return score;
	}
}
