using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class TileSet
    {
        public Texture2D Texture
        {
            get;
            private set;
        }
        public int TileSize
        {
            get;
            private set;
        }

        public TileSet(Texture2D texture, int tileSize)
        {
            this.Texture = texture;
            this.TileSize = tileSize;
        }
    }

    public static class TileSetExtensions
    {
        public static void DrawTile(this SpriteBatch batch, TileSet tileset, int index, Vector2 position, Color color, float scale = 1, float rotation = 0)
        {
            float radians = MathHelper.ToRadians(rotation);
            int actualX = (int)((position.X+0.5) * tileset.TileSize * scale);
            int actualY = (int)((position.Y+0.5) * tileset.TileSize * scale);
            int actualSize = (int)(tileset.TileSize * scale);
            batch.Draw(tileset.Texture,
                new Rectangle(actualX, actualY, actualSize, actualSize),
                new Rectangle(index * tileset.TileSize, 0, tileset.TileSize, tileset.TileSize),
                color, radians, new Vector2(tileset.TileSize/2,tileset.TileSize/2), SpriteEffects.None, 0);
        }
        
        public static void BatchDrawTile(this SpriteBatch batch, TileSet tileset, int index, List<Vector2> positions, Color color, float scale = 1, float rotation = 0)
        {
            foreach (Vector2 item in positions)
            {
                batch.DrawTile(tileset, index, item, color, scale, rotation);
            }
        }
    }
}
