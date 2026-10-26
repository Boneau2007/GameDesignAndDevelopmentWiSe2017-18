using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Presents the final run results on the existing game-over canvas.
/// </summary>
public class GameOverCanvasController : MonoBehaviour
{
    private static readonly Color ForestGreen = new Color(0.055f, 0.18f, 0.14f, 0.97f);
    private static readonly Color WarmCream = new Color(0.96f, 0.89f, 0.72f, 1f);
    private static readonly Color DeepGreen = new Color(0.10f, 0.25f, 0.18f, 1f);
    private static readonly Color ChristmasRed = new Color(0.66f, 0.12f, 0.14f, 1f);
    private static readonly Color ButtonGreen = new Color(0.15f, 0.38f, 0.27f, 1f);

    /// <summary>
    /// UI text element used to display the final score.
    /// </summary>
    public Text scoreText;

    /// <summary>
    /// UI text element used to display the distance travelled.
    /// </summary>
    public Text distanceText;

    private Canvas gameOverCanvas;
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
        GameManager manager = GetComponent<GameManager>();
        if (manager != null)
        {
            gameOverCanvas = manager.gameOverCanvas;
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
            titleText.color = achievedNewHighScore ? new Color(1f, 0.79f, 0.34f) : WarmCream;
        }

        SetButtonLabel("startButton", Localization.Get("gameover.retry"));
        SetButtonLabel("quitButton", Localization.Get("gameover.quit"));
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
        SetRectTransform(panelRect, new Vector2(620f, 560f), Vector2.zero);

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
        if (panelImage != null)
        {
            panelImage.color = ForestGreen;
            panelImage.raycastTarget = true;
        }

        Transform summaryPanel = panel.Find("RunSummaryPanel");
        if (summaryPanel != null)
        {
            Image summaryPanelImage = summaryPanel.GetComponent<Image>();
            if (summaryPanelImage != null)
            {
                summaryPanelImage.color = WarmCream;
                summaryPanelImage.raycastTarget = false;
            }

            SetRectTransform(summaryPanel as RectTransform, new Vector2(460f, 170f), new Vector2(0f, 24f));
            summaryPanel.SetAsFirstSibling();
        }
        else
        {
            Debug.LogError("GameOverPanel is missing its RunSummaryPanel.");
        }

        titleText = FindText(panel, "GameOverText");
        scoreText = scoreText != null ? scoreText : FindText(panel, "ScoreText");
        distanceText = distanceText != null ? distanceText : FindText(panel, "DistanceText");

        ConfigureText(titleText, new Vector2(540f, 74f), new Vector2(0f, 205f),
            46, FontStyle.Bold, WarmCream, TextAnchor.MiddleCenter);
        ConfigureText(scoreText, new Vector2(420f, 48f), new Vector2(0f, 54f),
            28, FontStyle.Bold, DeepGreen, TextAnchor.MiddleCenter);
        ConfigureText(distanceText, new Vector2(420f, 42f), new Vector2(0f, 5f),
            23, FontStyle.Normal, DeepGreen, TextAnchor.MiddleCenter);

        ConfigureButton(panel, "startButton", Localization.Get("gameover.retry"), new Vector2(-118f, -195f), ChristmasRed);
        ConfigureButton(panel, "quitButton", Localization.Get("gameover.quit"), new Vector2(118f, -195f), ButtonGreen);
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

    private static void ConfigureText(Text text, Vector2 size, Vector2 position, int fontSize,
        FontStyle fontStyle, Color color, TextAnchor alignment)
    {
        if (text == null)
        {
            Debug.LogError("Game-over screen is missing a required text element.");
            return;
        }

        SetRectTransform(text.rectTransform, size, position);
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = Mathf.Max(14, fontSize - 6);
        text.resizeTextMaxSize = fontSize;
        text.raycastTarget = false;
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

        SetRectTransform(buttonTransform as RectTransform, new Vector2(220f, 60f), position);

        Button button = buttonTransform.GetComponent<Button>();
        Image image = buttonTransform.GetComponent<Image>();
        if (image != null)
        {
            image.color = backgroundColor;
        }

        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.91f, 0.7f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            button.colors = colors;
        }

        Text buttonText = FindText(buttonTransform, "Text");
        ConfigureText(buttonText, new Vector2(220f, 60f), Vector2.zero,
            22, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
        if (buttonText != null)
        {
            buttonText.text = label;
        }
    }

    private static void SetRectTransform(RectTransform rectTransform, Vector2 size, Vector2 position)
    {
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;
        rectTransform.localScale = Vector3.one;
    }
}
