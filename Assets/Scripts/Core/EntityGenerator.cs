using Dev.Kosov.Factory.Core.Assets.Scripts.Core.Entities;
using Dev.Kosov.Factory.Core.Entities;
using static UnityEngine.EventSystems.EventTrigger;
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
                EntityType.Tree => new Entity(Rotation.Up, pos, new() { ItemType.Wood }, new() { 10 }, EntityType.Tree),
                EntityType.Assembler1 => new Assembler1(Rotation.Up, pos),
                EntityType.WoodChest => new WoodChest(Rotation.Up, pos),
                EntityType.StoneFurnace => new StoneFurnace(Rotation.Up, pos),
                _ => throw new Exception("Missing entity class"),
            };
        }
    }
}
