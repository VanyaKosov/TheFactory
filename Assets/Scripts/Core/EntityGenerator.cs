using Dev.Kosov.Factory.Core.Entities;
using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Core.Assets.Scripts.Core
{
    internal class EntityGenerator
    {
        internal static Entity GenEntityInstance(EntityType type, Vector2Int pos, Rotation rotation)
        {
            return type switch
            {
                EntityType.Tree => new Entity(rotation, pos, new() { new(ItemType.Wood, 4) }, EntityType.Tree),
                EntityType.Assembler1 => new Assembler1(rotation, pos),
                EntityType.WoodChest => new WoodChest(rotation, pos),
                EntityType.StoneFurnace => new StoneFurnace(rotation, pos),
                EntityType.Electric_drill => new ElectricDrill(rotation, pos),
                EntityType.Inserter => new Inserter(rotation, pos),
                _ => throw new Exception("Missing entity class")
            };
        }
    }
}
