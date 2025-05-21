using System.Collections.Generic;

namespace Dev.Kosov.Factory.Core
{
    public static class ItemInfo
    {
        private static readonly Dictionary<ItemType, Info> info = new()
        {
            { ItemType.None, new(EntityType.None, 0, OreType.None) },
            { ItemType.Wood, new(EntityType.None, 200, OreType.None) },
            { ItemType.Coal, new(EntityType.None, 200, OreType.Coal) },
            { ItemType.Stone, new(EntityType.None, 200, OreType.Stone) },
            { ItemType.Stone_brick, new(EntityType.None, 200, OreType.None) },
            { ItemType.Iron_ore, new(EntityType.None, 200, OreType.Iron) },
            { ItemType.Iron_plate, new(EntityType.None, 200, OreType.None) },
            { ItemType.Copper_ore, new(EntityType.None, 200, OreType.Copper) },
            { ItemType.Copper_plate, new(EntityType.None, 200, OreType.None) },
            { ItemType.Wood_chest, new(EntityType.Wood_chest, 50, OreType.None) },
            { ItemType.Assembler_1, new(EntityType.Assembler1, 50, OreType.None) },
            { ItemType.Stone_furnace, new(EntityType.Stone_furnace, 50, OreType.None) },
            { ItemType.Copper_wire, new(EntityType.None, 100, OreType.None) },
            { ItemType.Simple_circuit, new(EntityType.None, 200, OreType.None) },
            { ItemType.Electric_drill, new(EntityType.Electric_drill, 50, OreType.None) },
            { ItemType.Inserter, new(EntityType.Inserter, 50, OreType.None) },
            { ItemType.Iron_gear, new(EntityType.None, 100, OreType.None) },
            { ItemType.Iron_chest, new(EntityType.Iron_chest, 50, OreType.None) },
            { ItemType.Steel_plate, new(EntityType.None, 200, OreType.None) },
            { ItemType.Concrete, new(EntityType.None, 200, OreType.None) },
            { ItemType.Rocket_silo, new(EntityType.Rocket_silo, 1, OreType.None) },
            { ItemType.Rocket_part, new(EntityType.None, 100, OreType.None) },
            { ItemType.Compacted_coal, new(EntityType.None, 100, OreType.None) },
            { ItemType.Rocket_fuel, new(EntityType.None, 50, OreType.None) }

        };

        public static Info Get(ItemType type)
        {
            return info[type];
        }

        public readonly struct Info
        {
            public readonly EntityType EntityType;
            public readonly int MaxStackSize;
            public readonly OreType OreType;

            internal Info(EntityType entityType, int maxStackSize, OreType oreType)
            {
                EntityType = entityType;
                MaxStackSize = maxStackSize;
                OreType = oreType;
            }

            public bool Placable { get => EntityType != EntityType.None; }
        }
    }
}
