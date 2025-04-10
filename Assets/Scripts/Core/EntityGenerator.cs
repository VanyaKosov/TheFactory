﻿using Dev.Kosov.Factory.Core.Entities;
using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Core.Assets.Scripts.Core
{
    internal class EntityGenerator
    {
        internal static Entity GenEntityInstance(EntityType type, Vector2Int pos)
        {
            return type switch
            {
                EntityType.Tree => new Entity(Rotation.Up, pos, new() { new(ItemType.Wood, 4) }, EntityType.Tree),
                EntityType.Assembler1 => new Assembler1(Rotation.Up, pos),
                EntityType.WoodChest => new WoodChest(Rotation.Up, pos),
                EntityType.StoneFurnace => new StoneFurnace(Rotation.Up, pos),
                EntityType.Electric_drill => new ElectricDrill(Rotation.Up, pos),
                _ => throw new Exception("Missing entity class"),
            };
        }
    }
}
