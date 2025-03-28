using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public static class EntityInfo
    {
        private static readonly Dictionary<EntityType, Info> info = new()
        {
            { EntityType.Tree, new(ItemType.Empty, new(2, 2)) },
            { EntityType.Assembler1, new(ItemType.Assembler1, new(3, 3)) },
            { EntityType.WoodChest, new(ItemType.WoodChest, new(1, 1)) },
            { EntityType.StoneFurnace, new(ItemType.StoneFurnace, new(2, 2)) }
        };

        public static Info Get(EntityType type)
        {
            return info[type];
        }

        public readonly struct Info
        {
            public readonly ItemType ItemType;
            public readonly Vector2Int Size;

            internal Info(ItemType itemType, Vector2Int size)
            {
                ItemType = itemType;
                Size = size;
            }
        }
    }
}
