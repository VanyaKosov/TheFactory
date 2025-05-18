using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public static class EntityInfo
    {
        private static readonly Dictionary<EntityType, Info> info = new()
        {
            { EntityType.Tree, new(ItemType.None, new(2, 2)) },
            { EntityType.Assembler1, new(ItemType.Assembler_1, new(3, 3)) },
            { EntityType.Wood_chest, new(ItemType.Wood_chest, new(1, 1)) },
            { EntityType.Stone_furnace, new(ItemType.Stone_furnace, new(2, 2)) },
            { EntityType.Electric_drill, new(ItemType.Electric_drill, new(3, 3)) },
            { EntityType.Inserter, new(ItemType.Inserter, new(1, 1)) },
            { EntityType.Iron_chest, new(ItemType.Iron_chest, new(1, 1)) },
            { EntityType.Rocket_silo, new(ItemType.Rocket_silo, new(9, 20)) }
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
