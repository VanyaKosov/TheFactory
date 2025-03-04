using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Core
{
    class World
    {
        private readonly int worldGenRadius;
        private readonly Dictionary<Vector2Int, Tile> map = new();
        private Vector2Int playerPos;

        public World(int worldGenRadius)
        {
            this.worldGenRadius = worldGenRadius;
        }

        private void GenTile(Vector2Int pos)
        {
            if (map.ContainsKey(pos))
            {
                return;
            }

            Tile tile = new(Back.Grass1, Ore.Empty, 0);
            map.Add(pos, tile);
        }
    }
}
