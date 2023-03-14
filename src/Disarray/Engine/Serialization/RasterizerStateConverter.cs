using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine.Serialization;

public class RasterizerStateConverter : StringMatchConverter<RasterizerState>
{
    public override RasterizerState GetValueFromLowercaseString(string str)
    {
        switch (str)
        {
            case "cullclockwise":
                return RasterizerState.CullClockwise;
            case "cullcounterclockwise":
                return RasterizerState.CullCounterClockwise;
            case "cullnone":
                return RasterizerState.CullNone;
            default:
                return null;
        }
    }
}
