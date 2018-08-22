using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

	[SerializeField]
    private float cameraSpeed;
	[SerializeField]
	private float speedMultiplier;
	private int limit;
	private bool cameraMoves;
	public Rigidbody2D myCamera;
	[SerializeField]
	private GameManager gameManager;

    // Use this for initialization
    void Start () {
		gameManager = FindObjectOfType<GameManager> ();
		myCamera = GetComponent<Rigidbody2D> ();
		toggleCameraMovement (false);
		speedMultiplier = 1f;
		limit = 2000;
		cameraMoves = false; 

	}

    // Update is called once per frame
	void FixedUpdate () {
		
		// Move Camera
		moveCamera ();
		increaseSpeed ();
    }

	void Update() {
		checkGameStatusAndStartGame ();	

	}

	/// <summary>
	/// Checks the game status and start game. Gamestatus is handled by the GameManager.
	/// if the player dies, then the camera stops moving.
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
	/// Toggles the camera movement. When the player dies, the Camera will stop. 
	/// </summary>
	/// <param name="shouldMove">If set to <c>true</c> should move.</param>
	void toggleCameraMovement(bool shouldMove) {
		if (shouldMove) {
			cameraSpeed = 3.5f * speedMultiplier;
		} else {
			cameraSpeed = 0.0f;
		}
	}
	
    /// <summary>
    /// Gets the Position in x direction. 
    /// The method is called from the GameManager and grants access to the camera position.
    /// </summary>
    /// <returns>The Position in X.</returns>
    public float getPositionX(){
        return myCamera.position.x;
    }

    /// <summary>
    /// Gets the Position in y direction. 
    /// The method is called from the GameManager and grants access to the camera position.
    /// </summary>
    /// <returns>The Position in Y.</returns>
    public float getPositionY()
    {
        return myCamera.position.y;
}

	/// <summary>
	/// Gets the speed. The method is called from the GameManager and grants access to the camera Speed.
	/// </summary>
	/// <returns>The speed.</returns>
    public float getSpeed() {
        return cameraSpeed;
    }

	/// <summary>
	/// Moves the camera. The actual function that moves the camera.
	/// Needs access to the RidgedBody of the Camera.
	/// </summary>
	private void moveCamera(){
		myCamera.velocity = new Vector2 (cameraSpeed, 0);
	}


	/// <summary>
	/// Increases the speed depending on the score.
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
