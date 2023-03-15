using Microsoft.Xna.Framework.Content;
using System.Text.RegularExpressions;

namespace Disarray.Engine;

/// <summary>
/// Manages the loading of assets. Acts as a wrapper for the game's <see cref="ContentManager"/>.
/// </summary>
public class Assets
{
    private ContentManager contentManager;
    private Regex rootDirectoryMatcher;

    public void Initialize(Main main)
    {
        contentManager = main.Content;
        rootDirectoryMatcher = new($".*{contentManager.RootDirectory}(\\\\|\\/)*");
    }

    /// <summary>
    /// Load the asset with the given asset name.
    /// </summary>
    /// <typeparam name="T">The type of asset to load.</typeparam>
    /// <param name="assetName">The name of the asset, including its path relative to the assets directory.</param>
    /// <returns>The loaded asset.</returns>
    public T Load<T>(string assetName)
    {
        return contentManager.Load<T>(assetName);
    }

    /// <summary>
    /// Load the asset with the absolute file path (it should still eventually be found within the assets folder).
    /// </summary>
    /// <typeparam name="T">The type of asset to load.</typeparam>
    /// <param name="absoluteFilePath">The absolute file path to the asset.</param>
    /// <returns>The loaded asset.</returns>
    public T LoadFromFilePath<T>(string absoluteFilePath)
    {
        string relativeFilePath = rootDirectoryMatcher.Replace(absoluteFilePath, "");
        string file = Path.ChangeExtension(relativeFilePath, null);

        T asset = Load<T>(file);
        
        return asset;
    }
}
