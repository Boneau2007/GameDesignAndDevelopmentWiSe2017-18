using UnityEngine;

/// <summary>
/// Tracks and exposes the distance covered during a run.
/// </summary>
public class DistanceController : MonoBehaviour
{

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
    private float startingCameraPositionX;
    private bool runHasStarted;

    /// <summary>
    /// Finds the game manager used to calculate distance.
    /// </summary>
    void Start()
    {
        if (gameManager == null)
        {
			gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    /// <summary>
    /// Updates the recorded distance from camera travel during the active run.
    /// </summary>
    private void Update()
    {
        startCalculatingDistance();
    }

    /// <summary>
    /// Calculates distance from camera travel while the run is active.
    /// </summary>
    public void startCalculatingDistance()
    {
        if (gameManager == null || !gameManager.getGameStatus())
        {
            return;
        }

        float currentCameraPositionX = gameManager.getCameraPositionX();
        if (!runHasStarted)
        {
            startingCameraPositionX = currentCameraPositionX;
            runHasStarted = true;
        }

        RefreshDistance(currentCameraPositionX);
    }

    /// <summary>
    /// Captures the latest camera position as the final run distance.
    /// </summary>
    public void FinalizeDistance()
    {
        if (runHasStarted && gameManager != null)
        {
            RefreshDistance(gameManager.getCameraPositionX());
        }
    }

    private void RefreshDistance(float currentCameraPositionX)
    {
        int currentDistance = Mathf.FloorToInt(Mathf.Max(0f, currentCameraPositionX - startingCameraPositionX));
        setDistance(currentDistance);
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