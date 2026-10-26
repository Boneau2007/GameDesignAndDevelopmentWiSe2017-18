using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Shared winter palette and style helpers for game menus and result screens.
/// </summary>
public static class GameUITheme
{
    public const int ItemPadding = 5;
    public const int ItemSpacing = 5;
    public const FontStyle BodyFontStyle = FontStyle.Normal;
    public const FontStyle EmphasisFontStyle = FontStyle.Bold;
    public const int MenuButtonFontSize = 22;
    public const int MenuTitleFontSize = 36;
    public const int ScreenTitleFontSize = 44;
    public const int PanelTitleFontSize = 24;
    public const int BodyFontSize = 16;
    public const int HudFontSize = 34;
    public const int ResultFontSize = 32;
    public const int SmallLabelFontSize = 14;
    public const int LanguageFlagFontSize = 30;
    public const int ButtonFontSize = 20;

    public static readonly Color ForestGreen = new Color(0.055f, 0.18f, 0.14f, 1f);
    public static readonly Color WarmCream = new Color(0.96f, 0.89f, 0.72f, 1f);
    public static readonly Color DeepGreen = new Color(0.035f, 0.09f, 0.065f, 1f);
    public static readonly Color ChristmasRed = new Color(0.66f, 0.12f, 0.14f, 1f);
    public static readonly Color ButtonGreen = new Color(0.15f, 0.38f, 0.27f, 1f);
    public static readonly Color AccentGold = new Color(0.92f, 0.68f, 0.28f, 1f);
    private static readonly Dictionary<Color, Texture2D> GuiTextures = new Dictionary<Color, Texture2D>();
    private static readonly Dictionary<Color, Texture2D> GuiPanelTextures = new Dictionary<Color, Texture2D>();

