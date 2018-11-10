using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    [Serializable]
    public struct Entity
    {
        public string name;
        public int health;
        public int special;
        public int direction;
        public int tileIndex;
        public Vector2 position;
        public bool killable;
        public bool moved;

        public static Entity GetZero()
        {
            return new Entity("Zero", 0, 0, 0, Vector2.Zero, false);
        }

        public Entity(string name, int health, int direction, int tileIndex, Vector2 position, bool killable, int special = 3)
        {
            this.moved = false;
            this.name = name;
            this.health = health;
            this.special = special;
            this.direction = direction;
            this.tileIndex = tileIndex;
            this.position = position;
            this.killable = killable;
        }
    }

    public static class EntityExtensions
    {
        public static void DrawEntity(this SpriteBatch batch, TileSet tileset, Entity entity, Vector2 mod, Color color, float scale = 1, float frame = 0)
        {
            if (entity.moved)
            {
                switch (entity.direction)
                {
                    case 0:
                        batch.DrawTile(tileset, entity.tileIndex, new Vector2(entity.position.X - mod.X + frame - 1, entity.position.Y - mod.Y), color, scale, entity.direction * 90);
                        break;
                    case 1:
                        batch.DrawTile(tileset, entity.tileIndex, new Vector2(entity.position.X - mod.X, entity.position.Y - mod.Y + frame - 1), color, scale, entity.direction * 90);
                        break;
                    case 2:
                        batch.DrawTile(tileset, entity.tileIndex, new Vector2(entity.position.X - mod.X - frame + 1, entity.position.Y - mod.Y), color, scale, entity.direction * 90);
                        break;
                    case 3:
                        batch.DrawTile(tileset, entity.tileIndex, new Vector2(entity.position.X - mod.X, entity.position.Y - mod.Y - frame + 1), color, scale, entity.direction * 90);
                        break;
                }
            }
            else
                batch.DrawTile(tileset, entity.tileIndex, new Vector2(entity.position.X - mod.X, entity.position.Y - mod.Y), color, scale, entity.direction*90);
        }

        public static void BatchDrawEntity(this SpriteBatch batch, TileSet tileset, List<Entity> entities, Vector2 mod, Color color, float scale = 1, float frame = 0)
        {
            foreach (Entity item in entities)
            {
                batch.DrawEntity(tileset, item, mod, color, scale, frame);
            }
        }
    }
}
