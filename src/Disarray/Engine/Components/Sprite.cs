
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine.Components;

public struct Sprite
{
    public Texture2D Texture;
    public Rectangle? SourceRectangle;
    public Color Color;
    public Vector2 Origin;
    public SpriteEffects SpriteEffects;
    public float LayerDepth;
}
