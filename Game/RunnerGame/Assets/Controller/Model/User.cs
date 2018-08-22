using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User {
	[SerializeField]
	private string name;
	[SerializeField]
	private int score;

	// Use this for initialization
	void Start () {
		score = 0;
	}

	public User(string name){
		this.setName (name);
		this.setScore (0);
	}

	public string getName(){
		return this.name; 
	}

	public int getScore(){
		return this.score;
	}

	public void setName(string name){
		this.name = name;
	}

	public void setScore(int score){
		this.score = score;
	}
}
