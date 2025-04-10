﻿using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core.Entities
{
    internal class StoneFurnace : Entity, ICrafter
    {
        private readonly Crafter crafter;

        internal StoneFurnace(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Stone_furnace, 1) }, EntityType.StoneFurnace)
        {
            crafter = new(new() { RecipeType.Smelt_iron_ore, RecipeType.Smelt_copper_ore });
        }

        public Crafter GetCrafter()
        {
            return crafter;
        }

        override internal List<InvSlot> GetComponents()
        {
            List<InvSlot> items = base.GetComponents();
            items.AddRange(crafter.GetComponents());

            return items;
        }
    }
}
