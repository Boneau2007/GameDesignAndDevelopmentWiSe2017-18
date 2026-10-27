using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Coordinates the run state and exposes game data to other gameplay components.
/// </summary>
public class GameManager : MonoBehaviour {

	private const string HighScoreKey = "SantaChristmasRunHighScore";
	private CameraController cameraController;
	private PlayerController playerController;
	[SerializeField]
	private ScoreController scoreController;
	private PlatformGenerator platformGenerator;
	private DistanceController distanceController;
	private HazardController hazardController;
	private bool started; 
	private bool paused;
	private bool gameOverHandled;
	private bool achievedNewHighScore;

	/// <summary>
	/// Canvas shown when the player loses the run.
	/// </summary>
	public Canvas gameOverCanvas;
	/// <summary>
	/// Gets the active scene's game manager instance.
	/// </summary>
	public static GameManager instance;

	/// <summary>
	/// Registers this object as the active manager and removes duplicate instances.
	/// </summary>
	void Awake(){
		if (instance == null) {
			instance = this; 
		}else if (instance != this){
			Destroy (gameObject);
			return;
		}

	}


	/// <summary>
	/// Finds the scene's gameplay components and initializes the run state.
	/// </summary>
	void Start () {
		cameraController = FindObjectOfType<CameraController> ();
		playerController = FindObjectOfType<PlayerController> ();
		scoreController = FindObjectOfType<ScoreController> ();
		platformGenerator = FindObjectOfType<PlatformGenerator> ();
	    distanceController = FindObjectOfType<DistanceController>();
		hazardController = FindObjectOfType<HazardController>();
	    GameObject hudObject = GameObject.Find("HUD");
	    if (hudObject != null) {
	    	GameUITheme.ConfigureGameplayCanvas(hudObject.transform);
	    }
	    Time.timeScale = 1f;
	    if (gameOverCanvas != null) {
	    	gameOverCanvas.enabled = false;
	    }
	    started = false; 
	    paused = false;
	    gameOverHandled = false;
	}
	
	/// <summary>
	/// Processes game-over controls, pause input, and the initial start input.
	/// </summary>
	void Update () {
	    if (gameOverHandled) {
	    	if (Input.GetKeyDown (KeyCode.R)) {
	    		RestartRun ();
	    	} else if (Input.GetKeyDown (KeyCode.M)) {
	    		ReturnToMainMenu ();
	    	}
	    	return;
	    }

	    if (!isPlayerAlive ()) {
	    	HandleGameOver ();
	    	return;
	    }

	    if (Input.GetKeyDown (KeyCode.Escape)) {
	    	if (paused) {
	    		ResumeGame ();
	    	} else if (started) {
	    		PauseGame ();
	    	}
	    	return;
	    }

	    if (paused) {
	    	return;
	    }

	    GameStatus ();
	}

	/// <summary>
	/// Delegates pause-menu rendering to the shared UI theme.
	/// </summary>
	private void OnGUI () {
	    if (paused) {
	    	GameUITheme.DrawPauseMenu(Localization.Get("pause.title"),
	    		Localization.Get("pause.continue.hint"),
	    		Localization.Get("pause.continue"),
	    		Localization.Get("pause.restart"),
	    		Localization.Get("pause.menu"),
	    		ResumeGame, RestartRun, ReturnToMainMenu);
	    }
	}

	/// <summary>
	/// Restores normal time when this manager is disabled and releases its singleton reference.
	/// </summary>
	private void OnDisable () {
	    Time.timeScale = 1f;
	    paused = false;
	    if (instance == this) {
	    	instance = null;
	    }
	}

	/// <summary>
	/// Gets the current horizontal camera speed.
	/// </summary>
	/// <returns>The camera speed in world units per second.</returns>
	public float getCameraSpeed() {
		return cameraController.getSpeed ();
	}


	/// <summary>
	/// Gets the player's current horizontal movement speed.
	/// </summary>
	/// <returns>The player speed in world units per second.</returns>
	public float getPlayerSpeed() {
		return playerController.getSpeed ();
	}
    /// <summary>
    /// Gets the camera's horizontal world position.
    /// </summary>
    /// <returns>The camera's X coordinate.</returns>
    public float getCameraPositionX()
    {
        return cameraController.getPositionX();
    }

    /// <summary>
    /// Gets the camera's vertical world position.
    /// </summary>
    /// <returns>The camera's Y coordinate.</returns>
    public float getCameraPositionY()
    {
        return cameraController.getPositionY();
    }

