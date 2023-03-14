using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json;

namespace Disarray.Engine.Serialization;

public class OrthographicCameraConverter : JsonConverter, IGameConverter
{
    private GameWindow gameWindow;
    private GraphicsDevice graphicsDevice;

    public Game Game
    {
        set
        {
            gameWindow = value.Window;
            graphicsDevice = value.GraphicsDevice;
        }
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(OrthographicCamera);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        ViewportAdapter viewportAdapter = null;
        OrthographicCameraParameters cparams = serializer.Deserialize<OrthographicCameraParameters>(reader);
        ViewportParameters vparams = cparams.ViewportParameters;
        string key = cparams.ViewportAdapterType == null ? "" : cparams.ViewportAdapterType;

        switch (key.ToLowerInvariant())
        {
            case "boxing":
                viewportAdapter = new BoxingViewportAdapter(gameWindow, graphicsDevice, vparams.VirtualWidth,
                    vparams.VirtualHeight, vparams.HorizontalBleed, vparams.VerticalBleed);
                break;
            case "scaling":
                viewportAdapter = new ScalingViewportAdapter(graphicsDevice, vparams.VirtualWidth,
                    vparams.VirtualHeight);
                break;
            case "window":
                viewportAdapter = new WindowViewportAdapter(gameWindow, graphicsDevice);
                break;
        }

        if (viewportAdapter == null)
            viewportAdapter = new DefaultViewportAdapter(graphicsDevice);

        return new OrthographicCamera(viewportAdapter);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
