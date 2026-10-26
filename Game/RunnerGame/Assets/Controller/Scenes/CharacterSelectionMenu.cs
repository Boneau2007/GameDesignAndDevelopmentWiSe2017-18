using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Displays the character selector in the main menu and saves the selected character.
/// </summary>
public class CharacterSelectionMenu : MonoBehaviour
{
    /// <summary>
    /// PlayerPrefs key used to persist the selected character index.
    /// </summary>
    public const string PreferenceKey = "SelectedChristmasCharacter";

    /// <summary>
    /// Sprite used to preview Santa, the default character.
    /// </summary>
    public Sprite santaPreview;

    private static readonly string[] CharacterNames =
    {
        "selection.character.santa",
        "selection.character.elf",
        "selection.character.scout",
        "selection.character.gingerbread",
        "selection.character.reindeer"
    };

    private static readonly string[] CharacterDescriptions =
    {
        "selection.description.santa",
        "selection.description.elf",
        "selection.description.scout",
        "selection.description.gingerbread",
        "selection.description.reindeer"
    };

    private readonly Dictionary<Text, string> menuTexts = new Dictionary<Text, string>();
    private int selectedCharacter;
    private Sprite previewSprite;
    private GameObject mainMenuPanel;
    private Canvas menuCanvas;
    private Text menuLanguageText;
    private bool selectingCharacter;
    private bool showingLanguageSelector;
    private GUIStyle titleStyle;
    private GUIStyle characterNameStyle;
    private GUIStyle descriptionStyle;
    private GUIStyle optionButtonStyle;
    private GUIStyle actionButtonStyle;
    private GUIStyle footerStyle;
    private GUIStyle languageTitleStyle;
    private GUIStyle selectionPanelStyle;
    private GUIStyle selectionCardStyle;
    private GUIStyle languagePanelStyle;
    private GUIStyle languageOptionPanelStyle;
    private GUIStyle languageButtonStyle;
    private GUIStyle languageSelectedStyle;
    private GUIStyle languageNameStyle;
    private GUIStyle flagStyle;
    private IReadOnlyList<Localization.LanguageOption> availableLanguages;
    private Vector2 languageScrollPosition;
    private int lastMenuScreenWidth;
    private int lastMenuScreenHeight;
    private float lastMenuScaleFactor;

    /// <summary>
    /// Loads the saved selection and locates the main-menu panel.
    /// </summary>
    private void Start()
    {
        selectedCharacter = Mathf.Clamp(PlayerPrefs.GetInt(PreferenceKey, 0), 0, CharacterNames.Length - 1);
        mainMenuPanel = GameObject.Find("MainMenuPanel");
        selectingCharacter = false;
        UpdatePreview();
        availableLanguages = Localization.AvailableLanguages;
        menuCanvas = mainMenuPanel != null
            ? mainMenuPanel.GetComponentInParent<Canvas>()
            : FindObjectOfType<Canvas>();
        if (menuCanvas != null)
        {
            GameUITheme.ConfigureMenuCanvas(menuCanvas.transform);
        }
        else
        {
            Debug.LogError("CharacterSelectionMenu could not find the main-menu Canvas to apply the shared theme.");
        }

        CaptureMainMenuTexts();
        AddLanguageNavigationButton();
        if (menuCanvas != null)
        {
            GameUITheme.ConfigureResponsiveMenu(menuCanvas.transform);
            CacheMenuResolution();
        }
        Localization.LanguageChanged += ApplyMainMenuLocalization;
        ApplyMainMenuLocalization();
    }

