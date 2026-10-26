using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool selectingCharacter;

    /// <summary>
    /// Loads the saved selection and locates the main-menu panel.
    /// </summary>
    private void Start()
    {
        selectedCharacter = Mathf.Clamp(PlayerPrefs.GetInt(PreferenceKey, 0), 0, CharacterNames.Length - 1);
        mainMenuPanel = GameObject.Find("MainMenuPanel");
        selectingCharacter = false;
        UpdatePreview();
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
        if (!selectingCharacter)
        {
            return;
        }

        float panelWidth = Mathf.Min(360f, Screen.width - 24f);
        const float panelHeight = 260f;
        float x = (Screen.width - panelWidth) * 0.5f;
        float y = (Screen.height - panelHeight) * 0.5f;

        GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
        GUI.Label(new Rect(x + 12f, y + 10f, panelWidth - 24f, 24f), "CHOOSE YOUR RUNNER");

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
            GUI.Box(new Rect(x + 24f, y + 46f, 88f, 88f), "Santa");
        }

        GUI.Label(new Rect(x + 126f, y + 54f, panelWidth - 150f, 28f), CharacterNames[selectedCharacter]);
        GUI.Label(new Rect(x + 126f, y + 84f, panelWidth - 150f, 54f), CharacterDescriptions[selectedCharacter]);

        float buttonWidth = (panelWidth - 24f) / CharacterNames.Length;
        for (int i = 0; i < CharacterNames.Length; i++)
        {
            string label = i == selectedCharacter ? "[" + (i + 1) + "]" : (i + 1).ToString();
            if (GUI.Button(new Rect(x + 12f + buttonWidth * i, y + 154f, buttonWidth - 3f, 30f), label))
            {
                SelectCharacter(i);
            }
        }

        if (GUI.Button(new Rect(x + 12f, y + 202f, panelWidth * 0.5f - 18f, 36f), "Start Run"))
        {
            StartRun();
        }
        if (GUI.Button(new Rect(x + panelWidth * 0.5f + 6f, y + 202f, panelWidth * 0.5f - 18f, 36f), "Back"))
        {
            ReturnToMainMenu();
        }
        GUI.Label(new Rect(x + 12f, y + 238f, panelWidth - 24f, 18f), "Select a number or use the buttons.");
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
}
