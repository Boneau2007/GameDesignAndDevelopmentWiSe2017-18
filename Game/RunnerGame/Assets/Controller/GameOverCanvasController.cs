using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Displays the final score and distance on the game-over screen.
/// </summary>
public class GameOverCanvasController : MonoBehaviour {

	private GameManager gm;
	/// <summary>
	/// UI text element used to display the final score.
	/// </summary>
	public Text scoreText;
	/// <summary>
	/// UI text element used to display the distance travelled.
	/// </summary>
	public Text distanceText;

	/// <summary>
	/// Finds the game manager used to retrieve the final run results.
	/// </summary>
	void Start () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}
	
	/// <summary>
	/// Updates the result labels with the current score and distance.
	/// </summary>
	void Update () {
		scoreText.text = "You achieved " + gm.getScore() + " points!";
		distanceText.text = "You travelled " + gm.getDistance () + " m."; 
	}
}
