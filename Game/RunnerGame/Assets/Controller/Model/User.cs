using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores a user's display name and score.
/// </summary>
public class User {
	/// <summary>
	/// Creates a user with the specified name and an initial score of zero.
	/// </summary>
	/// <param name="name">The user's display name.</param>
	/// <remarks>The score is initialized by the constructor.</remarks>
	/// <example><code>var user = new User("Player");</code></example>
	/// <seealso cref="getName"/>
	/// <seealso cref="getScore"/>
	/// <seealso cref="setName(string)"/>
	/// <seealso cref="setScore(int)"/>
	public User(string name){
		this.setName (name);
		this.setScore (0);
	}

	[SerializeField]
	private string name;
	[SerializeField]
	private int score;

	/// <summary>
	/// Sets the user's score to zero.
	/// </summary>
	void Start () {
		score = 0;
	}

	/// <summary>
	/// Gets the user's display name.
	/// </summary>
	/// <returns>The current display name.</returns>
	public string getName(){
		return this.name; 
	}

	/// <summary>
	/// Gets the user's score.
	/// </summary>
	/// <returns>The current score.</returns>
	public int getScore(){
		return this.score;
	}

	/// <summary>
	/// Sets the user's display name.
	/// </summary>
	/// <param name="name">The new display name.</param>
	public void setName(string name){
		this.name = name;
	}

	/// <summary>
	/// Sets the user's score.
	/// </summary>
	/// <param name="score">The new score.</param>
	public void setScore(int score){
		this.score = score;
	}
}
