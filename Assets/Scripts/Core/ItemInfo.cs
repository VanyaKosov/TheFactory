using System.Collections.Generic;

namespace Dev.Kosov.Factory.Core
{
    public static class ItemInfo
    {
        private static readonly Dictionary<ItemType, Info> info = new()
        {
            { ItemType.None, new(EntityType.None, 0) },
            { ItemType.Wood, new(EntityType.None, 200) },
            { ItemType.Coal, new(EntityType.None, 200) },
            { ItemType.Stone, new(EntityType.None, 200) },
            { ItemType.Stone_brick, new(EntityType.None, 200) },
            { ItemType.Iron_ore, new(EntityType.None, 200) },
            { ItemType.Iron_plate, new(EntityType.None, 200) },
            { ItemType.Copper_ore, new(EntityType.None, 200) },
            { ItemType.Copper_plate, new(EntityType.None, 200) },
            { ItemType.Wood_chest, new(EntityType.Wood_chest, 50) },
            { ItemType.Assembler_1, new(EntityType.Assembler1, 50) },
            { ItemType.Stone_furnace, new(EntityType.Stone_furnace, 50) },
            { ItemType.Copper_wire, new(EntityType.None, 100) },
            { ItemType.Simple_circuit, new(EntityType.None, 200) },
            { ItemType.Electric_drill, new(EntityType.Electric_drill, 50) },
            { ItemType.Inserter, new(EntityType.Inserter, 50) },
            { ItemType.Iron_gear, new(EntityType.None, 100) },
            { ItemType.Iron_chest, new(EntityType.Iron_chest, 50) },
        };

        public static Info Get(ItemType type)
        {
            return info[type];
        }

        public readonly struct Info
        {
            public readonly EntityType EntityType;
            public readonly int MaxStackSize;

            internal Info(EntityType entityType, int maxStackSize)
            {
                EntityType = entityType;
                MaxStackSize = maxStackSize;
            }

            public bool Placable { get => EntityType != EntityType.None; }
        }
    }
}
