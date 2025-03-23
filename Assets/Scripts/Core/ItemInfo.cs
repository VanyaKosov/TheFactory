using System.Collections.Generic;

namespace Dev.Kosov.Factory.Core
{
    public static class ItemInfo
    {
        private static readonly Dictionary<ItemType, Info> info = new()
        {
            { ItemType.Empty , new(EntityType.Empty , 0) },
            { ItemType.Wood , new(EntityType.Empty , 200) },
            { ItemType.Coal , new(EntityType.Empty , 200) },
            { ItemType.Iron_ore , new(EntityType.Empty , 200) },
            { ItemType.Copper_ore , new(EntityType.Empty , 200) },
            { ItemType.WoodChest, new(EntityType.WoodChest, 100) },
            { ItemType.Assembler1 , new(EntityType.Assembler1 , 50) }
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

            public bool Placable { get => EntityType != EntityType.Empty; }
        }
    }
}
