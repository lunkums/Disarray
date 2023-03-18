using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using TiledCS;

namespace Disarray.Engine.Systems;

public class InfiniteTilemapRenderer
{
    public float LayerDepth;

    private Main main;

    private TiledMap map;
    private TiledLayer collisionLayer;
    private Dictionary<int, TiledTileset> tilesets;
    private Dictionary<TiledTileset, Texture2D> tilesetTextures;
    private Rectangle? debugRect;

    public InfiniteTilemapRenderer(Main main)
    {
        this.main = main;
    }

    [Flags]
    enum Trans
    {
        None = 0,
        Flip_H = 1 << 0,
        Flip_V = 1 << 1,
        Flip_D = 1 << 2,

        Rotate_90 = Flip_D | Flip_H,
        Rotate_180 = Flip_H | Flip_V,
        Rotate_270 = Flip_V | Flip_D,

        Rotate_90AndFlip_H = Flip_H | Flip_V | Flip_D,
    }

    public void LoadContent(string tilemapDirectory, string tilemap)
    {
        string tilemapPath = Path.Combine(tilemapDirectory + "/", tilemap);
        string tilesetDirectory = Path.Combine(Data.DataDirectoryPath, tilemapDirectory + "/");

        map = new TiledMap();
        map.ParseXml(Data.ReadTextFromRelativeFile(tilemapPath));
        tilesets = map.GetTiledTilesets(tilesetDirectory);

        tilesetTextures = new();

        foreach (var tileset in tilesets.Values)
        {
            tilesetTextures.Add(tileset, main.Assets.LoadFromFilePath<Texture2D>(tileset.Image.source));
        }

        // Retrieving objects or layers can be done using Linq or a for loop
        collisionLayer = map.Layers.First(l => l.name == "Ground");
    }

    public void Update(float delta)
    {
        Vector2 mousePosVec = main.Input.MousePosition;
        var mousePosInWorld = main.Camera.ScreenToWorld(mousePosVec);

        // Check if mouse is in the bounds of a Tiled object
        debugRect = null;
        foreach (var obj in collisionLayer.objects)
        {
            var objRect = new Rectangle((int)obj.x, (int)obj.y, (int)obj.width, (int)obj.height);
            if (objRect.Contains((int)mousePosInWorld.X, (int)mousePosInWorld.Y))
            {
                debugRect = objRect;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var tileLayers = map.Layers.Where(x => x.type == TiledLayerType.TileLayer);
        BoundingFrustum boundingFrustum = main.Camera.GetBoundingFrustum();
        BoundingBox chunkBoundingBox = new();

        foreach (var layer in tileLayers)
        {
            foreach (var chunk in layer.chunks)
            {
                chunkBoundingBox.Min
                    = new(chunk.x * map.TileWidth, chunk.y * map.TileHeight, 0);
                chunkBoundingBox.Max
                    = new((chunk.x + chunk.width) * map.TileWidth, (chunk.y + chunk.height) * map.TileHeight, 0);

                // Don't draw tiles that will be culled anyways
                if (boundingFrustum.Contains(chunkBoundingBox) == ContainmentType.Disjoint)
                {
                    continue;
                }

                for (var y = 0; y < chunk.height; y++)
                {
                    for (var x = 0; x < chunk.width; x++)
                    {
                        var index = (y * chunk.width) + x; // Assuming the default render order is used which is from right to bottom
                        var gid = chunk.data[index]; // The tileset tile index
                        var tileX = (chunk.x + x) * map.TileWidth;
                        var tileY = (chunk.y + y) * map.TileHeight;
                        
                        // Gid 0 is used to tell there is no tile set
                        if (gid == 0)
                        {
                            continue;
                        }

                        // Helper method to fetch the right TieldMapTileset instance
                        // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property
                        var mapTileset = map.GetTiledMapTileset(gid);

                        // Retrieve the actual tileset based on the firstgid property of the connection object we retrieved just now
                        var tileset = tilesets[mapTileset.firstgid];

                        // Use the connection object as well as the tileset to figure out the source rectangle
                        var rect = map.GetSourceRect(mapTileset, tileset, gid);

                        // Create destination and source rectangles
                        var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                        var destination = new Rectangle(tileX, tileY, map.TileWidth, map.TileHeight);


                        // You can use the helper methods to get information to handle flips and rotations
                        Trans tileTrans = Trans.None;
                        if (map.IsTileFlippedHorizontal(layer, x, y)) tileTrans |= Trans.Flip_H;
                        if (map.IsTileFlippedVertical(layer, x, y)) tileTrans |= Trans.Flip_V;
                        if (map.IsTileFlippedDiagonal(layer, x, y)) tileTrans |= Trans.Flip_D;

                        SpriteEffects effects = SpriteEffects.None;
                        double rotation = 0f;
                        switch (tileTrans)
                        {
                            case Trans.Flip_H: effects = SpriteEffects.FlipHorizontally; break;
                            case Trans.Flip_V: effects = SpriteEffects.FlipVertically; break;

                            case Trans.Rotate_90:
                                rotation = Math.PI * .5f;
                                destination.X += map.TileWidth;
                                break;

                            case Trans.Rotate_180:
                                rotation = Math.PI;
                                destination.X += map.TileWidth;
                                destination.Y += map.TileHeight;
                                break;

                            case Trans.Rotate_270:
                                rotation = Math.PI * 3 / 2;
                                destination.Y += map.TileHeight;
                                break;

                            case Trans.Rotate_90AndFlip_H:
                                effects = SpriteEffects.FlipHorizontally;
                                rotation = Math.PI * .5f;
                                destination.X += map.TileWidth;
                                break;

                            default:
                                break;
                        }


                        // Render sprite at position tileX, tileY using the rect
                        spriteBatch.Draw(tilesetTextures[tileset], destination, source, Color.White, (float)rotation,
                            Vector2.Zero, effects, LayerDepth);
                    }
                }
            }
        }

        // If mouse is over a collider, display its bounds
        if (debugRect != null)
        {
            Texture2D _texture = new Texture2D(main.GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.Green });

            spriteBatch.Draw(_texture, (Rectangle)debugRect, Color.White);
        }
    }
}
