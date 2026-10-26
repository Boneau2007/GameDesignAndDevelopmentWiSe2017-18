using UnityEngine;

/// <summary>
/// Displays the character selector in the main menu and saves the selected character.
/// </summary>
public class CharacterSelectionMenu : MonoBehaviour
{
    /// <summary>
    /// PlayerPrefs key used to persist the selected character index.
    /// </summary>
    public const string PreferenceKey = "SelectedChristmasCharacter";

    private static readonly string[] CharacterNames =
    {
        "Santa",
        "Peppermint Elf",
        "Snowflake Scout",
        "Gingerbread Runner",
        "Reindeer Dash"
    };

    private static readonly string[] CharacterDescriptions =
    {
        "The classic gift-giving hero.",
        "A nimble helper in peppermint green.",
        "A cozy explorer of the winter trails.",
        "A sweet sprinter with frosted icing.",
        "A speedy friend with a bright red nose."
    };

    private int selectedCharacter;
    private Sprite previewSprite;
    private GameObject mainMenuPanel;

    /// <summary>
    /// Loads the saved selection and locates the main-menu panel.
    /// </summary>
    private void Start()
    {
        selectedCharacter = Mathf.Clamp(PlayerPrefs.GetInt(PreferenceKey, 0), 0, CharacterNames.Length - 1);
        mainMenuPanel = GameObject.Find("MainMenuPanel");
        UpdatePreview();
    }

    /// <summary>
    /// Draws the character preview, description, and selection buttons.
    /// </summary>
    private void OnGUI()
    {
        if (mainMenuPanel == null || !mainMenuPanel.activeInHierarchy)
        {
            return;
        }

        float panelWidth = Mathf.Min(300f, Screen.width - 24f);
        float panelHeight = 190f;
        float x = Screen.width - panelWidth - 12f;
        const float y = 12f;

        GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
        GUI.Label(new Rect(x + 12f, y + 8f, panelWidth - 24f, 24f), "CHOOSE YOUR RUNNER");

        if (previewSprite != null)
        {
            GUI.DrawTexture(new Rect(x + 16f, y + 42f, 80f, 80f), previewSprite.texture, ScaleMode.ScaleToFit, true);
        }
        else
        {
            GUI.Box(new Rect(x + 16f, y + 42f, 80f, 80f), "Santa");
        }

        GUI.Label(new Rect(x + 106f, y + 48f, panelWidth - 118f, 28f), CharacterNames[selectedCharacter]);
        GUI.Label(new Rect(x + 106f, y + 78f, panelWidth - 118f, 54f), CharacterDescriptions[selectedCharacter]);

        float buttonWidth = (panelWidth - 24f) / CharacterNames.Length;
        for (int i = 0; i < CharacterNames.Length; i++)
        {
            string label = i == selectedCharacter ? "[" + (i + 1) + "]" : (i + 1).ToString();
            if (GUI.Button(new Rect(x + 12f + buttonWidth * i, y + 137f, buttonWidth - 3f, 28f), label))
            {
                SelectCharacter(i);
            }
        }

        GUI.Label(new Rect(x + 12f, y + 166f, panelWidth - 24f, 18f), "Select a number or use the buttons.");
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
            ? null
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
    /// Handles numeric keyboard shortcuts while the main-menu panel is active.
    /// </summary>
    private void Update()
    {
        if (mainMenuPanel == null || !mainMenuPanel.activeInHierarchy)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectCharacter(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SelectCharacter(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SelectCharacter(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SelectCharacter(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) SelectCharacter(4);
    }
}
