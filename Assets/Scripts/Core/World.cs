using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Core
{
    class World
    {
        private const int worldGenRadius = 100;
        private const float coalSpawnChance = 0.00005f; // 0.005% per tile
        private const int maxRichness = 10_000;
        private readonly Vector2 coalRadiusVariation = new(8f, 12f);

        private readonly Dictionary<Vector2Int, Tile> map = new();
        public Vector2Int PlayerPos { get; private set; }

        public event EventHandler<TileGeneratedEventArgs> TileGenerated;
        public event EventHandler<OreSpawnedEventArgs> OreSpawned;

        public World()
        {
            PlayerPos = new(0, 0);
        }

        public void Run()
        {
            GenInitialWorld(worldGenRadius * 2);

            //SpawnOre(Ore.Coal, coalRadiusVariation, new(0, 0));
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
            if (CheckSpawnChance(coalSpawnChance))
            {
                SpawnOre(Ore.Coal, coalRadiusVariation, pos);
                return;
            }

            GenBackTile(pos);
        }

        private void GenBackTile(Vector2Int pos)
        {
            if (map.ContainsKey(pos))
            {
                return;
            }

            Tile tile = new(Back.Grass1, Ore.Empty, 0);
            map.Add(pos, tile);
            TileGenerated?.Invoke(this, new(pos, tile.BackType));
        }

        private void SpawnOre(Ore type, Vector2 radiusVariation, Vector2Int center)
        {
            float radiusSquared = UnityEngine.Random.Range(radiusVariation.x, radiusVariation.y);
            int intRadius = (int)(radiusSquared + 0.5f);
            radiusSquared *= radiusSquared;

            for (int x = center.x - intRadius; x <= center.x + intRadius; x++)
            {
                for (int y = center.y - intRadius; y <= center.y + intRadius; y++)
                {
                    Vector2Int pos = new(x, y);
                    GenBackTile(pos);

                    float distSquared = Math.Abs(Mathf.Pow(x - center.x, 2)) + Math.Abs(Mathf.Pow(y - center.y, 2));
                    if (distSquared <= radiusSquared)
                    {
                        float richnessPercent = 99f - (distSquared / radiusSquared) * 100;
                        map[pos] = new(map[pos].BackType, type, (int)(richnessPercent * maxRichness));
                        OreSpawned?.Invoke(this, new(pos, type, richnessPercent));
                    }
                }
            }
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

        private bool CheckSpawnChance(float chance)
        {
            float random = UnityEngine.Random.Range(0f, 1f);
            return random < chance;
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

        public class OreSpawnedEventArgs : EventArgs
        {
            public readonly Vector2Int Pos;
            public readonly Ore Type;
            public readonly float RichnessPercent;

            public OreSpawnedEventArgs(Vector2Int pos, Ore type, float richnessPercent)
            {
                Pos = pos;
                Type = type;
                RichnessPercent = richnessPercent;
            }
        }
    }
}
