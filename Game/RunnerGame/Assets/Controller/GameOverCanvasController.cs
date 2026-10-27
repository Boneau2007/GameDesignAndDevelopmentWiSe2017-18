using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Presents the final run results on the existing game-over canvas.
/// </summary>
public class GameOverCanvasController : MonoBehaviour
{
    /// <summary>
    /// UI text element used to display the final score.
    /// </summary>
    public Text scoreText;

    /// <summary>
    /// UI text element used to display the distance travelled.
    /// </summary>
    public Text distanceText;

    private Canvas gameOverCanvas;
    private GameManager gameManager;
    private Text titleText;
    private int finalScore;
    private int finalDistance;
    private bool achievedNewHighScore;
    private bool hasResults;

    /// <summary>
    /// Finds the existing game-over elements and applies the shared winter theme.
    /// </summary>
    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        if (gameManager != null)
        {
            gameOverCanvas = gameManager.gameOverCanvas;
        }

        if (gameOverCanvas == null)
        {
            Debug.LogError("GameOverCanvasController could not find the game-over canvas.");
            return;
        }

        Transform panel = gameOverCanvas.transform.Find("GameOverPanel");
        if (panel == null)
        {
            Debug.LogError("GameOverCanvasController could not find GameOverPanel.");
            return;
        }

        ConfigurePanel(panel);
        Localization.LanguageChanged += RefreshLocalizedResults;
        RefreshLocalizedResults();
    }

    /// <summary>
    /// Updates the existing result labels when the run ends.
    /// </summary>
    /// <param name="score">The final score in points.</param>
    /// <param name="distance">The final distance in metres.</param>
    /// <param name="achievedNewHighScore">Whether this run set a new high score.</param>
    public void ShowResults(int score, int distance, bool achievedNewHighScore)
    {
        finalScore = score;
        finalDistance = distance;
        this.achievedNewHighScore = achievedNewHighScore;
        hasResults = true;
        RefreshLocalizedResults();
    }

    private void RefreshLocalizedResults()
    {
        if (titleText == null)
        {
            return;
        }

        if (hasResults)
        {
            if (scoreText != null)
            {
                scoreText.text = Localization.Format("gameover.score", finalScore);
            }

            if (distanceText != null)
            {
                distanceText.text = Localization.Format("gameover.distance", finalDistance);
            }

            titleText.text = Localization.Get(achievedNewHighScore ? "gameover.new_record" : "gameover.title");
            GameUITheme.SetTextColor(titleText,
                achievedNewHighScore ? GameUITheme.AccentGold : GameUITheme.WarmCream);
        }

        SetButtonLabel("startButton", Localization.Get("gameover.retry"));
        SetButtonLabel("quitButton", Localization.Get("gameover.quit"));
        SetButtonLabel("mainMenuButton", Localization.Get("gameover.menu"));
    }

    private void OnDestroy()
    {
        Localization.LanguageChanged -= RefreshLocalizedResults;
    }

    private void SetButtonLabel(string buttonName, string label)
    {
        Transform button = gameOverCanvas.transform.Find("GameOverPanel/" + buttonName);
        Text buttonText = button != null ? FindText(button, "Text") : null;
        if (buttonText != null)
        {
            buttonText.text = label;
        }
    }

    private void ConfigurePanel(Transform panel)
    {
        Transform summaryPanel = panel.Find("RunSummaryPanel");
        titleText = FindText(panel, "GameOverText");
        scoreText = FindText(panel, "ScoreText") ?? scoreText;
        distanceText = FindText(panel, "DistanceText") ?? distanceText;
        if (summaryPanel != null)
        {
            if (scoreText != null)
            {
                scoreText.transform.SetParent(summaryPanel, false);
            }

            if (distanceText != null)
            {
                distanceText.transform.SetParent(summaryPanel, false);
            }
        }

        EnsureMainMenuButton(panel);
        if (titleText != null)
        {
            titleText.text = Localization.Get("gameover.title");
        }

        GameUITheme.ConfigureGameOverLayout(panel, summaryPanel, titleText, scoreText, distanceText,
            FindButton(panel, "startButton"), FindButton(panel, "quitButton"),
            FindButton(panel, "mainMenuButton"));
        if (summaryPanel != null)
        {
            summaryPanel.SetAsFirstSibling();
        }
    }

    private static Text FindText(Transform parent, string objectName)
    {
        Transform child = parent.Find(objectName);
        return child != null ? child.GetComponent<Text>() : null;
    }

    private static Button FindButton(Transform parent, string objectName)
    {
        Transform child = parent.Find(objectName);
        return child != null ? child.GetComponent<Button>() : null;
    }

    private void EnsureMainMenuButton(Transform panel)
    {
        if (gameManager == null)
        {
            Debug.LogError("GameOverCanvasController could not find GameManager for its main-menu button.");
            return;
        }

        Transform existingButton = panel.Find("quitButton");
        Button templateButton = existingButton != null ? existingButton.GetComponent<Button>() : null;
        if (templateButton == null)
        {
            Debug.LogError("GameOverCanvasController could not find the quit button to create its main-menu action.");
            return;
        }

        Transform currentButton = panel.Find("mainMenuButton");
        if (currentButton != null)
        {
            Destroy(currentButton.gameObject);
        }

        Button menuButton = GameUITheme.CreateButton(templateButton, panel, "mainMenuButton",
            Localization.Get("gameover.menu"), gameManager.ReturnToMainMenu);
        if (menuButton == null)
        {
            return;
        }
    }
}
