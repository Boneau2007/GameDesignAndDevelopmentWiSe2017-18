using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HeartsController : MonoBehaviour {

	public Sprite[] heartsSprites;
	public Image heartUI;
	private GameManager gm;

	// Use this for initialization
	void Start () {
		gm = FindObjectOfType<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		heartUI.sprite = heartsSprites[gm.getPlayerLifePoints()];
	}
}
