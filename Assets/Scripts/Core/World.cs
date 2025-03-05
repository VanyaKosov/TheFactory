using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    class World
    {
        private const int worldGenRadius = 100;
        private readonly Dictionary<Vector2Int, Tile> map = new();
        public Vector2Int PlayerPos { get; private set; }

        public event EventHandler<TileGeneratedEventArgs> TileGenerated;

        public World()
        {
            PlayerPos = new(0, 0);
        }

        public void Run()
        {
            GenInitialWorld(worldGenRadius * 2);
        }

        public void UpdatePLayerPos(Vector3 newPos)
        {
            Vector2Int roundedPos = new(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y));
            if (roundedPos.Equals(PlayerPos))
            {
                return;
            }

            ExpandMap(roundedPos);
        }

        private void ExpandMap(Vector2Int newPlayerPos)
        {
            Vector2Int offset = newPlayerPos - PlayerPos;
            if (offset.x == 0 && offset.y == 0)
            {
                return;
            }

            if (offset.x != 0 && offset.y != 0)
            {
                Vector2Int tempOffset = new(offset.x, 0);
                MakeLine(newPlayerPos, tempOffset);
                tempOffset = new(0, offset.y);
                MakeLine(newPlayerPos, tempOffset);
            }
            else
            {
                MakeLine(newPlayerPos, offset);
            }

            PlayerPos = newPlayerPos;
        }

        private void MakeLine(Vector2Int newPlayerPos, Vector2Int offset)
        {
            Vector2Int pos = newPlayerPos + offset * worldGenRadius;
            offset = new(offset.y, offset.x);
            pos -= offset * worldGenRadius;
            for (int i = 0; i < worldGenRadius * 2 + 1; i++)
            {
                GenTile(pos);
                pos += offset;
            }
        }

        private void GenTile(Vector2Int pos)
        {
            if (map.ContainsKey(pos))
            {
                return;
            }

            Tile tile = new(Back.Grass1, Ore.Empty, 0);
            map.Add(pos, tile);

            TileGenerated?.Invoke(this, new(pos, tile.BackType));
        }

        private void GenInitialWorld(int radius)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    GenTile(new(x, y));
                }
            }
        }

        public class TileGeneratedEventArgs : EventArgs
        {
            public readonly Vector2Int Pos;
            public readonly Back Background;

            public TileGeneratedEventArgs(Vector2Int pos, Back background)
            {
                Pos = pos;
                Background = background;
            }
        }
    }
}