    /// <summary>
    /// Gets the player's horizontal world position.
    /// </summary>
    /// <returns>The player's X coordinate.</returns>
    public float getPlayerPositionX()
    {
        return playerController.getPositionX();
    }

    /// <summary>
    /// Gets the player's vertical world position.
    /// </summary>
    /// <returns>The player's Y coordinate.</returns>
    public float getPlayerPositionY()
    {
        return playerController.getPositionY();
}
	/// <summary>
	/// Gets the player life points.
	/// </summary>
	/// <returns>The player life points.</returns>
	/// <summary>
	/// Gets the player's remaining life points.
	/// </summary>
	/// <returns>The current number of life points.</returns>
	public int getPlayerLifePoints() {
		return playerController.getLifePoints ();
	}

	/// <summary>
	/// Gets the score earned during the current run.
	/// </summary>
	/// <returns>The current score in points.</returns>
	public int getScore() {
		return scoreController.getScore ();
	}

	/// <summary>
	/// Gets the distance recorded for the current run.
	/// </summary>
	/// <returns>The recorded distance in metres.</returns>
	public int getDistance() {
		return distanceController.getDistance();
	}


	/// <summary>
	/// Starts the run after player input and clears the running state if the player has died.
	/// </summary>
	private void GameStatus(){
		if (Input.anyKeyDown && !started) {
			started = true;
		}
		if (!isPlayerAlive ()) {
			started = false;		
		}
	}

	/// <summary>
	/// Reports whether the current run has started.
	/// </summary>
	/// <returns><see langword="true"/> if the run has started; otherwise, <see langword="false"/>.</returns>
	public bool getGameStatus(){
		return this.started;
	}

	/// <summary>
	/// Reports whether gameplay systems should update during an active run.
	/// </summary>
	/// <returns><see langword="true"/> while the run is active and unpaused.</returns>
	public bool isGameplayActive(){
		return started && !paused && !gameOverHandled && Time.timeScale > 0f;
	}

	/// <summary>
	/// Reports whether the player is still alive.
	/// </summary>
	/// <returns><see langword="true"/> if the player is alive; otherwise, <see langword="false"/>.</returns>
	public bool isPlayerAlive(){
		return playerController.isPlayerAlive ();
	}


	/// <summary>
	/// Gets the most recently selected platform from the platform generator.
	/// </summary>
	/// <returns>The last platform object.</returns>
	public GameObject getLastPlatform(){
		return platformGenerator.getLastPlatform ();
	}

	/// <summary>
	/// Adds points earned by collecting an item to the current score.
	/// </summary>
	/// <param name="points">The number of points to add.</param>
	public void addPointsFromCollectable(int points){
		scoreController.addPointsFromCollectable (points);
	}

	/// <summary>
	/// Ends and freezes the run, records a new high score if applicable, and displays the existing game-over canvas.
	/// </summary>
	private void HandleGameOver () {
		gameOverHandled = true;
		if (distanceController != null) {
			distanceController.FinalizeDistance ();
		}
		started = false;
		paused = false;
		Time.timeScale = 0f;
		int score = getScore ();
		int highScore = PlayerPrefs.GetInt (HighScoreKey, 0);
		achievedNewHighScore = score > highScore;
		if (achievedNewHighScore) {
			PlayerPrefs.SetInt (HighScoreKey, score);
			PlayerPrefs.Save ();
		}
		if (gameOverCanvas != null) {
			gameOverCanvas.enabled = true;
		}
		GameOverCanvasController gameOverController = GetComponent<GameOverCanvasController> ();
		if (gameOverController != null) {
			gameOverController.ShowResults (score, getDistance (), achievedNewHighScore);
		}
	}

	/// <summary>
	/// Pauses gameplay by setting the time scale to zero.
	/// </summary>
	private void PauseGame () {
		paused = true;
		Time.timeScale = 0f;
	}

	/// <summary>
	/// Resumes gameplay at the normal time scale.
	/// </summary>
	private void ResumeGame () {
		paused = false;
		Time.timeScale = 1f;
	}

	/// <summary>
	/// Reloads the currently active scene to begin a new run.
	/// </summary>
	private void RestartRun () {
		Time.timeScale = 1f;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	/// <summary>
	/// Loads the main menu scene.
	/// </summary>
	public void ReturnToMainMenu () {
		Time.timeScale = 1f;
		SceneManager.LoadScene ("mainmenu");
	}
		
}
