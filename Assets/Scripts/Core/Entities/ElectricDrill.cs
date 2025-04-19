﻿using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class ElectricDrill : Entity, ICrafter
    {
        private readonly Crafter Crafter;

        internal ElectricDrill(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Electric_drill, 1) }, EntityType.Electric_drill)
        {
            Crafter = new(new() { RecipeType.Mine_iron_ore, RecipeType.Mine_copper_ore });
        }

        public Crafter GetCrafter()
        {
            return Crafter;
        }

        internal override void UpdateState()
        {
            base.UpdateState();
            Crafter.UpdateState();
        }

        override internal List<InvSlot> GetComponents()
        {
            List<InvSlot> items = base.GetComponents();
            items.AddRange(Crafter.GetComponents());

            return items;
        }
    }
}
