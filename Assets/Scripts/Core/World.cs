using Dev.Kosov.Factory.Core.Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public class World
    {
        private const int startingOreRadius = 40;
        private const int worldGenRadius = 100;
        private const float treeGenThreshold = 0.25f;
        private const float treeNoiseScale = 0.005f;
        private float2 treeNoiseOffset;
        private readonly Dictionary<Vector2Int, Tile> map = new();
        private readonly Dictionary<int, Entity> entities = new();

        public Inventory Inventory { get; private set; }
        public Vector2Int PlayerPos { get; private set; }
        public Crafter PlayerCrafter { get; private set; }
        public event EventHandler<TileGeneratedEventArgs> TileGenerated;
        public event EventHandler<OreSpawnedEventArgs> OreSpawned;
        public event EventHandler<EntityCreatedEventArgs> EntityCreated;
        public event EventHandler<EntityRemovedEventArgs> EntityRemoved;
        public event EventHandler<OreMinedEventArgs> OreMined;
        public event EventHandler<CrafterOpenedEventArgs> CrafterOpened;
        public event EventHandler<ChestOpenedEventArgs> ChestOpened;

        public World()
        {
            Inventory = new();
            PlayerPos = new(0, 0);
            PlayerCrafter = new(new()
            {
                RecipeType.Smelt_iron_ore,
                RecipeType.Smelt_copper_ore,
                RecipeType.Smelt_stone_ore,
                RecipeType.Smelt_iron_plate,
                RecipeType.Make_copper_wire,
                RecipeType.Make_simple_circuit,
                RecipeType.Make_iron_gear,
                RecipeType.Make_inserter,
                RecipeType.Make_assembler1,
                RecipeType.Make_wooden_chest,
                RecipeType.Make_iron_chest,
                RecipeType.Make_furnace,
                RecipeType.Make_electric_drill,
                RecipeType.Make_concrete
            });
        }

        public void UpdateState()
        {
            PlayerCrafter.UpdateState();
            UpdateEntities();
        }

        public void Run()
        {
            treeNoiseOffset = new(UnityEngine.Random.Range(-1_000_000, 1_000_000), UnityEngine.Random.Range(-1_000_000, 1_000_000));
            GenInitialWorld(worldGenRadius * 2);

            Inventory.Run();
        }

        public void TryPutOrTakeFromCrafterInput(Crafter crafter, Vector2Int pos)
        {
            if (Inventory.CursorSlot.Type == ItemType.None)
            {
                InvSlot item = crafter.InputStorage.GetItem(pos);
                crafter.InputStorage.SetItem(new(ItemType.None, 0), pos);
                Inventory.SetCursorSlot(item.Type, item.Amount);

                return;
            }

            if (Inventory.CursorSlot.Type != crafter.GetExpectedInputItem(pos).Type) return;
            int remainder = crafter.InputStorage.TryStack(Inventory.CursorSlot, pos);
            if (remainder == 0)
            {
                Inventory.SetCursorSlot(ItemType.None, 0);
                return;
            }
            Inventory.SetCursorSlot(Inventory.CursorSlot.Type, remainder);
        }

        public void TryTakeFromCrafterOutput(Crafter crafter, Vector2Int pos)
        {
            if (!crafter.OutputStorage.CanTake) return;
            if (Inventory.CursorSlot.Type != ItemType.None) return;
            InvSlot item = crafter.OutputStorage.GetItem(pos);
            crafter.OutputStorage.SetItem(new(ItemType.None, 0), pos);
            Inventory.SetCursorSlot(item.Type, item.Amount);
        }

        public void OpenEntity(Vector2Int pos)
        {
            if (Inventory.CursorSlot.Type != ItemType.None) return;
            Tile tile = map[pos];
            if (tile.EntityID == -1) return;
            Entity entity = entities[tile.EntityID];
            if (entity.Type == EntityType.Tree) return;

            if (entity is ICrafter crafter)
            {
                CrafterOpened?.Invoke(this, new(crafter.GetCrafter()));
            }

            if (entity is Chest chest)
            {
                ChestOpened?.Invoke(this, new(chest));
            }
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

        public void PlaceEntity(Vector2Int bottomLeftPos, Rotation rotation)
        {
            InvSlot cursorSlot = Inventory.CursorSlot;
            if (cursorSlot.Amount <= 0) return;
            if (!ItemInfo.Get(cursorSlot.Type).Placable) return;

            EntityType type = ItemInfo.Get(cursorSlot.Type).EntityType;
            if (CreateEntity(bottomLeftPos, type, rotation))
            {
                Inventory.SetCursorSlot(cursorSlot.Type, cursorSlot.Amount - 1);
            }
        }

        public Tile GetTileInfo(Vector2Int pos)
        {
            return map[pos];
        }

        public ActionType GetActionType(Vector2Int pos)
        {
            if (map[pos].EntityID != -1 && entities[map[pos].EntityID].Type == EntityType.Tree) return ActionType.ChopTree;
            if (map[pos].EntityID != -1) return ActionType.RemoveBuilding;
            if (map[pos].OreType != OreType.None) return ActionType.MineOre;

            return ActionType.None;
        }

        public void Remove(Vector2Int pos)
        {
            ActionType action = GetActionType(pos);
            if (action == ActionType.None) return;

            if (action == ActionType.RemoveBuilding)
            {
                RemoveEntity(pos);

                return;
            }

            if (action == ActionType.ChopTree)
            {
                RemoveEntity(pos);

                return;
            }

            if (action == ActionType.MineOre)
            {
                OreType oreType = MineOre(pos);
                if (oreType == OreType.None) return;
                Inventory.AddItemToInventory(OreInfo.Get(oreType).ItemType, 1);

                return;
            }

            throw new Exception("Unknown action");
        }

        private void UpdateEntities()
        {
            foreach (Entity entity in entities.Values)
            {
                entity.UpdateState();
            }
        }

        private void RemoveEntity(Vector2Int pos)
        {
            Tile tile = map[pos];
            int id = tile.EntityID;
            if (id == -1) return;

            Entity entity = entities[id];
            List<InvSlot> deconstructionComponents = entity.GetComponents();
            foreach (InvSlot item in deconstructionComponents)
            {
                Inventory.AddItemToInventory(item.Type, item.Amount);
            }

            entities.Remove(id);
            Vector2Int size = EntityInfo.Get(entity.Type).Size;
            for (int x = entity.BottomLeftPos.x; x < entity.BottomLeftPos.x + size.x; x++)
            {
                for (int y = entity.BottomLeftPos.y; y < entity.BottomLeftPos.y + size.y; y++)
                {
                    Vector2Int newPos = new(x, y);
                    map[newPos].EntityID = -1;
                }
            }

            EntityRemoved?.Invoke(this, new(id));
        }

        private OreType MineOre(Vector2Int pos)
        {
            Tile tile = map[pos];
            if (tile.OreType == OreType.None) return OreType.None;

            OreType oreType = map[pos].OreType;
            float prevRichnessPercent = ((float)tile.OreAmount / OreInfo.Get(oreType).MaxRichness) * 100;
            tile.OreAmount--;
            if (tile.OreAmount <= 0)
            {
                tile.OreType = OreType.None;
            }
            float newRichnessPercent = ((float)tile.OreAmount / OreInfo.Get(oreType).MaxRichness) * 100;

            OreMined?.Invoke(this, new(pos, prevRichnessPercent, newRichnessPercent, tile.OreType));
            return oreType;
        }

        private bool CreateEntity(Vector2Int bottomLeftPos, EntityType type, Rotation rotation)
        {
            Entity entity = EntityGenerator.GenEntityInstance(type, bottomLeftPos, rotation, GetEntityAtPos);
            Vector2Int size = EntityInfo.Get(entity.Type).Size;
            if (!CheckAvailability(entity.BottomLeftPos, size)) return false;
            int entityID = Tile.GenEntityID();
            entities.Add(entityID, entity);

            for (int x = entity.BottomLeftPos.x; x < entity.BottomLeftPos.x + size.x; x++)
            {
                for (int y = entity.BottomLeftPos.y; y < entity.BottomLeftPos.y + size.y; y++)
                {
                    map[new(x, y)].EntityID = entityID;
                }
            }

            EntityCreated?.Invoke(this, new(entity, bottomLeftPos, entityID));
            return true;
        }

        private Entity GetEntityAtPos(Vector2Int pos)
        {
            int id = map[pos].EntityID;
            if (id == -1) return null;
            return entities[id];
        }

        private void SpawnStartingOres()
        {
            foreach ((OreType oreType, OreInfo.Info oreInfo) in OreInfo.GetAll())
            {
                bool found = false;
                while (!found)
                {
                    int centerX = UnityEngine.Random.Range(-startingOreRadius, startingOreRadius);
                    int centerY = UnityEngine.Random.Range(-startingOreRadius, startingOreRadius);
                    float radius = UnityEngine.Random.Range(oreInfo.RadiusVariation.x, oreInfo.RadiusVariation.y);
                    int intRadius = (int)(radius + 0.5f);

                    found = true;
                    for (int x = centerX - intRadius; x <= centerX + intRadius; x++)
                    {
                        if (!found) break;
                        for (int y = centerY - intRadius; y <= centerY + intRadius; y++)
                        {
                            Vector2Int pos = new(x, y);
                            if (map.ContainsKey(pos) && map[pos].OreType != OreType.None)
                            {
                                found = false;
                                break;
                            }
                        }
                    }

                    if (!found) continue;

                    SpawnOre(oreType, oreInfo, new(centerX, centerY), radius);
                }
            }
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
            foreach ((OreType oreType, OreInfo.Info oreInfo) in OreInfo.GetAll())
            {
                if (CheckSpawnChance(oreInfo.SpawnChance))
                {
                    SpawnOre(oreType, oreInfo, pos);
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

            Tile tile = new(BackType.Grass1, OreType.None, 0);
            map.Add(pos, tile);
            TileGenerated?.Invoke(this, new(pos, tile.BackType));
        }

        private void SpawnOre(OreType oreType, OreInfo.Info oreInfo, Vector2Int center, float radiusSquared = -1)
        {
            if (radiusSquared == -1)
            {
                radiusSquared = UnityEngine.Random.Range(oreInfo.RadiusVariation.x, oreInfo.RadiusVariation.y);
            }
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
                                if (map[pos].OreType != OreType.None) return;

                                continue;
                            }

                            float richnessPercent = 100f - (distSquared / radiusSquared) * 100;
                            map[pos] = new(map[pos].BackType, oreType, (int)(richnessPercent / 100 * oreInfo.MaxRichness), map[pos].EntityID);
                            OreSpawned?.Invoke(this, new(pos, oreType, richnessPercent));
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

            CreateEntity(bottomLeftPos, EntityType.Tree, Rotation.Up);
        }

        private void GenInitialWorld(int radius)
        {
            SpawnStartingOres();

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
            public readonly BackType Background;

            internal TileGeneratedEventArgs(Vector2Int pos, BackType background)
            {
                Pos = pos;
                Background = background;
            }
        }

        public class OreSpawnedEventArgs : EventArgs
        {
            public readonly Vector2Int Pos;
            public readonly OreType Type;
            public readonly float RichnessPercent;

            internal OreSpawnedEventArgs(Vector2Int pos, OreType type, float richnessPercent)
            {
                Pos = pos;
                Type = type;
                RichnessPercent = richnessPercent;
            }
        }

        public class EntityCreatedEventArgs : EventArgs
        {
            public readonly Entity Entity;
            public readonly Vector2Int Pos;
            public readonly int EntityID;

            internal EntityCreatedEventArgs(Entity entity, Vector2Int pos, int entityID)
            {
                Entity = entity;
                Pos = pos;
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

        public class OreMinedEventArgs : EventArgs
        {
            public readonly Vector2Int Pos;
            public readonly float PrevRichnessPercent;
            public readonly float NewRichnessPercent;
            public readonly OreType Type;

            public OreMinedEventArgs(Vector2Int pos, float prevRichnessPercent, float newRichnessPercent, OreType type)
            {
                PrevRichnessPercent = prevRichnessPercent;
                NewRichnessPercent = newRichnessPercent;
                Pos = pos;
                Type = type;
            }
        }

        public class CrafterOpenedEventArgs : EventArgs
        {
            public readonly Crafter Crafter;

            public CrafterOpenedEventArgs(Crafter crafter)
            {
                Crafter = crafter;
            }
        }

        public class ChestOpenedEventArgs : EventArgs
        {
            public readonly Chest Chest;

            public ChestOpenedEventArgs(Chest chest)
            {
                Chest = chest;
            }
        }
    }
}
