using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Shows the player's remaining life points as text in the HUD.
/// </summary>
public class LifepointViewController : MonoBehaviour {

	private GameManager gm;
	/// <summary>
	/// Text element used to display the current life-point count.
	/// </summary>
	public Text lifepointsObject;


	/// <summary>
	/// Finds the game manager that provides the player's life-point count.
	/// </summary>
	void Start () {
		gm = FindObjectOfType<GameManager> ();
	}
	
	/// <summary>
	/// Refreshes the displayed life-point count.
	/// </summary>
	void Update () {
		ShowLifePoints ();
	}

	/// <summary>
	/// Writes the current life-point count to the HUD text element.
	/// </summary>
	void ShowLifePoints() {
		lifepointsObject.text = "Lifepoints: " + gm.getPlayerLifePoints ();
	}
}
