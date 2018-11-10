using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    [Serializable]
    public class Level
    {
        public List<Entity> walls = new List<Entity>();
        public List<Entity> boxes = new List<Entity>();
        public List<Entity> ammos = new List<Entity>();
        public List<Entity> lifes = new List<Entity>();
        public List<Entity> checkpoints = new List<Entity>();
        public List<Entity> cannons = new List<Entity>();
        public List<Entity> bullets = new List<Entity>();
        public List<Entity> horizontals = new List<Entity>();
        public List<Entity> randoms = new List<Entity>();
        public Entity player;
    }
}
