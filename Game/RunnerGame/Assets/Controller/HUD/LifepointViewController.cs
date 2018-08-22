using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LifepointViewController : MonoBehaviour {

	private GameManager gm;
	public Text lifepointsObject;


	// Use this for initialization
	void Start () {
		gm = FindObjectOfType<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		ShowLifePoints ();
	}

	/// <summary>
	/// Shows the life points.
	/// </summary>
	void ShowLifePoints() {
		lifepointsObject.text = "Lifepoints: " + gm.getPlayerLifePoints ();
	}
}
