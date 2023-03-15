using Microsoft.Xna.Framework.Content;
using System.Text.RegularExpressions;

namespace Disarray.Engine;

public class Assets
{
    private ContentManager contentManager;
    private Regex rootDirectoryMatcher;

    public void Initialize(Main main)
    {
        contentManager = main.Content;
        rootDirectoryMatcher = new($".*{contentManager.RootDirectory}(\\\\|\\/)*");
    }

    public T Load<T>(string assetName)
    {
        return contentManager.Load<T>(assetName);
    }

    public T LoadFromFilePath<T>(string absoluteFilePath)
    {
        string relativeFilePath = rootDirectoryMatcher.Replace(absoluteFilePath, "");
        string file = Path.ChangeExtension(relativeFilePath, null);

        T asset = contentManager.Load<T>(file);
        
        return asset;
    }
}
