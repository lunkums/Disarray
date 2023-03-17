using DefaultEcs;
using DefaultEcs.System;
using Disarray.Engine.Components;
using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine.Systems;

[With(typeof(BlendedTransform), typeof(Sprite))]
public class MovingSpriteRenderer : AEntitySetSystem<SpriteBatch>
{
    public MovingSpriteRenderer(World world) : base(world)
    {
    }

    protected override void Update(SpriteBatch spriteBatch, in Entity entity)
    {
        ref BlendedTransform transform = ref entity.Get<BlendedTransform>();
        ref Sprite sprite = ref entity.Get<Sprite>();

        spriteBatch.Draw(sprite.Texture, transform.Position, sprite.SourceRectangle, sprite.Color,
            transform.Rotation, sprite.Origin, transform.Scale, sprite.SpriteEffects, sprite.LayerDepth);
    }
}
