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
        public Sprite Stone;
        public Sprite Stone_brick;
        public Sprite Iron_Ore;
        public Sprite Iron_Plate;
        public Sprite Copper_Ore;
        public Sprite Copper_Plate;
        public Sprite Assembler1;
        public Sprite WoodChest;
        public Sprite StoneFurnace;
        public Sprite CopperWire;
        public Sprite SimpleCircuit;
        public Sprite ElectricDrill;
        public Sprite Inserter;
        public Sprite IronGear;
        public Sprite IronChest;
        public Sprite SteelPlate;
        public Sprite Concrete;

        [Header("Entity Ghost Sprites")]
        public Sprite Assembler1Ghost;
        public Sprite WoodChestGhost;
        public Sprite StoneFurnaceGhost;
        public Sprite ElectricDrillNGhost;
        public Sprite ElectricDrillEGhost;
        public Sprite ElectricDrillSGhost;
        public Sprite ElectricDrillWGhost;
        public Sprite InserterNGhost;
        public Sprite InserterEGhost;
        public Sprite InserterSGhost;
        public Sprite InserterWGhost;
        public Sprite IronChestGhost;

        [Header("Entity Prefabs")]
        public GameObject[] TreePrefabs;
        public GameObject Assembler1Prefab;
        public GameObject WoodChestPrefab;
        public GameObject StoneFurnacePrefab;
        public GameObject ElectricDrillNPrefab;
        public GameObject ElectricDrillEPrefab;
        public GameObject ElectricDrillSPrefab;
        public GameObject ElectricDrillWPrefab;
        public GameObject InserterNPrefab;
        public GameObject InserterEPrefab;
        public GameObject InserterSPrefab;
        public GameObject InserterWPrefab;
        public GameObject IronChestPrefab;

        private readonly Dictionary<ItemType, Sprite> itemTypeToSprite = new();
        private readonly Dictionary<BackType, Sprite[]> backTypeToSprite = new();
        private readonly Dictionary<OreType, Sprite[]> oreToSprite = new();

        void Awake()
        {
            // Icons
            itemTypeToSprite.Add(ItemType.Wood, Wood);
            itemTypeToSprite.Add(ItemType.Coal, Coal);
            itemTypeToSprite.Add(ItemType.Stone, Stone);
            itemTypeToSprite.Add(ItemType.Stone_brick, Stone_brick);
            itemTypeToSprite.Add(ItemType.Iron_ore, Iron_Ore);
            itemTypeToSprite.Add(ItemType.Iron_plate, Iron_Plate);
            itemTypeToSprite.Add(ItemType.Copper_ore, Copper_Ore);
            itemTypeToSprite.Add(ItemType.Copper_plate, Copper_Plate);
            itemTypeToSprite.Add(ItemType.Assembler_1, Assembler1);
            itemTypeToSprite.Add(ItemType.Wood_chest, WoodChest);
            itemTypeToSprite.Add(ItemType.Stone_furnace, StoneFurnace);
            itemTypeToSprite.Add(ItemType.Copper_wire, CopperWire);
            itemTypeToSprite.Add(ItemType.Simple_circuit, SimpleCircuit);
            itemTypeToSprite.Add(ItemType.Electric_drill, ElectricDrill);
            itemTypeToSprite.Add(ItemType.Inserter, Inserter);
            itemTypeToSprite.Add(ItemType.Iron_gear, IronGear);
            itemTypeToSprite.Add(ItemType.Iron_chest, IronChest);
            itemTypeToSprite.Add(ItemType.Steel_plate, SteelPlate);
            itemTypeToSprite.Add(ItemType.Concrete, Concrete);

            // Background tiles
            backTypeToSprite.Add(BackType.Empty, null);
            backTypeToSprite.Add(BackType.Grass1, Resources.LoadAll<Sprite>("Grass1"));

            // Ores
            oreToSprite.Add(OreType.None, null);
            oreToSprite.Add(OreType.Coal, Resources.LoadAll<Sprite>("Coal"));
            oreToSprite.Add(OreType.Copper, Resources.LoadAll<Sprite>("Copper"));
            oreToSprite.Add(OreType.Iron, Resources.LoadAll<Sprite>("Iron"));
            oreToSprite.Add(OreType.Stone, Resources.LoadAll<Sprite>("Stone"));
        }



        public Sprite GetIconSprite(ItemType type)
        {
            return itemTypeToSprite[type];
        }

        public Sprite GetEntitySprite(EntityType type, Rotation rotation)
        {
            return type switch
            {
                EntityType.Assembler1 => Assembler1Ghost,
                EntityType.Wood_chest => WoodChestGhost,
                EntityType.Stone_furnace => StoneFurnaceGhost,
                EntityType.Electric_drill => rotation switch
                {
                    Rotation.Up => ElectricDrillNGhost,
                    Rotation.Right => ElectricDrillEGhost,
                    Rotation.Down => ElectricDrillSGhost,
                    Rotation.Left => ElectricDrillWGhost,
                    _ => throw new Exception("Unknown rotation")
                },
                EntityType.Inserter => rotation switch
                {
                    Rotation.Up => InserterNGhost,
                    Rotation.Right => InserterEGhost,
                    Rotation.Down => InserterSGhost,
                    Rotation.Left => InserterWGhost,
                    _ => throw new Exception("Unknown rotation")
                },
                EntityType.Iron_chest => IronChestGhost,
                _ => throw new Exception("Missing entity ghost")
            };
        }

        public Sprite GetRandomBackSprite(BackType type)
        {
            Sprite[] sprites = backTypeToSprite[type];

            return sprites[UnityEngine.Random.Range(0, sprites.Length)];
        }

        public Sprite GetRandomOreSprite(OreType type, float richnessPercent)
        {
            Sprite[] sprites = oreToSprite[type];

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

        public GameObject EntityTypeToPrefab(EntityType type, Rotation rotation)
        {
            return type switch
            {
                EntityType.Tree => TreePrefabs[UnityEngine.Random.Range(0, TreePrefabs.Length)],
                EntityType.Assembler1 => Assembler1Prefab,
                EntityType.Wood_chest => WoodChestPrefab,
                EntityType.Stone_furnace => StoneFurnacePrefab,
                EntityType.Electric_drill => rotation switch
                {
                    Rotation.Up => ElectricDrillNPrefab,
                    Rotation.Right => ElectricDrillEPrefab,
                    Rotation.Down => ElectricDrillSPrefab,
                    Rotation.Left => ElectricDrillWPrefab,
                    _ => throw new Exception("Unknown rotation")
                },
                EntityType.Inserter => rotation switch
                {
                    Rotation.Up => InserterNPrefab,
                    Rotation.Right => InserterEPrefab,
                    Rotation.Down => InserterSPrefab,
                    Rotation.Left => InserterWPrefab,
                    _ => throw new Exception("Unknown rotation")
                },
                EntityType.Iron_chest => IronChestPrefab,
                _ => throw new Exception("Missing entity prefab")
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
