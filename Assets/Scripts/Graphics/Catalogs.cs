using Dev.Kosov.Factory.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class Catalogs : MonoBehaviour
    {
        private const int oreVariantsPerRichnessLevel = 8;

        [Header("Item Icons Sprites")]
        public Sprite Wood;
        public Sprite Coal;
        public Sprite Iron_Ore;
        public Sprite Iron_Plate;
        public Sprite Copper_Ore;
        public Sprite Copper_Plate;
        public Sprite Assembler1;
        public Sprite WoodChest;
        public Sprite StoneFurnace;

        [Header("Entity Sprites")]
        public Sprite Assembler1Entity;
        public Sprite WoodChestEntity;
        public Sprite StoneFurnaceEntity;

        [Header("Entity Prefabs")]
        public GameObject[] TreePrefabs;
        public GameObject Assembler1Prefab;
        public GameObject WoodChestPrefab;
        public GameObject StoneFurnacePrefab;

        private readonly Dictionary<ItemType, Sprite> itemTypeToSprite = new();
        private readonly Dictionary<EntityType, Sprite> entityTypeToSprite = new();
        private readonly Dictionary<BackType, Sprite[]> backTypeToSprite = new();
        private readonly Dictionary<OreType, Sprite[]> oreToSprite = new();

        void Awake()
        {
            // Icons
            itemTypeToSprite.Add(ItemType.Wood, Wood);
            itemTypeToSprite.Add(ItemType.Coal, Coal);
            itemTypeToSprite.Add(ItemType.Iron_ore, Iron_Ore);
            itemTypeToSprite.Add(ItemType.Iron_plate, Iron_Plate);
            itemTypeToSprite.Add(ItemType.Copper_ore, Copper_Ore);
            itemTypeToSprite.Add(ItemType.Copper_plate, Copper_Plate);
            itemTypeToSprite.Add(ItemType.Assembler_1, Assembler1);
            itemTypeToSprite.Add(ItemType.Wood_chest, WoodChest);
            itemTypeToSprite.Add(ItemType.Stone_furnace, StoneFurnace);

            // Entities
            entityTypeToSprite.Add(EntityType.Assembler1, Assembler1Entity);
            entityTypeToSprite.Add(EntityType.WoodChest, WoodChestEntity);
            entityTypeToSprite.Add(EntityType.StoneFurnace, StoneFurnaceEntity);

            // Background tiles
            backTypeToSprite.Add(BackType.Empty, null);
            backTypeToSprite.Add(BackType.Grass1, Resources.LoadAll<Sprite>("Grass1"));

            // Ores
            oreToSprite.Add(OreType.None, null);
            oreToSprite.Add(OreType.Coal, Resources.LoadAll<Sprite>("Coal"));
            oreToSprite.Add(OreType.Copper, Resources.LoadAll<Sprite>("Copper"));
            oreToSprite.Add(OreType.Iron, Resources.LoadAll<Sprite>("Iron"));
        }



        public Sprite GetIconSprite(ItemType type)
        {
            return itemTypeToSprite[type];
        }

        public Sprite GetEntitySprite(EntityType type)
        {
            return entityTypeToSprite[type];
        }

        public Sprite GetRandomBackSprite(BackType type)
        {
            Sprite[] sprites = backTypeToSprite[type];

            return sprites[UnityEngine.Random.Range(0, sprites.Length)];
        }

        public Sprite GetRandomOreSprite(OreType type, float richnessPercent)
        {
            Sprite[] sprites = oreToSprite[type];

            //int idx = sprites.Length - 1 - (int)(sprites.Length / 100f * richnessPercent);
            //idx -= idx % 8;
            int idx = GetOreRichnessLevel(richnessPercent, sprites.Length);
            idx += UnityEngine.Random.Range(0, oreVariantsPerRichnessLevel);

            return sprites[idx];
        }

        public Sprite GetNewOreSpriteIfNeeded(OreType type, float prevRichnessPercent, float newRichnessPercent)
        {
            Sprite[] sprites = oreToSprite[type];

            int prevLevel = GetOreRichnessLevel(prevRichnessPercent, sprites.Length);
            int newLevel = GetOreRichnessLevel(newRichnessPercent, sprites.Length);

            if (prevLevel == newLevel) return null;

            return GetRandomOreSprite(type, newRichnessPercent);
        }

        public GameObject EntityTypeToPrefab(EntityType type)
        {
            return type switch
            {
                EntityType.Tree => TreePrefabs[UnityEngine.Random.Range(0, TreePrefabs.Length)],
                EntityType.Assembler1 => Assembler1Prefab,
                EntityType.WoodChest => WoodChestPrefab,
                EntityType.StoneFurnace => StoneFurnacePrefab,
                _ => throw new Exception("Missing entity prefab"),
            };
        }

        private int GetOreRichnessLevel(float richnessPercent, int levelSize)
        {
            int idx = levelSize - 1 - (int)(levelSize / 100f * richnessPercent);
            idx -= idx % oreVariantsPerRichnessLevel;

            return idx;
        }
    }
}