    /// <summary>
    /// Opens character selection after the main menu's Start button is clicked.
    /// </summary>
    public void BeginSelection()
    {
        selectingCharacter = true;
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Draws the character selection screen after the player chooses Start.
    /// </summary>
    private void OnGUI()
    {
        EnsureStyles();
        if (showingLanguageSelector && !selectingCharacter)
        {
            DrawLanguageSelector();
            return;
        }

        if (!selectingCharacter)
        {
            return;
        }

        float panelWidth = Mathf.Min(360f, Screen.width - 24f);
        const float panelHeight = 260f;
        float x = (Screen.width - panelWidth) * 0.5f;
        float y = (Screen.height - panelHeight) * 0.5f;

        Color previousBackgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.white;
        GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty, selectionPanelStyle);
        GUI.Box(new Rect(x + 12f, y + 42f, panelWidth - 24f, 106f), string.Empty, selectionCardStyle);
        GUI.Label(new Rect(x + 12f, y + 10f, panelWidth - 24f, 24f),
            Localization.Get("selection.title"), titleStyle);

        if (previewSprite != null)
        {
            Rect spriteRect = previewSprite.textureRect;
            float aspectRatio = spriteRect.width / spriteRect.height;
            float previewWidth = 88f;
            float previewHeight = 88f;
            if (aspectRatio < 1f)
            {
                previewWidth *= aspectRatio;
            }
            else
            {
                previewHeight /= aspectRatio;
            }

            Rect previewRect = new Rect(
                x + 24f + (88f - previewWidth) * 0.5f,
                y + 46f + (88f - previewHeight) * 0.5f,
                previewWidth,
                previewHeight);
            Rect textureCoordinates = new Rect(
                spriteRect.x / previewSprite.texture.width,
                spriteRect.y / previewSprite.texture.height,
                spriteRect.width / previewSprite.texture.width,
                spriteRect.height / previewSprite.texture.height);
            GUI.DrawTextureWithTexCoords(previewRect, previewSprite.texture, textureCoordinates, true);
        }
        else
        {
            GUI.Label(new Rect(x + 24f, y + 46f, 88f, 88f),
                Localization.Get("selection.character.santa"), characterNameStyle);
        }

        GUI.Label(new Rect(x + 126f, y + 54f, panelWidth - 150f, 28f),
            Localization.Get(CharacterNames[selectedCharacter]), characterNameStyle);
        GUI.Label(new Rect(x + 126f, y + 84f, panelWidth - 150f, 54f),
            Localization.Get(CharacterDescriptions[selectedCharacter]), descriptionStyle);

        float buttonGap = GameUITheme.ItemSpacing;
        float buttonWidth = (panelWidth - 24f - buttonGap * (CharacterNames.Length - 1)) /
                            CharacterNames.Length;
        for (int i = 0; i < CharacterNames.Length; i++)
        {
            string label = i == selectedCharacter ? "[" + (i + 1) + "]" : (i + 1).ToString();
            GUI.backgroundColor = i == selectedCharacter ? GameUITheme.ChristmasRed : GameUITheme.ButtonGreen;
            if (GUI.Button(new Rect(x + 12f + (buttonWidth + buttonGap) * i, y + 154f,
                    buttonWidth, 30f), label,
                    optionButtonStyle))
            {
                SelectCharacter(i);
            }
        }

        GUI.backgroundColor = GameUITheme.ChristmasRed;
        if (GUI.Button(new Rect(x + 12f, y + 202f, panelWidth * 0.5f - 18f, 36f),
                Localization.Get("selection.start"), actionButtonStyle))
        {
            StartRun();
        }
        GUI.backgroundColor = GameUITheme.ButtonGreen;
        if (GUI.Button(new Rect(x + panelWidth * 0.5f + 6f, y + 202f, panelWidth * 0.5f - 18f, 36f),
                Localization.Get("menu.back"),
                actionButtonStyle))
        {
            ReturnToMainMenu();
        }
        GUI.backgroundColor = previousBackgroundColor;
        GUI.Label(new Rect(x + 12f, y + 238f, panelWidth - 24f, 18f),
            Localization.Get("selection.hint"), footerStyle);
    }

