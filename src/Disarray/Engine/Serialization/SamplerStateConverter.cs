using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine.Serialization;

public class SamplerStateConverter : StringMatchConverter<SamplerState>
{
    public override SamplerState GetValueFromLowercaseString(string str)
    {
        switch (str)
        {
            case "anisotropicclamp":
                return SamplerState.AnisotropicClamp;
            case "anisotropicwrap":
                return SamplerState.AnisotropicWrap;
            case "linearclamp":
                return SamplerState.LinearClamp;
            case "linearwrap":
                return SamplerState.LinearWrap;
            case "pointclamp":
                return SamplerState.PointClamp;
            case "pointwrap":
                return SamplerState.PointWrap;
            default:
                return null;
        }
    }
}
