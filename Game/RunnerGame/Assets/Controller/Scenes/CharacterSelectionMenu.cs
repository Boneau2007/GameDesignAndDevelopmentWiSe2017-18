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

    private readonly string[] localizedCharacterNames = new string[CharacterNames.Length];
    private readonly string[] localizedCharacterDescriptions = new string[CharacterDescriptions.Length];
    private readonly Dictionary<Text, string> menuTexts = new Dictionary<Text, string>();
    private int selectedCharacter;
    private Sprite previewSprite;
    private GameObject mainMenuPanel;
    private Canvas menuCanvas;
    private Text menuLanguageText;
    private bool selectingCharacter;
    private bool showingLanguageSelector;
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
            : FindAnyObjectByType<Canvas>();
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
        if (showingLanguageSelector && !selectingCharacter)
        {
            DrawLanguageSelector();
            return;
        }

        if (!selectingCharacter)
        {
            return;
        }

        GameUITheme.DrawCharacterSelection(selectedCharacter, previewSprite,
            Localization.Get("selection.title"), localizedCharacterNames, localizedCharacterDescriptions,
            Localization.Get("selection.start"), Localization.Get("menu.back"),
            Localization.Get("selection.hint"), SelectCharacter, StartRun, ReturnToMainMenu);
    }

    private void DrawLanguageSelector()
    {
        languageScrollPosition = GameUITheme.DrawLanguageSelector(availableLanguages,
            Localization.CurrentLanguageCode, Localization.Get("language.title"),
            Localization.Get("menu.back"), languageScrollPosition,
            code => { Localization.SetLanguage(code); }, ReturnFromLanguageSelector);
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
        Text[] sceneTexts = FindObjectsByType<Text>(FindObjectsInactive.Include);
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
        for (int i = 0; i < CharacterNames.Length; i++)
        {
            localizedCharacterNames[i] = Localization.Get(CharacterNames[i]);
            localizedCharacterDescriptions[i] = Localization.Get(CharacterDescriptions[i]);
        }

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
