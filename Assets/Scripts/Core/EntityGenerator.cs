using Dev.Kosov.Factory.Core;
using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Core.Assets.Scripts.Core
{
    internal class EntityGenerator
    {
        internal static Entity GenEntityInstance(EntityType type, Vector2Int pos, Rotation rotation, Func<Vector2Int, Entity> getEntityAtPos)
        {
            return type switch
            {
                EntityType.Tree => new Entity(rotation, pos, new() { new(ItemType.Wood, 4) }, EntityType.Tree),
                EntityType.Assembler1 => new Assembler1(rotation, pos),
                EntityType.Wood_chest => new Chest(rotation, pos, 6, 2, EntityInfo.Get(type).ItemType),
                EntityType.Stone_furnace => new StoneFurnace(rotation, pos),
                EntityType.Electric_drill => new ElectricDrill(rotation, pos),
                EntityType.Inserter => new Inserter(rotation, pos, getEntityAtPos),
                EntityType.Iron_chest => new Chest(rotation, pos, 6, 4, EntityInfo.Get(type).ItemType),
                _ => throw new Exception("Missing entity class")
            };
        }
    }
}
