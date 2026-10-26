using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Shows the current run score as text in the HUD.
/// </summary>
public class ScoreTextController : MonoBehaviour {

	private GameManager gm;
	private RectTransform scorePanel;
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

		ConfigureScorePanel();
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

	private void ConfigureScorePanel()
	{
		GameObject hudObject = GameObject.Find("HUD");
		if (hudObject == null)
		{
			Debug.LogError("ScoreTextController could not find the active HUD Canvas.");
			return;
		}

		Transform hud = hudObject.transform;
		scoreTextObject.transform.SetParent(hud, false);

		GameObject panelObject = new GameObject("ScorePanel", typeof(RectTransform), typeof(CanvasRenderer),
			typeof(Image));
		panelObject.transform.SetParent(hud, false);
		panelObject.transform.SetAsLastSibling();
		scorePanel = panelObject.GetComponent<RectTransform>();
		scorePanel.anchorMin = Vector2.one;
		scorePanel.anchorMax = Vector2.one;
		scorePanel.pivot = Vector2.one;
		scorePanel.anchoredPosition = new Vector2(-24f, -24f);
		scorePanel.sizeDelta = new Vector2(400f, 84f);

		Image panelImage = panelObject.GetComponent<Image>();
		GameUITheme.ConfigurePanel(panelImage, GameUITheme.ForestGreen);

		scoreTextObject.rectTransform.SetParent(scorePanel, false);
		RectTransform textRect = scoreTextObject.rectTransform;
		GameUITheme.ConfigureText(scoreTextObject, new Vector2(376f, 76f), Vector2.zero,
			GameUITheme.HudFontSize, FontStyle.Bold, GameUITheme.WarmCream, TextAnchor.MiddleCenter);
		textRect.anchorMin = Vector2.zero;
		textRect.anchorMax = Vector2.one;
		textRect.offsetMin = new Vector2(12f, GameUITheme.ItemPadding);
		textRect.offsetMax = new Vector2(-12f, -GameUITheme.ItemPadding);
		scoreTextObject.transform.SetAsLastSibling();
	}
}