    private void DrawLanguageSelector()
    {
        if (availableLanguages == null || availableLanguages.Count == 0)
        {
            return;
        }

        const float buttonHeight = 62f;
        const float buttonGap = GameUITheme.ItemSpacing;
        const float panelPadding = 8f;
        int columns = Mathf.Min(3, availableLanguages.Count);
        int rows = Mathf.CeilToInt(availableLanguages.Count / (float)columns);
        float contentHeight = rows * buttonHeight + (rows - 1) * buttonGap;
        float maxPanelHeight = Mathf.Max(100f, Screen.height - 24f);
        bool needsScroll = 76f + contentHeight > maxPanelHeight;
        float buttonWidth = Mathf.Min(92f,
            (Screen.width - panelPadding * 2f - 16f - (needsScroll ? 16f : 0f) -
             buttonGap * (columns - 1)) / columns);
        buttonWidth = Mathf.Max(54f, buttonWidth);
        float panelWidth = panelPadding * 2f + buttonWidth * columns + buttonGap * (columns - 1) +
                           (needsScroll ? 16f : 0f);
        float panelHeight = Mathf.Min(76f + contentHeight, maxPanelHeight);
        float x = (Screen.width - panelWidth) * 0.5f;
        float y = (Screen.height - panelHeight) * 0.5f;

        Color oldBackground = GUI.backgroundColor;
        GUI.backgroundColor = Color.white;
        GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty, languagePanelStyle);
        GUI.Label(new Rect(x + panelPadding, y + 1f, panelWidth - panelPadding * 2f, 24f),
            Localization.Get("language.title"), languageTitleStyle);

        Rect scrollRect = new Rect(x + panelPadding, y + 24f,
            panelWidth - panelPadding * 2f, panelHeight - 68f);
        Rect contentRect = new Rect(0f, 0f, scrollRect.width - (needsScroll ? 16f : 0f), contentHeight);
        languageScrollPosition = GUI.BeginScrollView(
            scrollRect, languageScrollPosition, contentRect, false, needsScroll);
        for (int i = 0; i < availableLanguages.Count; i++)
        {
            Localization.LanguageOption language = availableLanguages[i];
            int row = i / columns;
            int column = i % columns;
            Rect buttonRect = new Rect(
                column * (buttonWidth + buttonGap),
                row * (buttonHeight + buttonGap),
                buttonWidth,
                buttonHeight);

            bool isSelected = string.Equals(Localization.CurrentLanguageCode, language.Code,
                StringComparison.OrdinalIgnoreCase);
            GUI.backgroundColor = Color.white;
            GUI.Box(buttonRect, string.Empty, isSelected ? languageSelectedStyle : languageOptionPanelStyle);
            if (GUI.Button(buttonRect, GUIContent.none, languageButtonStyle))
            {
                Localization.SetLanguage(language.Code);
            }

            GUI.Label(new Rect(buttonRect.x + 3f, buttonRect.y + 2f, buttonRect.width - 6f, 30f),
                language.Flag, flagStyle);
            GUI.Label(new Rect(buttonRect.x + 2f, buttonRect.y + 31f, buttonRect.width - 4f, 26f),
                language.DisplayName, languageNameStyle);
        }
        GUI.EndScrollView();

        GUI.backgroundColor = GameUITheme.ButtonGreen;
        if (GUI.Button(new Rect(x + panelPadding, y + panelHeight - 38f,
                panelWidth - panelPadding * 2f, 30f), Localization.Get("menu.back"), actionButtonStyle))
        {
            ReturnFromLanguageSelector();
        }