    /// <summary>
    /// Creates a label style using the shared UI defaults.
    /// </summary>
    public static GUIStyle CreateLabelStyle(int fontSize, FontStyle fontStyle, TextAnchor alignment, Color color)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.font = GUI.skin.font;
        style.fontSize = fontSize;
        style.fontStyle = fontStyle;
        style.alignment = alignment;
        style.wordWrap = true;
        ApplyGuiSpacing(style);
        style.normal.textColor = color;
        style.hover.textColor = color;
        style.active.textColor = color;
        style.focused.textColor = color;
        return style;
    }

    /// <summary>
    /// Creates a button style using the shared winter palette.
    /// </summary>
    public static GUIStyle CreateButtonStyle(int fontSize)
    {
        GUIStyle style = new GUIStyle(GUI.skin.button)
        {
            font = GUI.skin.font,
            fontSize = fontSize,
            fontStyle = EmphasisFontStyle
        };
        ApplyGuiSpacing(style);
        style.normal.textColor = Color.white;
        style.hover.textColor = WarmCream;
        style.active.textColor = WarmCream;
        Texture2D buttonTexture = GetGuiTexture(Color.white);
        style.normal.background = buttonTexture;
        style.hover.background = buttonTexture;
        style.active.background = buttonTexture;
        style.focused.background = buttonTexture;
        return style;
    }

    /// <summary>
    /// Creates an immediate-mode panel style using the shared palette.
    /// </summary>
    public static GUIStyle CreatePanelStyle()
    {
        return CreatePanelStyle(ForestGreen, WarmCream);
    }

    public static GUIStyle CreatePanelStyle(Color backgroundColor, Color textColor)
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.font = GUI.skin.font;
        ApplyGuiSpacing(style);
        Texture2D background = GetGuiPanelTexture(backgroundColor);
        style.border = new RectOffset(1, 1, 1, 1);
        style.normal.background = background;
        style.hover.background = background;
        style.active.background = background;
        style.focused.background = background;
        style.normal.textColor = textColor;
        style.hover.textColor = textColor;
        style.active.textColor = textColor;
        style.focused.textColor = textColor;
        return style;
    }

    public static GUIStyle CreateTransparentButtonStyle(int fontSize)
    {
        GUIStyle style = CreateButtonStyle(fontSize);
        style.normal.background = null;
        style.hover.background = null;
        style.active.background = null;
        style.focused.background = null;
        return style;
    }

    /// <summary>
    /// Applies the shared palette and typography to all panels, buttons, and labels in a menu canvas.
    /// </summary>
    public static void ConfigureMenuCanvas(Transform canvasRoot)
    {
        if (canvasRoot == null)
        {
            Debug.LogError("A menu canvas is required to apply the shared UI theme.");
            return;
        }

        ConfigureNamedPanels(canvasRoot, "MainMenuPanel", ForestGreen);
        ConfigureNamedPanels(canvasRoot, "HighscorePanel", ForestGreen);
        ConfigureNamedPanels(canvasRoot, "AboutPanel", ForestGreen);
        ConfigureNamedPanels(canvasRoot, "VerticalLayoutPanel", WarmCream);

        Button[] buttons = canvasRoot.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            Color buttonColor = string.Equals(button.name, "startButton", System.StringComparison.OrdinalIgnoreCase)
                ? ChristmasRed
                : ButtonGreen;
            ConfigureMenuButton(button, buttonColor);
        }

        Text[] labels = canvasRoot.GetComponentsInChildren<Text>(true);
        foreach (Text label in labels)
        {
            if (label.GetComponentInParent<Button>() != null)
            {
                continue;
            }

            Transform card = FindAncestor(label.transform, "VerticalLayoutPanel");
            bool isTitle = string.Equals(label.name, "Title", System.StringComparison.Ordinal);
            Color textColor = card != null ? DeepGreen : WarmCream;
            int fontSize = isTitle
                ? (card != null ? PanelTitleFontSize : MenuTitleFontSize)
                : BodyFontSize;
            TextAnchor alignment = isTitle ? TextAnchor.MiddleCenter : label.alignment;
            if (isTitle)
            {
                Vector2 titleSize = label.rectTransform.sizeDelta;
                titleSize.y = Mathf.Max(titleSize.y, 52f);
                label.rectTransform.sizeDelta = titleSize;
            }

            ConfigureTextInPlace(label, fontSize,
                isTitle ? EmphasisFontStyle : BodyFontStyle, textColor, alignment);
        }

        ConfigureResponsiveMenu(canvasRoot);
    }

    public static void ConfigureResponsiveMenu(Transform canvasRoot)
    {
        if (canvasRoot == null)
        {
            Debug.LogError("A menu canvas is required to configure its responsive layout.");
            return;
        }

        Canvas canvas = canvasRoot.GetComponent<Canvas>();
        RectTransform canvasRect = canvas != null ? canvas.transform as RectTransform : null;
        Transform menu = FindDescendant(canvasRoot, "MainMenuPanel");
        if (canvasRect == null || menu == null)
        {
            return;
        }

        float scaleFactor = Mathf.Max(0.01f, canvas.scaleFactor);
        float availableWidth = canvasRect.rect.width;
        float availableHeight = canvasRect.rect.height;
        if (availableWidth <= 0f || availableHeight <= 0f)
        {
            availableWidth = Screen.width / scaleFactor;
            availableHeight = Screen.height / scaleFactor;
        }

        float panelWidth = Mathf.Min(440f, Mathf.Max(0f, availableWidth - 32f));
        float buttonHeight = Mathf.Clamp(availableHeight * 0.075f, 30f, 46f);
        float titleHeight = Mathf.Clamp(availableHeight * 0.11f, 42f, 58f);
        const float spacing = 8f;
        const int panelPadding = 12;

        VerticalLayoutGroup layout = menu.GetComponent<VerticalLayoutGroup>();
        if (layout == null)
        {
            layout = menu.gameObject.AddComponent<VerticalLayoutGroup>();
        }

        layout.padding = new RectOffset(panelPadding, panelPadding, panelPadding, panelPadding);
        layout.spacing = spacing;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = menu.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            fitter.enabled = false;
        }

        RectTransform menuRect = menu as RectTransform;
        menuRect.anchorMin = new Vector2(0.5f, 0.5f);
        menuRect.anchorMax = new Vector2(0.5f, 0.5f);
        menuRect.pivot = new Vector2(0.5f, 0.5f);

        Text title = FindDescendant(menu, "Title") != null
            ? FindDescendant(menu, "Title").GetComponent<Text>()
            : null;
        int buttonCount = menu.GetComponentsInChildren<Button>(true).Length;
        float contentHeight = panelPadding * 2f + titleHeight + buttonCount * buttonHeight +
                              spacing * (buttonCount + 1);
        float panelHeight = Mathf.Min(contentHeight, Mathf.Max(0f, availableHeight - 24f));
        menuRect.sizeDelta = new Vector2(panelWidth, panelHeight);

        if (title != null)
        {
            SetPreferredHeight(title.gameObject, Mathf.Min(titleHeight,
                Mathf.Max(24f, panelHeight - buttonCount * 30f - panelPadding * 2f)));
        }

        Button[] buttons = menu.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            SetPreferredHeight(button.gameObject, Mathf.Min(buttonHeight,
                Mathf.Max(26f, (panelHeight - titleHeight - panelPadding * 2f -
                                spacing * (buttonCount + 1)) / Mathf.Max(1, buttonCount))));
        }

        VerticalLayoutGroup[] nestedLayouts = canvasRoot.GetComponentsInChildren<VerticalLayoutGroup>(true);
        foreach (VerticalLayoutGroup nestedLayout in nestedLayouts)
        {
            if (nestedLayout == layout)
            {
                continue;
            }

            nestedLayout.padding = new RectOffset(
                Mathf.Max(ItemPadding, nestedLayout.padding.left),
                Mathf.Max(ItemPadding, nestedLayout.padding.right),
                Mathf.Max(ItemPadding, nestedLayout.padding.top),
                Mathf.Max(ItemPadding, nestedLayout.padding.bottom));
            nestedLayout.spacing = Mathf.Max(ItemSpacing, nestedLayout.spacing);
            nestedLayout.childControlWidth = true;
            nestedLayout.childForceExpandWidth = true;
        }

        ConfigureResponsiveDialog(canvasRoot, "AboutPanel", availableWidth, availableHeight);
        ConfigureResponsiveDialog(canvasRoot, "HighscorePanel", availableWidth, availableHeight);
        LayoutRebuilder.MarkLayoutForRebuild(menuRect);
    }

    private static void ConfigureResponsiveDialog(Transform canvasRoot, string panelName,
        float availableWidth, float availableHeight)
    {
        Transform panel = FindDescendant(canvasRoot, panelName);
        if (panel == null)
        {
            return;
        }

        RectTransform panelRect = panel as RectTransform;
        if (panelRect == null)
        {
            Debug.LogError(panelName + " must use a RectTransform to support responsive layout.");
            return;
        }

        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(Mathf.Min(960f, Mathf.Max(0f, availableWidth - 32f)),
            Mathf.Min(700f, Mathf.Max(0f, availableHeight - 32f)));

        Transform content = FindDescendant(panel, "VerticalLayoutPanel");
        RectTransform contentRect = content as RectTransform;
        if (contentRect != null)
        {
            contentRect.anchorMin = new Vector2(0f, 0f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.offsetMin = new Vector2(16f, 76f);
            contentRect.offsetMax = new Vector2(-16f, -16f);
        }

        Transform backButton = FindDescendant(panel, "BackButton");
        RectTransform backRect = backButton as RectTransform;
        if (backRect != null)
        {
            backRect.anchorMin = new Vector2(0.5f, 0f);
            backRect.anchorMax = new Vector2(0.5f, 0f);
            backRect.pivot = new Vector2(0.5f, 0f);
            backRect.sizeDelta = new Vector2(Mathf.Min(240f, Mathf.Max(0f, panelRect.rect.width - 32f)), 48f);
            backRect.anchoredPosition = new Vector2(0f, 12f);
        }

        VerticalLayoutGroup contentLayout = content != null
            ? content.GetComponent<VerticalLayoutGroup>()
            : null;
        if (contentLayout != null)
        {
            contentLayout.padding = new RectOffset(12, 12, 12, 12);
            contentLayout.spacing = ItemSpacing * 2;
            contentLayout.childControlWidth = true;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childControlHeight = true;
            contentLayout.childForceExpandHeight = false;

            int flexibleChildCount = 0;
            for (int i = 0; i < content.childCount; i++)
            {
                if (!string.Equals(content.GetChild(i).name, "Title", System.StringComparison.Ordinal))
                {
                    flexibleChildCount++;
                }
            }

            float contentHeight = Mathf.Max(0f, panelRect.sizeDelta.y - 92f - 24f -
                contentLayout.spacing * Mathf.Max(0, content.childCount - 1));
            float titleMinHeight = content.childCount > 0
                ? Mathf.Min(48f, contentHeight / content.childCount)
                : 0f;
            float flexibleMinHeight = flexibleChildCount > 0
                ? Mathf.Max(0f, (contentHeight - titleMinHeight) / flexibleChildCount)
                : 0f;

            for (int i = 0; i < content.childCount; i++)
            {
                Transform child = content.GetChild(i);
                LayoutElement element = child.GetComponent<LayoutElement>();
                if (element == null)
                {
                    element = child.gameObject.AddComponent<LayoutElement>();
                }

                bool isTitle = string.Equals(child.name, "Title", System.StringComparison.Ordinal);
                element.minHeight = isTitle ? titleMinHeight : flexibleMinHeight;
                element.preferredHeight = isTitle
                    ? Mathf.Min(56f, Mathf.Max(titleMinHeight, contentHeight * 0.3f))
                    : Mathf.Min(140f, Mathf.Max(flexibleMinHeight, contentHeight * 0.7f /
                                                  Mathf.Max(1, flexibleChildCount)));
                element.flexibleHeight = isTitle ? 0f : 1f;
            }
        }

        LayoutRebuilder.MarkLayoutForRebuild(panelRect);
    }

    private static void SetPreferredHeight(GameObject item, float height)
    {
        LayoutElement layout = item.GetComponent<LayoutElement>();
        if (layout == null)
        {
            layout = item.AddComponent<LayoutElement>();
        }

        layout.minHeight = height;
        layout.preferredHeight = height;
        layout.flexibleHeight = 0f;
    }

    private static Transform FindDescendant(Transform root, string objectName)
    {
        Transform[] descendants = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform descendant in descendants)
        {
            if (string.Equals(descendant.name, objectName, System.StringComparison.Ordinal))
            {
                return descendant;
            }
        }

        return null;
    }

    /// <summary>
    /// Applies the shared gameplay palette to HUD labels and controls without tinting sprite icons.
    /// </summary>
    public static void ConfigureGameplayCanvas(Transform canvasRoot)
    {
        if (canvasRoot == null)
        {
            Debug.LogError("A gameplay HUD is required to apply the shared UI theme.");
            return;
        }

        Button[] buttons = canvasRoot.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            ConfigureMenuButton(button, ButtonGreen);
        }

        Text[] labels = canvasRoot.GetComponentsInChildren<Text>(true);
        foreach (Text label in labels)
        {
            if (label.GetComponentInParent<Button>() != null)
            {
                continue;
            }

            bool isScore = string.Equals(label.name, "Score", System.StringComparison.Ordinal);
            int fontSize = isScore ? HudFontSize : BodyFontSize;
            ConfigureText(label, label.rectTransform.sizeDelta, label.rectTransform.anchoredPosition,
                fontSize, EmphasisFontStyle, WarmCream, TextAnchor.MiddleCenter);
        }
    }

    /// <summary>
    /// Gives a panel graphic the shared surface color and gold outline.
    /// </summary>
    public static void ConfigurePanel(Image image, Color backgroundColor)
    {
        if (image == null)
        {
            return;
        }

        image.color = backgroundColor;
        image.raycastTarget = false;

        Outline outline = image.GetComponent<Outline>();
        if (outline == null)
        {
            outline = image.gameObject.AddComponent<Outline>();
        }

        outline.effectColor = AccentGold;
        outline.effectDistance = new Vector2(2f, -2f);
        outline.useGraphicAlpha = true;
    }

    private static void ConfigureNamedPanels(Transform root, string panelName, Color color)
    {
        Transform[] descendants = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform panel in descendants)
        {
            if (!string.Equals(panel.name, panelName, System.StringComparison.Ordinal))
            {
                continue;
            }

            Image image = panel.GetComponent<Image>();
            if (image == null)
            {
                image = panel.gameObject.AddComponent<Image>();
            }

            ConfigurePanel(image, color);
        }
    }

    private static Transform FindAncestor(Transform current, string objectName)
    {
        while (current != null)
        {
            if (string.Equals(current.name, objectName, System.StringComparison.Ordinal))
            {
                return current;
            }

            current = current.parent;
        }

        return null;
    }

    private static Texture2D GetGuiTexture(Color color)
    {
        Texture2D texture;
        if (GuiTextures.TryGetValue(color, out texture) && texture != null)
        {
            return texture;
        }

        texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
        {
            name = "GameUIThemeColor",
            hideFlags = HideFlags.HideAndDontSave,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GuiTextures[color] = texture;
        return texture;
    }

    private static Texture2D GetGuiPanelTexture(Color backgroundColor)
    {
        Texture2D texture;
        if (GuiPanelTextures.TryGetValue(backgroundColor, out texture) && texture != null)
        {
            return texture;
        }

        texture = new Texture2D(3, 3, TextureFormat.RGBA32, false)
        {
            name = "GameUIThemePanel",
            hideFlags = HideFlags.HideAndDontSave,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                bool isBorder = x == 0 || y == 0 || x == 2 || y == 2;
                texture.SetPixel(x, y, isBorder ? AccentGold : backgroundColor);
            }
        }

        texture.Apply();
        GuiPanelTextures[backgroundColor] = texture;
        return texture;
    }

    /// <summary>
    /// Creates a button that visually matches a template without copying its persistent click actions.
    /// </summary>
    public static Button CreateButton(Button template, Transform parent, string objectName, string label,
        UnityAction onClick)
    {
        if (template == null || parent == null)
        {
            Debug.LogError("A button template and parent are required to create a themed button.");
            return null;
        }

        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform buttonRect = buttonObject.transform as RectTransform;
        RectTransform templateRect = template.transform as RectTransform;
        if (templateRect != null)
        {
            buttonRect.anchorMin = templateRect.anchorMin;
            buttonRect.anchorMax = templateRect.anchorMax;
            buttonRect.pivot = templateRect.pivot;
            buttonRect.sizeDelta = templateRect.sizeDelta;
        }

        LayoutElement templateLayout = template.GetComponent<LayoutElement>();
        if (templateLayout != null)
        {
            LayoutElement layout = buttonObject.AddComponent<LayoutElement>();
            layout.ignoreLayout = templateLayout.ignoreLayout;
            layout.minWidth = templateLayout.minWidth;
            layout.minHeight = templateLayout.minHeight;
            layout.preferredWidth = templateLayout.preferredWidth;
            layout.preferredHeight = templateLayout.preferredHeight;
            layout.flexibleWidth = templateLayout.flexibleWidth;
            layout.flexibleHeight = templateLayout.flexibleHeight;
        }

        Image templateImage = template.GetComponent<Image>();
        Image image = buttonObject.GetComponent<Image>();
        if (templateImage != null)
        {
            image.sprite = templateImage.sprite;
            image.type = templateImage.type;
            image.color = templateImage.color;
            image.material = templateImage.material;
            image.raycastTarget = templateImage.raycastTarget;
        }

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.transition = template.transition;
        button.colors = template.colors;
        button.spriteState = template.spriteState;
        button.animationTriggers = template.animationTriggers;
        button.interactable = template.interactable;
        if (onClick != null)
        {
            button.onClick.AddListener(onClick);
        }

        Text templateText = template.GetComponentInChildren<Text>(true);
        if (templateText != null)
        {
            GameObject textObject = Object.Instantiate(templateText.gameObject, buttonObject.transform, false);
            textObject.name = "Text";
            Text text = textObject.GetComponent<Text>();
            text.text = label;
            text.raycastTarget = false;
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(4f, 0f);
            textRect.offsetMax = new Vector2(-4f, 0f);
            textRect.localScale = Vector3.one;
        }
        else
        {
            Debug.LogError("The button template is missing its Text component.");
        }

        return button;
    }

    /// <summary>
    /// Applies common typography and overflow settings to a UI text element.
    /// </summary>
    public static void ConfigureText(Text text, Vector2 size, Vector2 position, int fontSize,
        FontStyle fontStyle, Color color, TextAnchor alignment)
    {
        if (text == null)
        {
            Debug.LogError("A required UI text element is missing.");
            return;
        }

        SetRectTransform(text.rectTransform,
            new Vector2(Mathf.Max(0f, size.x - ItemPadding * 2f),
                Mathf.Max(0f, size.y - ItemPadding * 2f)), position);
        ConfigureTextAppearance(text, fontSize, fontStyle, color, alignment);
    }

    public static void ConfigureButtonText(Text text, int fontSize, FontStyle fontStyle, Color color,
        TextAnchor alignment)
    {
        if (text == null)
        {
            Debug.LogError("A required button text element is missing.");
            return;
        }

        ApplyButtonLabelPadding(text.rectTransform);
        ConfigureTextAppearance(text, fontSize, fontStyle, color, alignment);
    }

    private static void ConfigureTextInPlace(Text text, int fontSize, FontStyle fontStyle, Color color,
        TextAnchor alignment)
    {
        if (text == null)
        {
            Debug.LogError("A required UI text element is missing.");
            return;
        }

        ApplyRectPadding(text.rectTransform);
        ConfigureTextAppearance(text, fontSize, fontStyle, color, alignment);
    }

    private static void ConfigureTextAppearance(Text text, int fontSize, FontStyle fontStyle, Color color,
        TextAnchor alignment)
    {
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
        AddTextShadow(text);
    }

    /// <summary>
    /// Applies the shared palette to a UI button's graphic and transition colors.
    /// </summary>
    public static void ConfigureButton(Button button, Image image, Color backgroundColor)
    {
        if (image != null)
        {
            image.color = backgroundColor;
        }

        if (button == null)
        {
            return;
        }

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = WarmCream;
        colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
        button.colors = colors;
    }

    private static void AddTextShadow(Text text)
    {
        if (text == null)
        {
            return;
        }

        Shadow shadow = text.GetComponent<Shadow>();
        if (shadow == null)
        {
            shadow = text.gameObject.AddComponent<Shadow>();
        }

        shadow.effectColor = new Color(0f, 0f, 0f, 0.38f);
        shadow.effectDistance = new Vector2(1f, -1f);
        shadow.useGraphicAlpha = true;
    }

    /// <summary>
    /// Applies shared button colors and label typography to a menu navigation button.
    /// </summary>
    public static void ConfigureMenuButton(Button button, Color backgroundColor)
    {
        if (button == null)
        {
            return;
        }

        ConfigureButton(button, button.GetComponent<Image>(), backgroundColor);
        Text label = button.GetComponentInChildren<Text>(true);
        if (label != null)
        {
            ApplyButtonLabelPadding(label.rectTransform);
            label.fontSize = MenuButtonFontSize;
            label.fontStyle = EmphasisFontStyle;
            label.color = Color.white;
            label.alignment = TextAnchor.MiddleCenter;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 16;
            label.resizeTextMaxSize = MenuButtonFontSize;
            label.raycastTarget = false;
            AddTextShadow(label);
        }
    }

    private static void ApplyGuiSpacing(GUIStyle style)
    {
        RectOffset padding = new RectOffset(ItemPadding, ItemPadding, ItemPadding, ItemPadding);
        style.padding = padding;
        style.margin = new RectOffset(ItemSpacing, ItemSpacing, ItemSpacing, ItemSpacing);
    }

    private static void ApplyRectPadding(RectTransform rectTransform)
    {
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.offsetMin += new Vector2(ItemPadding, ItemPadding);
        rectTransform.offsetMax -= new Vector2(ItemPadding, ItemPadding);
    }

    private static void ApplyButtonLabelPadding(RectTransform labelRect)
    {
        if (labelRect == null)
        {
            return;
        }

        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(ItemPadding, ItemPadding);
        labelRect.offsetMax = new Vector2(-ItemPadding, -ItemPadding);
    }

    /// <summary>
    /// Sets a UI element's centered size and position.
    /// </summary>
    public static void SetRectTransform(RectTransform rectTransform, Vector2 size, Vector2 position)
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
