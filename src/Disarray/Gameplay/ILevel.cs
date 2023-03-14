using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Gameplay;

/// <summary>
/// An interface that describes a game level. Inherit this to create different levels with distinct gameplay logic.
/// </summary>
public interface ILevel
{
    void Initialize(Main main);
    void LoadContent();
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
}