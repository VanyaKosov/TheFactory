using Dev.Kosov.Factory.Core.Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Dev.Kosov.Factory.Core
{
    public class World
    {
        private const int worldGenRadius = 100;
        private const float treeGenThreshold = 0.25f;
        private const float treeNoiseScale = 0.005f;
        private float2 treeNoiseOffset;
        private readonly OreInfo[] oreInfos =
        {
            new(Ore.Coal, new(8f, 12f), 10_000, 0.00001f),
            new(Ore.Copper, new(8f, 12f), 10_000, 0.00001f),
            new(Ore.Iron, new(8f, 12f), 10_000, 0.00001f)
        };
        private readonly Dictionary<Vector2Int, Tile> map = new();
        private readonly Dictionary<int, Entity> entities = new();

        public Inventory Inventory { get; private set; }
        public Vector2Int PlayerPos { get; private set; }
        public event EventHandler<TileGeneratedEventArgs> TileGenerated;
        public event EventHandler<OreSpawnedEventArgs> OreSpawned;
        public event EventHandler<EntityCreatedEventArgs> EntityCreated;
        public event EventHandler<EntityRemovedEventArgs> EntityRemoved;

        public World()
        {
            Inventory = new();
            PlayerPos = new(0, 0);
        }

        public void Run()
        {
            treeNoiseOffset = new(UnityEngine.Random.Range(-1_000_000, 1_000_000), UnityEngine.Random.Range(-1_000_000, 1_000_000));
            GenInitialWorld(worldGenRadius * 2);

            Inventory.Run();

            //SpawnOre(Ore.Coal, coalRadiusVariation, new(0, 0));
        }

        public void UpdatePlayerPos(Vector3 newPos)
        {
            Vector2Int roundedPos = new(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y));
            if (roundedPos.Equals(PlayerPos))
            {
                return;
            }

            ExpandMap(roundedPos);
        }

        public void PlaceEntity(Vector2Int bottomLeftPos)
        {
            if (Inventory.CursorSlot.Amount <= 0) return;
            if (!ItemInfo.Get(Inventory.CursorSlot.Type).Placable) return;

            EntityType type = ItemInfo.Get(Inventory.CursorSlot.Type).EntityType;
            CreateEntity(bottomLeftPos, type);
            /*Entity entity = EntityGenerator.GenEntityInstance(type, bottomLeftPos);
            Vector2Int size = EntityInfo.Get(type).Size;
            if (!CheckAvailability(entity.bottomLeftPos, size)) return;
            int entityID = Tile.GenEntityID();
            entities.Add(entityID, entity);
            for (int x = entity.bottomLeftPos.x; x < entity.bottomLeftPos.x + size.x; x++)
            {
                for (int y = entity.bottomLeftPos.y; y < entity.bottomLeftPos.y + size.y; y++)
                {
                    Vector2Int newPos = new(x, y);
                    map[newPos].EntityID = entityID;
                }
            }

            EntityCreated?.Invoke(this, new(bottomLeftPos, type, size, entityID));*/
        }

        public void RemoveEntity(Vector2Int pos)
        {
            Tile tile = map[pos];
            int id = tile.EntityID;
            if (id == -1) return;

            Entity entity = entities[id];
            entities.Remove(id);
            Vector2Int size = EntityInfo.Get(entity.type).Size;
            for (int x = entity.bottomLeftPos.x; x < entity.bottomLeftPos.x + size.x; x++)
            {
                for (int y = entity.bottomLeftPos.y; y < entity.bottomLeftPos.x + size.y; y++)
                {
                    Vector2Int newPos = new(x, y);
                    map[newPos].EntityID = -1;
                }
            }

            EntityRemoved?.Invoke(this, new(id));
        }

        public Tile GetTileInfo(Vector2Int pos)
        {
            return map[pos];
        }

        private void CreateEntity(Vector2Int bottomLeftPos, EntityType type)
        {
            Entity entity = EntityGenerator.GenEntityInstance(type, bottomLeftPos);
            Vector2Int size = EntityInfo.Get(entity.type).Size;
            if (!CheckAvailability(entity.bottomLeftPos, size)) return;
            int entityID = Tile.GenEntityID();
            entities.Add(entityID, entity);

            for (int x = entity.bottomLeftPos.x; x < entity.bottomLeftPos.x + size.x; x++)
            {
                for (int y = entity.bottomLeftPos.y; y < entity.bottomLeftPos.y + size.y; y++)
                {
                    map[new(x, y)].EntityID = entityID;
                }
            }

            EntityCreated?.Invoke(this, new(bottomLeftPos, type, size, entityID));
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
            foreach (OreInfo oreInfo in oreInfos)
            {
                if (CheckSpawnChance(oreInfo.spawnChance))
                {
                    SpawnOre(oreInfo, pos);
                    return;
                }
            }

            GenBackTile(pos);
            GenTree(pos);
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

        private void SpawnOre(OreInfo oreInfo, Vector2Int center)
        {
            float radiusSquared = UnityEngine.Random.Range(oreInfo.radiusVariation.x, oreInfo.radiusVariation.y);
            int intRadius = (int)(radiusSquared + 0.5f);
            radiusSquared *= radiusSquared;

            for (int i = 0; i <= 1; i++)
            {
                for (int x = center.x - intRadius; x <= center.x + intRadius; x++)
                {
                    for (int y = center.y - intRadius; y <= center.y + intRadius; y++)
                    {
                        Vector2Int pos = new(x, y);
                        GenBackTile(pos);

                        float distSquared = Math.Abs(Mathf.Pow(x - center.x, 2)) + Math.Abs(Mathf.Pow(y - center.y, 2));
                        if (distSquared <= radiusSquared)
                        {
                            if (i == 0)
                            {
                                if (map[pos]?.OreType != Ore.Empty)
                                {
                                    return;
                                }

                                continue;
                            }

                            float richnessPercent = 100f - (distSquared / radiusSquared) * 100;
                            map[pos] = new(map[pos].BackType, oreInfo.type, (int)(richnessPercent * oreInfo.maxRichness));
                            OreSpawned?.Invoke(this, new(pos, oreInfo.type, richnessPercent));
                        }
                    }
                }
            }
        }

        private void GenTree(Vector2Int bottomLeftPos)
        {
            Vector2Int size = EntityInfo.Get(EntityType.Tree).Size;
            float2 noisePos = new(bottomLeftPos.x * treeNoiseScale, bottomLeftPos.y * treeNoiseScale);
            float noiseVal = noise.snoise(noisePos + treeNoiseOffset);
            noiseVal += noise.snoise(noisePos * 2) / 2;
            noiseVal += noise.snoise(noisePos * 4) / 4;
            noiseVal += noise.snoise(noisePos * 8) / 8;
            noiseVal += noise.snoise(noisePos * 16) / 16;
            if (noiseVal < treeGenThreshold) return;
            for (int x = bottomLeftPos.x; x < bottomLeftPos.x + size.x; x++)
            {
                for (int y = bottomLeftPos.y; y < bottomLeftPos.y + size.y; y++)
                {
                    GenBackTile(new(x, y));
                }
            }

            CreateEntity(bottomLeftPos, EntityType.Tree);
            //if (!CheckAvailability(pos, size)) return;

            /*int entityID = Tile.GenEntityID();
            for (int x = pos.x; x < pos.x + size.x; x++)
            {
                for (int y = pos.y; y < pos.y + size.y; y++)
                {
                    Vector2Int newPos = new(x, y);
                    GenBackTile(newPos);
                    map[newPos].EntityID = entityID;

                }
            }*/

            //Entity tree = new(Rotation.Up, pos, new() { ItemType.Wood }, new() { 10 }, EntityType.Tree);
            //entities.Add(entityID, tree);
            //EntityCreated?.Invoke(this, new(pos, EntityType.Tree, size, entityID));
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

        private bool CheckAvailability(Vector2Int pos, Vector2Int size)
        {
            for (int x = pos.x; x < pos.x + size.x; x++)
            {
                for (int y = pos.y; y < pos.y + size.y; y++)
                {
                    Vector2Int newPos = new(x, y);
                    if (map.ContainsKey(newPos) && map[newPos].EntityID != -1)
                    {
                        return false;
                    }
                }
            }

            return true;
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

            internal TileGeneratedEventArgs(Vector2Int pos, Back background)
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

            internal OreSpawnedEventArgs(Vector2Int pos, Ore type, float richnessPercent)
            {
                Pos = pos;
                Type = type;
                RichnessPercent = richnessPercent;
            }
        }

        public class EntityCreatedEventArgs : EventArgs
        {
            public readonly Vector2Int Pos;
            public readonly EntityType Type;
            public readonly Vector2Int Size;
            public readonly int EntityID;

            internal EntityCreatedEventArgs(Vector2Int pos, EntityType type, Vector2Int size, int entityID)
            {
                Pos = pos;
                Type = type;
                Size = size;
                EntityID = entityID;
            }
        }

        public class EntityRemovedEventArgs : EventArgs
        {
            public readonly int EntityID;

            public EntityRemovedEventArgs(int entityID)
            {
                EntityID = entityID;
            }
        }

        private class OreInfo
        {
            public readonly Ore type;
            public readonly Vector2 radiusVariation;
            public readonly int maxRichness;
            public readonly float spawnChance;

            public OreInfo(Ore type, Vector2 radiusVariation, int maxRichness, float spawnChance)
            {
                this.type = type;
                this.radiusVariation = radiusVariation;
                this.maxRichness = maxRichness;
                this.spawnChance = spawnChance;
            }
        }
    }
}
