using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core.Entities
{
    internal class ElectricDrill : Entity, ICrafter
    {
        private readonly Crafter crafter;

        internal ElectricDrill(Rotation rotation, Vector2Int bottomLeftPos) 
            : base(rotation, bottomLeftPos, new() { new(ItemType.Electric_drill, 1) }, EntityType.Electric_drill)
        {
            crafter = new(new() { RecipeType.Mine_iron_ore, RecipeType.Mine_copper_ore });
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
