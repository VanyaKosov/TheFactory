using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class StoneFurnace : Entity, ICrafter
    {
        private readonly Crafter Crafter;

        internal StoneFurnace(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Stone_furnace, 1) }, EntityType.StoneFurnace)
        {
            Crafter = new(new() { RecipeType.Smelt_iron_ore, RecipeType.Smelt_copper_ore });
        }

        public Crafter GetCrafter()
        {
            return Crafter;
        }

        override internal List<InvSlot> GetComponents()
        {
            List<InvSlot> items = base.GetComponents();
            items.AddRange(Crafter.GetComponents());

            return items;
        }
    }
}
