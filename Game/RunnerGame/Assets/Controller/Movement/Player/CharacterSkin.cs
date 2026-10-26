using UnityEngine;

/// <summary>
/// Replaces the default player visual with the saved Christmas character and animates it.
/// </summary>
public class CharacterSkin : MonoBehaviour
{
    private const string ResourceFolder = "ChristmasCharacters/";
    private static readonly string[] CharacterNames =
    {
        "",
        "PeppermintElf",
        "SnowflakeScout",
        "GingerbreadRunner",
        "ReindeerDash"
    };

    private SpriteRenderer characterRenderer;
    private Sprite[] idleFrames;
    private Sprite[] walkFrames;
    private Collider2D playerCollider;
    private float animationTimer;
    private int currentFrame;

    /// <summary>
    /// Loads the selected character's sprites and replaces Santa's visual when needed.
    /// </summary>
    private void Awake()
    {
        int characterIndex = Mathf.Clamp(PlayerPrefs.GetInt(CharacterSelectionMenu.PreferenceKey, 0), 0,
            CharacterNames.Length - 1);
        if (characterIndex == 0)
        {
            return;
        }

        string characterName = CharacterNames[characterIndex];
        idleFrames = LoadFrames(characterName, "Idle");
        walkFrames = LoadFrames(characterName, "Walk");
        if (idleFrames == null || walkFrames == null)
        {
            Debug.LogError("Missing sprite frames for character " + characterName + ".");
            return;
        }

        Transform santaVisual = transform.Find("santa_64x64_Body");
        if (santaVisual == null)
        {
            Debug.LogError("Could not find the Santa visual to replace on the Player.");
            return;
        }

        SpriteRenderer santaRenderer = santaVisual.GetComponent<SpriteRenderer>();
        if (santaRenderer == null)
        {
            Debug.LogError("Could not find the Santa sprite renderer to replace on the Player.");
            return;
        }

        playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogError("Could not find a collider to align the selected character with the ground.");
            return;
        }

        int sortingLayerId = santaRenderer.sortingLayerID;
        santaVisual.gameObject.SetActive(false);
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        GameObject visual = new GameObject("Selected Christmas Character");
        visual.transform.SetParent(transform, false);
        visual.transform.localScale = santaVisual.localScale;
        characterRenderer = visual.AddComponent<SpriteRenderer>();
        characterRenderer.sprite = idleFrames[0];
        characterRenderer.sortingLayerID = sortingLayerId;
        characterRenderer.sortingOrder = 2;
        AlignCharacterToGround();
    }

    /// <summary>
    /// Updates the selected character's facing direction and animation frame.
    /// </summary>
    private void Update()
    {
        if (characterRenderer == null)
        {
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        characterRenderer.flipX = horizontal < -0.01f;
        bool isWalking = Mathf.Abs(horizontal) > 0.01f;
        Sprite[] frames = isWalking ? walkFrames : idleFrames;
        float frameDuration = isWalking ? 0.16f : 0.55f;

        animationTimer += Time.deltaTime;
        if (animationTimer >= frameDuration)
        {
            animationTimer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            characterRenderer.sprite = frames[currentFrame];
            AlignCharacterToGround();
        }
    }

    /// <summary>
    /// Aligns the sprite's authored foot pivot with the bottom of the player collider.
    /// </summary>
    private void AlignCharacterToGround()
    {
        Transform visualTransform = characterRenderer.transform;
        Vector3 position = visualTransform.position;
        position.y = playerCollider.bounds.min.y;
        visualTransform.position = position;
    }

    /// <summary>
    /// Loads the two animation frames for a character and animation state.
    /// </summary>
    /// <param name="characterName">The resource-name prefix for the character.</param>
    /// <param name="animationName">The animation state, such as Idle or Walk.</param>
    /// <returns>The loaded sprite frames, or <see langword="null"/> if a frame is missing.</returns>
    private static Sprite[] LoadFrames(string characterName, string animationName)
    {
        Sprite[] frames = new Sprite[2];
        for (int i = 0; i < frames.Length; i++)
        {
            frames[i] = Resources.Load<Sprite>(ResourceFolder + characterName + "_" + animationName + "_" + i);
            if (frames[i] == null)
            {
                return null;
            }
        }

        return frames;
    }
}
