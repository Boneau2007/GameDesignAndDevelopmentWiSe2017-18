using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Moves the camera across the level and increases its speed as the score grows.
/// </summary>
public class CameraController : MonoBehaviour {

	[SerializeField]
    private float cameraSpeed;
	[SerializeField]
	private float speedMultiplier;
	private int limit;
	private bool cameraMoves;
	/// <summary>
	/// Rigidbody used to move the camera horizontally.
	/// </summary>
	public Rigidbody2D myCamera;
	[SerializeField]
	private GameManager gameManager;

    /// <summary>
    /// Finds the game manager and initializes camera movement settings.
    /// </summary>
    void Start () {
		gameManager = FindObjectOfType<GameManager> ();
		myCamera = GetComponent<Rigidbody2D> ();
		toggleCameraMovement (false);
		speedMultiplier = 1f;
		limit = 2000;
		cameraMoves = false; 

	}

    /// <summary>
    /// Moves the camera and checks whether its speed should increase.
    /// </summary>
	void FixedUpdate () {
		
		moveCamera ();
		increaseSpeed ();
    }

	/// <summary>
	/// Starts camera movement when the run begins and stops it when the player dies.
	/// </summary>
	void Update() {
		checkGameStatusAndStartGame ();	

	}

	/// <summary>
	/// Synchronizes camera movement with the run state and player survival state.
	/// </summary>
	void checkGameStatusAndStartGame(){
		if (gameManager.getGameStatus () && !cameraMoves && gameManager.getGameStatus ()) {
			toggleCameraMovement (gameManager.isPlayerAlive());
			cameraMoves = true; 
			}
		if(!gameManager.isPlayerAlive ()){
			toggleCameraMovement (false);
		}

	}

	/// <summary>
	/// Sets the camera's movement speed according to whether it should move.
	/// </summary>
	/// <param name="shouldMove"><see langword="true"/> to enable camera movement; otherwise, <see langword="false"/>.</param>
	void toggleCameraMovement(bool shouldMove) {
		if (shouldMove) {
			cameraSpeed = 3.5f * speedMultiplier;
		} else {
			cameraSpeed = 0.0f;
		}
	}
	
    /// <summary>
    /// Gets the camera's horizontal world position.
    /// </summary>
    /// <returns>The camera's X coordinate.</returns>
    public float getPositionX(){
        return myCamera.position.x;
    }

    /// <summary>
    /// Gets the camera's vertical world position.
    /// </summary>
    /// <returns>The camera's Y coordinate.</returns>
    public float getPositionY()
    {
        return myCamera.position.y;
}

	/// <summary>
	/// Gets the camera's current horizontal speed.
	/// </summary>
	/// <returns>The camera speed in world units per second.</returns>
    public float getSpeed() {
        return cameraSpeed;
    }

	/// <summary>
	/// Applies the current horizontal speed to the camera's rigidbody.
	/// </summary>
	private void moveCamera(){
		myCamera.linearVelocity = new Vector2 (cameraSpeed, 0);
	}


	/// <summary>
	/// Increases camera speed when the score passes the next configured threshold.
	/// </summary>
	private void increaseSpeed(){
		if((gameManager.getScore () / limit) > 0){
			speedMultiplier += 0.2f;
			cameraSpeed = cameraSpeed * speedMultiplier;
			limit *= 10;
			Debug.Log ("Speed increased   " + gameManager.getScore ());
		}
	}
		
}
