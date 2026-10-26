using UnityEditor;
using UnityEngine;

/// <summary>
/// Ensures character sprites use tight meshes; set each sprite's custom pivot at its feet in the Sprite Editor.
/// </summary>
public class ChristmasCharacterSpritePostprocessor : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        const string characterFolder = "Assets/Resources/ChristmasCharacters/";
        if (!assetPath.Replace('\\', '/').StartsWith(characterFolder, System.StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = UnityEngine.SpriteMeshType.Tight;
        importer.SetTextureSettings(settings);
    }
}
