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
            titleText.color = achievedNewHighScore ? GameUITheme.AccentGold : GameUITheme.WarmCream;
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
        RectTransform panelRect = panel as RectTransform;
        GameUITheme.SetRectTransform(panelRect, new Vector2(620f, 560f), Vector2.zero);

        VerticalLayoutGroup verticalLayout = panel.GetComponent<VerticalLayoutGroup>();
        if (verticalLayout != null)
        {
            verticalLayout.enabled = false;
        }

        ContentSizeFitter contentSizeFitter = panel.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter != null)
        {
            contentSizeFitter.enabled = false;
        }

        Image panelImage = panel.GetComponent<Image>();
        GameUITheme.ConfigurePanel(panelImage, GameUITheme.ForestGreen);

        Transform summaryPanel = panel.Find("RunSummaryPanel");
        if (summaryPanel != null)
        {
            Image summaryPanelImage = summaryPanel.GetComponent<Image>();
            GameUITheme.ConfigurePanel(summaryPanelImage, GameUITheme.WarmCream);

            GameUITheme.SetRectTransform(summaryPanel as RectTransform, new Vector2(460f, 170f), new Vector2(0f, 24f));
            summaryPanel.SetAsFirstSibling();
        }
        else
        {
            Debug.LogError("GameOverPanel is missing its RunSummaryPanel.");
        }

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

        GameUITheme.ConfigureText(titleText, new Vector2(540f, 74f), new Vector2(0f, 205f),
            GameUITheme.ScreenTitleFontSize, FontStyle.Bold, GameUITheme.WarmCream, TextAnchor.MiddleCenter);
        GameUITheme.ConfigureText(scoreText, new Vector2(420f, 48f), new Vector2(0f, 31f),
            GameUITheme.ResultFontSize, FontStyle.Bold, GameUITheme.DeepGreen, TextAnchor.MiddleCenter);
        GameUITheme.ConfigureText(distanceText, new Vector2(420f, 44f), new Vector2(0f, -27f),
            GameUITheme.ResultFontSize, FontStyle.Bold, GameUITheme.DeepGreen, TextAnchor.MiddleCenter);

        ConfigureButton(panel, "startButton", Localization.Get("gameover.retry"), new Vector2(0f, -110f),
            GameUITheme.ChristmasRed);
        ConfigureButton(panel, "quitButton", Localization.Get("gameover.quit"), new Vector2(0f, -180f),
            GameUITheme.ButtonGreen);
        EnsureMainMenuButton(panel);
        if (titleText != null)
        {
            titleText.text = Localization.Get("gameover.title");
        }
    }

    private static Text FindText(Transform parent, string objectName)
    {
        Transform child = parent.Find(objectName);
        return child != null ? child.GetComponent<Text>() : null;
    }

    private static void ConfigureButton(Transform panel, string objectName, string label,
        Vector2 position, Color backgroundColor)
    {
        Transform buttonTransform = panel.Find(objectName);
        if (buttonTransform == null)
        {
            Debug.LogError("Game-over screen is missing the " + objectName + " button.");
            return;
        }

        GameUITheme.SetRectTransform(buttonTransform as RectTransform, new Vector2(180f, 60f), position);

        Button button = buttonTransform.GetComponent<Button>();
        Image image = buttonTransform.GetComponent<Image>();
        GameUITheme.ConfigureButton(button, image, backgroundColor);

        Text buttonText = FindText(buttonTransform, "Text");
        GameUITheme.ConfigureButtonText(buttonText,
            GameUITheme.ButtonFontSize, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
        if (buttonText != null)
        {
            buttonText.text = label;
        }
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

        ConfigureButton(panel, "mainMenuButton", Localization.Get("gameover.menu"), new Vector2(0f, -250f),
            GameUITheme.ButtonGreen);
    }
}
