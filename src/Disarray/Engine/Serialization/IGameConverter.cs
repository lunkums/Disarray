using Microsoft.Xna.Framework;

namespace Disarray.Engine.Serialization;

/// <summary>
/// A JSON converter that needs an instance of a game.
/// </summary>
public interface IGameConverter
{
    Game Game { set; }
}
