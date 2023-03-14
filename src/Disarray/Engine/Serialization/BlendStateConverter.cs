using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine.Serialization;

public class BlendStateConverter : StringMatchConverter<BlendState>
{
    public override BlendState? GetValueFromLowercaseString(string str)
    {
        switch (str)
        {
            case "additive":
                return BlendState.Additive;
            case "alphablend":
                return BlendState.AlphaBlend;
            case "nonpremultiplied":
                return BlendState.NonPremultiplied;
            case "opaque":
                return BlendState.Opaque;
            default:
                return null;
        }
    }
}
