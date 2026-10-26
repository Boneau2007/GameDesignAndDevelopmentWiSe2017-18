using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// Loads language bundles from Resources and persists the selected language.
/// </summary>
public static class Localization
{
    private const string PreferenceKey = "SelectedLanguage";
    private const string ResourceFolder = "Localization";
    private static readonly HashSet<string> ReportedMissingKeys = new HashSet<string>();
    private static readonly Dictionary<string, LanguageData> Languages =
        new Dictionary<string, LanguageData>(StringComparer.OrdinalIgnoreCase);
    private static readonly List<LanguageOption> AvailableLanguageOptions = new List<LanguageOption>();

    private static Dictionary<string, string> currentTranslations;
    private static Dictionary<string, string> fallbackTranslations;
    private static string currentLanguageCode;
    private static bool initialized;

    /// <summary>
    /// Provides display information and the flag stored in a language bundle.
    /// </summary>
    public sealed class LanguageOption
    {
        /// <summary>
        /// Gets the language code used to identify the bundle.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Gets the language's display name, preferably in that language.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the Unicode flag symbol displayed for the language.
        /// </summary>
        public string Flag { get; private set; }

        internal LanguageOption(string code, string displayName, string flag)
        {
            Code = code;
            DisplayName = displayName;
            Flag = flag;
        }
    }

    private sealed class LanguageData
    {
        public LanguageOption Option;
        public Dictionary<string, string> Translations;
    }

    /// <summary>
    /// Occurs after the active language and translation table change.
    /// </summary>
    public static event Action LanguageChanged;

    /// <summary>
    /// Gets the active language code.
    /// </summary>
    public static string CurrentLanguageCode
    {
        get
        {
            EnsureLoaded();
            return currentLanguageCode;
        }
    }

    /// <summary>
    /// Gets the languages discovered in the Resources/Localization folder.
    /// </summary>
    public static IReadOnlyList<LanguageOption> AvailableLanguages
    {
        get
        {
            EnsureLoaded();
            return AvailableLanguageOptions;
        }
    }

