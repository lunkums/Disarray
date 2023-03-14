using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine.Serialization;

public class DepthStencilStateConverter : StringMatchConverter<DepthStencilState>
{
    public override DepthStencilState GetValueFromLowercaseString(string str)
    {
        switch (str)
        {
            case "default":
                return DepthStencilState.Default;
            case "depthread":
                return DepthStencilState.DepthRead;
            case "none":
                return DepthStencilState.None;
            default:
                return null;
        }
    }
}
