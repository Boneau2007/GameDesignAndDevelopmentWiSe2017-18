using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreTextController : MonoBehaviour {

	private GameManager gm;
	public Text scoreTextObject;

	// Use this for initialization
	void Start () {
		gm = FindObjectOfType<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		showScore ();
	}

	/// <summary>
	/// Sets the score text that is displayed in the HUD.
	/// </summary>
	void showScore() {
		scoreTextObject.text = "Score: " + gm.getScore ();
	}
}