        GUI.backgroundColor = oldBackground;
    }

    private void AddLanguageNavigationButton()
    {
        if (mainMenuPanel == null)
        {
            Debug.LogError("CharacterSelectionMenu could not find MainMenuPanel.");
            return;
        }

        Button[] menuButtons = mainMenuPanel.GetComponentsInChildren<Button>(true);
        if (menuButtons.Length == 0)
        {
            Debug.LogError("CharacterSelectionMenu could not find a button to use for the language menu item.");
            return;
        }

        Button quitButton = FindQuitButton(menuButtons);
        Transform menuButtonParent = quitButton != null ? quitButton.transform.parent : mainMenuPanel.transform;
        Button languageButton = GameUITheme.CreateButton(menuButtons[0], menuButtonParent,
            "LanguageNavigationButton", Localization.Get("menu.language"), OpenLanguageSelector);
        if (languageButton == null)
        {
            return;
        }

        GameUITheme.ConfigureMenuButton(languageButton, GameUITheme.ButtonGreen);
        menuLanguageText = languageButton.GetComponentInChildren<Text>(true);
        if (menuLanguageText == null)
        {
            Debug.LogError("The main-menu button template does not contain a Text component.");
        }

        if (quitButton != null)
        {
            int quitIndex = quitButton.transform.GetSiblingIndex();
            int languageIndex = languageButton.transform.GetSiblingIndex();
            languageButton.transform.SetSiblingIndex(quitIndex);
            quitButton.transform.SetSiblingIndex(languageIndex);
        }
        else
        {
            languageButton.transform.SetAsLastSibling();
        }
    }

    private static Button FindQuitButton(Button[] buttons)
    {
        foreach (Button button in buttons)
        {
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
            {
                if (string.Equals(button.onClick.GetPersistentMethodName(i), "QuitProgramm",
                        StringComparison.Ordinal))
                {
                    return button;
                }
            }
        }

        return null;
    }

    private void OpenLanguageSelector()
    {
        showingLanguageSelector = true;
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
    }

    private void ReturnFromLanguageSelector()
    {
        showingLanguageSelector = false;
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    private void CaptureMainMenuTexts()
    {
        Text[] sceneTexts = FindObjectsOfType<Text>(true);
        foreach (Text text in sceneTexts)
        {
            string key = GetMainMenuTextKey(text.text);
            if (!string.IsNullOrEmpty(key))
            {
                menuTexts[text] = key;
            }
        }
    }

    private void ApplyMainMenuLocalization()
    {
        foreach (KeyValuePair<Text, string> entry in menuTexts)
        {
            if (entry.Key != null)
            {
                entry.Key.text = Localization.Get(entry.Value);
            }
        }

        if (menuLanguageText != null)
        {
            menuLanguageText.text = Localization.Get("menu.language");
        }
    }

    private static string GetMainMenuTextKey(string text)
    {
        string normalized = text.Trim().Trim('\'', '"');
        normalized = string.Join(" ", normalized.Split(new[] { ' ', '\t', '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries));
        if (normalized.StartsWith("menu.", StringComparison.Ordinal))
        {
            return normalized;
        }

        switch (normalized)
        {
            case "Start": return "menu.start";
            case "Quit": return "menu.quit";
            case "Highscore": return "menu.highscore";
            case "About": return "menu.about";
            case "Back": return "menu.back";
            case "Highscorelist": return "menu.highscore.title";
            case "At this Point not Implemented": return "menu.placeholder";
            case "Santa's Christmas Run": return "menu.title";
        }

        if (normalized.StartsWith("This is a project developed by students", StringComparison.OrdinalIgnoreCase))
        {
            return "menu.about.description";
        }

        return null;
    }

    private void OnDestroy()
    {
        Localization.LanguageChanged -= ApplyMainMenuLocalization;
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
        {
            return;
        }

        titleStyle = GameUITheme.CreateLabelStyle(GameUITheme.PanelTitleFontSize, GameUITheme.EmphasisFontStyle,
            TextAnchor.MiddleCenter, GameUITheme.WarmCream);
        characterNameStyle = GameUITheme.CreateLabelStyle(GameUITheme.BodyFontSize + 4, GameUITheme.EmphasisFontStyle,
            TextAnchor.MiddleLeft, GameUITheme.DeepGreen);
        descriptionStyle = GameUITheme.CreateLabelStyle(GameUITheme.BodyFontSize, GameUITheme.BodyFontStyle,
            TextAnchor.UpperLeft, GameUITheme.DeepGreen);
        footerStyle = GameUITheme.CreateLabelStyle(GameUITheme.SmallLabelFontSize, GameUITheme.BodyFontStyle, TextAnchor.MiddleCenter,
            GameUITheme.WarmCream);
        languageTitleStyle = GameUITheme.CreateLabelStyle(GameUITheme.PanelTitleFontSize, GameUITheme.EmphasisFontStyle,
            TextAnchor.MiddleCenter, GameUITheme.WarmCream);

        optionButtonStyle = GameUITheme.CreateButtonStyle(GameUITheme.BodyFontSize);
        actionButtonStyle = GameUITheme.CreateButtonStyle(GameUITheme.ButtonFontSize);
        languagePanelStyle = GameUITheme.CreatePanelStyle();
        selectionPanelStyle = GameUITheme.CreatePanelStyle();
        selectionCardStyle = GameUITheme.CreatePanelStyle(GameUITheme.WarmCream, GameUITheme.DeepGreen);
        languageOptionPanelStyle = GameUITheme.CreatePanelStyle(GameUITheme.ButtonGreen, Color.white);
        languageSelectedStyle = GameUITheme.CreatePanelStyle(GameUITheme.ChristmasRed, Color.white);
        languageButtonStyle = GameUITheme.CreateTransparentButtonStyle(GameUITheme.BodyFontSize);
        languageNameStyle = GameUITheme.CreateLabelStyle(GameUITheme.BodyFontSize, GameUITheme.EmphasisFontStyle,
            TextAnchor.MiddleCenter, Color.white);
        flagStyle = GameUITheme.CreateLabelStyle(GameUITheme.LanguageFlagFontSize, GameUITheme.BodyFontStyle,
            TextAnchor.MiddleCenter, Color.white);
    }

    /// <summary>
    /// Loads the gameplay scene using the saved character selection.
    /// </summary>
    private void StartRun()
    {
        SceneManager.LoadScene("world");
    }

    /// <summary>
    /// Closes character selection and returns to the main menu panel.
    /// </summary>
    private void ReturnToMainMenu()
    {
        selectingCharacter = false;
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Saves the selected character and refreshes the preview.
    /// </summary>
    /// <param name="index">The zero-based index of the character to select.</param>
    private void SelectCharacter(int index)
    {
        selectedCharacter = index;
        PlayerPrefs.SetInt(PreferenceKey, selectedCharacter);
        PlayerPrefs.Save();
        UpdatePreview();
    }

    /// <summary>
    /// Loads the preview sprite for the current character selection.
    /// </summary>
    private void UpdatePreview()
    {
        previewSprite = selectedCharacter == 0
            ? santaPreview
            : Resources.Load<Sprite>("ChristmasCharacters/" +
                                     GetResourceName(selectedCharacter) + "_Idle_0");
    }

    /// <summary>
    /// Maps a character selection index to its resource-name prefix.
    /// </summary>
    /// <param name="index">The zero-based character index.</param>
    /// <returns>The matching resource-name prefix, or an empty string for the default character.</returns>
    private static string GetResourceName(int index)
    {
        switch (index)
        {
            case 1: return "PeppermintElf";
            case 2: return "SnowflakeScout";
            case 3: return "GingerbreadRunner";
            case 4: return "ReindeerDash";
            default: return string.Empty;
        }
    }

    /// <summary>
    /// Handles numeric keyboard shortcuts while character selection is open.
    /// </summary>
    private void Update()
    {
        RefreshResponsiveMenu();
        if (!selectingCharacter)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectCharacter(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SelectCharacter(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SelectCharacter(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SelectCharacter(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) SelectCharacter(4);
    }

    private void RefreshResponsiveMenu()
    {
        if (menuCanvas == null ||
            (lastMenuScreenWidth == Screen.width && lastMenuScreenHeight == Screen.height &&
             Mathf.Approximately(lastMenuScaleFactor, menuCanvas.scaleFactor)))
        {
            return;
        }

        GameUITheme.ConfigureResponsiveMenu(menuCanvas.transform);
        CacheMenuResolution();
    }

    private void CacheMenuResolution()
    {
        lastMenuScreenWidth = Screen.width;
        lastMenuScreenHeight = Screen.height;
        lastMenuScaleFactor = menuCanvas != null ? menuCanvas.scaleFactor : 1f;
    }
}