    /// <summary>
    /// Loads and persists a discovered language bundle.
    /// </summary>
    /// <param name="languageCode">The language code declared in its bundle.</param>
    /// <returns><see langword="true"/> when the language is available and selected.</returns>
    public static bool SetLanguage(string languageCode)
    {
        EnsureLoaded();
        if (string.IsNullOrEmpty(languageCode) || !Languages.TryGetValue(languageCode, out LanguageData language))
        {
            Debug.LogWarning("No localization bundle was found for language code: " + languageCode);
            return false;
        }

        if (string.Equals(currentLanguageCode, language.Option.Code, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        currentLanguageCode = language.Option.Code;
        currentTranslations = language.Translations;
        PlayerPrefs.SetString(PreferenceKey, currentLanguageCode);
        PlayerPrefs.Save();
        LanguageChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Returns the localized value for a key, or the key itself when no translation exists.
    /// </summary>
    /// <param name="key">The translation key.</param>
    /// <returns>The translated string.</returns>
    public static string Get(string key)
    {
        EnsureLoaded();
        if (currentTranslations.TryGetValue(key, out string value))
        {
            return value;
        }

        if (fallbackTranslations.TryGetValue(key, out value))
        {
            ReportMissingKey(key);
            return value;
        }

        ReportMissingKey(key);
        return key;
    }

    /// <summary>
    /// Formats a localized value using the supplied arguments.
    /// </summary>
    /// <param name="key">The translation key.</param>
    /// <param name="arguments">Values inserted into numbered placeholders.</param>
    /// <returns>The formatted translated string.</returns>
    public static string Format(string key, params object[] arguments)
    {
        return string.Format(CultureInfo.CurrentCulture, Get(key), arguments);
    }

    private static void EnsureLoaded()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        TextAsset[] resources = Resources.LoadAll<TextAsset>(ResourceFolder);
        foreach (TextAsset resource in resources)
        {
            LanguageData language = ParseLanguage(resource);
            if (language == null)
            {
                continue;
            }

            if (Languages.ContainsKey(language.Option.Code))
            {
                Debug.LogError("Duplicate localization language code '" + language.Option.Code +
                               "' in Resources/" + ResourceFolder + ".");
                continue;
            }

            Languages.Add(language.Option.Code, language);
        }

        foreach (LanguageData language in Languages.Values)
        {
            AvailableLanguageOptions.Add(language.Option);
        }

        AvailableLanguageOptions.Sort((left, right) =>
            StringComparer.OrdinalIgnoreCase.Compare(left.DisplayName, right.DisplayName));

        if (AvailableLanguageOptions.Count == 0)
        {
            Debug.LogError("No valid localization bundles were found in Resources/" + ResourceFolder + ".");
            currentTranslations = new Dictionary<string, string>();
            fallbackTranslations = currentTranslations;
            currentLanguageCode = string.Empty;
            return;
        }

        string fallbackCode = Languages.ContainsKey("en") ? Languages["en"].Option.Code : AvailableLanguageOptions[0].Code;
        fallbackTranslations = Languages[fallbackCode].Translations;

        string savedLanguage = PlayerPrefs.GetString(PreferenceKey, fallbackCode);
        if (!Languages.TryGetValue(savedLanguage, out LanguageData selectedLanguage))
        {
            Debug.LogWarning("Saved language '" + savedLanguage +
                             "' is unavailable. Using '" + fallbackCode + "' instead.");
            selectedLanguage = Languages[fallbackCode];
            PlayerPrefs.SetString(PreferenceKey, selectedLanguage.Option.Code);
            PlayerPrefs.Save();
        }

        currentLanguageCode = selectedLanguage.Option.Code;
        currentTranslations = selectedLanguage.Translations;
    }

    private static LanguageData ParseLanguage(TextAsset resource)
    {
        LocalizationTable table;
        try
        {
            table = JsonUtility.FromJson<LocalizationTable>(resource.text);
        }
        catch (Exception exception)
        {
            Debug.LogError("Could not parse localization bundle '" + resource.name + "': " + exception.Message);
            return null;
        }

        if (table == null || string.IsNullOrWhiteSpace(table.languageCode) ||
            string.IsNullOrWhiteSpace(table.displayName) || string.IsNullOrWhiteSpace(table.flag) ||
            table.entries == null)
        {
            Debug.LogError("Localization bundle '" + resource.name +
                           "' must define languageCode, displayName, flag, and entries.");
            return null;
        }

        string languageCode = table.languageCode.Trim();
        Dictionary<string, string> translations = new Dictionary<string, string>();
        foreach (LocalizationEntry entry in table.entries)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.key))
            {
                Debug.LogError("Localization entries in '" + resource.name + "' must have a non-empty key.");
                return null;
            }

            string key = entry.key.Trim();
            if (translations.ContainsKey(key))
            {
                Debug.LogError("Duplicate localization key '" + key + "' in '" + resource.name + "'.");
                return null;
            }

            translations.Add(key, entry.value ?? string.Empty);
        }

        return new LanguageData
        {
            Option = new LanguageOption(languageCode, table.displayName.Trim(), table.flag.Trim()),
            Translations = translations
        };
    }

    private static void ReportMissingKey(string key)
    {
        if (ReportedMissingKeys.Add(currentLanguageCode + ":" + key))
        {
            Debug.LogWarning("Missing localization key '" + key + "' for language '" + currentLanguageCode + "'.");
        }
    }

    [Serializable]
    private sealed class LocalizationTable
    {
        public string languageCode;
        public string displayName;
        public string flag;
        public LocalizationEntry[] entries;
    }

    [Serializable]
    private sealed class LocalizationEntry
    {
        public string key;
        public string value;
    }
}
