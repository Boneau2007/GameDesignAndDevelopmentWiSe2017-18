using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameOverCanvasController : MonoBehaviour {

	private GameManager gm;
	public Text scoreText;
	public Text distanceText;

	// Use this for initialization
	void Start () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		scoreText.text = "You achieved " + gm.getScore() + " points!";
		distanceText.text = "You travelled " + gm.getDistance () + " m."; 
	}
}
