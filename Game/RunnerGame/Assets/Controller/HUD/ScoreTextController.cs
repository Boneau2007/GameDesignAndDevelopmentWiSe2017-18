using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Shows the current run score as text in the HUD.
/// </summary>
public class ScoreTextController : MonoBehaviour {

	private GameManager gm;
	/// <summary>
	/// Text element used to display the current score.
	/// </summary>
	public Text scoreTextObject;

	/// <summary>
	/// Finds the game manager that provides the current score.
	/// </summary>
	void Start () {
		gm = FindObjectOfType<GameManager> ();
		if (scoreTextObject == null)
		{
			scoreTextObject = GetComponent<Text>();
		}

		if (scoreTextObject == null)
		{
			Debug.LogError("ScoreTextController requires a score Text component.");
			enabled = false;
			return;
		}

		GameObject hudObject = GameObject.Find("HUD");
		if (hudObject == null)
		{
			Debug.LogError("ScoreTextController could not find the active HUD Canvas.");
			return;
		}

		GameUITheme.ConfigureScoreHud(hudObject.transform, scoreTextObject);
	}
	
	/// <summary>
	/// Refreshes the displayed score.
	/// </summary>
	void Update () {
		showScore ();
	}

	/// <summary>
	/// Writes the current score to the HUD text element.
	/// </summary>
	void showScore() {
		if (gm != null && scoreTextObject != null)
		{
			scoreTextObject.text = Localization.Format ("hud.score", gm.getScore ());
		}
	}

}
