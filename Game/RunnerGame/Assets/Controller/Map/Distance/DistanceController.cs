using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceController : MonoBehaviour {

    [SerializeField]
    private int distance;
    [SerializeField]
    private GameManager gameManager;

    // Use this for initialization
    void Start () {
        gameManager = FindObjectOfType<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
	    startCalculatingDistance();
    }

    /// <summary>
    /// Starts to calculating the distance. Invoked when the game is not started or stopped. 
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
    /// Invoked when the Value of distance need to be called 
    /// </summary>
    public int getDistance()
    {
        return distance;
    }

    /// <summary>
    /// Invoked when the Value of distance need to be set
    /// </summary>
    public void setDistance(int distance)
    {
        this.distance = distance;
    }
}