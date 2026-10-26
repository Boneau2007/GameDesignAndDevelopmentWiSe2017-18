using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Shows the current run score as text in the HUD.
/// </summary>
public class ScoreTextController : MonoBehaviour {

	private GameManager gm;
	/// <summary>
	/// Text element used to display the current score.
	/// </summary>
	public Text scoreTextObject;

	/// <summary>
	/// Finds the game manager that provides the current score.
	/// </summary>
	void Start () {
		gm = FindObjectOfType<GameManager> ();
	}
	
	/// <summary>
	/// Refreshes the displayed score.
	/// </summary>
	void Update () {
		showScore ();
	}

	/// <summary>
	/// Writes the current score to the HUD text element.
	/// </summary>
	void showScore() {
		scoreTextObject.text = Localization.Format ("hud.score", gm.getScore ());
	}
}
