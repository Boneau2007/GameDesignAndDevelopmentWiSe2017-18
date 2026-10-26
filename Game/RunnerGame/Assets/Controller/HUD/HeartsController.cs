using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Displays the player's life points using a heart sprite.
/// </summary>
public class HeartsController : MonoBehaviour {

	/// <summary>
	/// Heart sprites indexed by the player's remaining life points.
	/// </summary>
	public Sprite[] heartsSprites;
	/// <summary>
	/// Image component that displays the selected heart sprite.
	/// </summary>
	public Image heartUI;
	private GameManager gm;

	/// <summary>
	/// Finds the game manager that provides the player's life-point count.
	/// </summary>
	void Start () {
		gm = FindObjectOfType<GameManager> ();
	}
	
	/// <summary>
	/// Selects the heart sprite corresponding to the current life-point count.
	/// </summary>
	void Update () {
		heartUI.sprite = heartsSprites[gm.getPlayerLifePoints()];
	}
}
