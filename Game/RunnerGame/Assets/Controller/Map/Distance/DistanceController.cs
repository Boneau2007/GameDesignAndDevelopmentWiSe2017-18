using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks and exposes the distance covered during a run.
/// </summary>
public class DistanceController : MonoBehaviour {

    /// <summary>
    /// Distance currently recorded for the run, in metres.
    /// </summary>
    [SerializeField]
    private int distance;
    /// <summary>
    /// Game manager used to read the run state and player and camera positions.
    /// </summary>
    [SerializeField]
    private GameManager gameManager;

    /// <summary>
    /// Finds the game manager used to calculate distance.
    /// </summary>
    void Start () {
        gameManager = FindObjectOfType<GameManager>();
    }
	
	/// <summary>
	/// Updates the recorded distance while the run has not started.
	/// </summary>
	void Update () {
	    startCalculatingDistance();
    }

    /// <summary>
    /// Calculates distance from the player's and camera's horizontal positions while the run is inactive.
    /// </summary>
    public void startCalculatingDistance()
    {
        if (!gameManager.getGameStatus())
        {
            int currentDistance = (int)gameManager.getPlayerPositionX() +
                                 ((int)gameManager.getCameraPositionX() - (int)gameManager.getPlayerPositionX());
            setDistance(currentDistance);
        }
    }
    /// <summary>
    /// Gets the currently recorded distance.
    /// </summary>
    /// <returns>The distance in metres.</returns>
    public int getDistance()
    {
        return distance;
    }

    /// <summary>
    /// Sets the currently recorded distance.
    /// </summary>
    /// <param name="distance">The distance in metres.</param>
    public void setDistance(int distance)
    {
        this.distance = distance;
    }
}