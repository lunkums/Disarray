using DefaultEcs;
using DefaultEcs.System;
using Disarray.Engine.Components;
using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine.Systems;

[With(typeof(Transform), typeof(Sprite)), Without(typeof(BlendedTransform))]
public class SpriteRenderer : AEntitySetSystem<SpriteBatch>
{
    public SpriteRenderer(World world) : base(world)
    {
    }

    protected override void Update(SpriteBatch spriteBatch, in Entity entity)
    {
        ref Transform transform = ref entity.Get<Transform>();
        ref Sprite sprite = ref entity.Get<Sprite>();

        spriteBatch.Draw(sprite.Texture, transform.Position, sprite.SourceRectangle, sprite.Color,
            transform.Rotation, sprite.Origin, transform.Scale, sprite.SpriteEffects, sprite.LayerDepth);
    }
}
