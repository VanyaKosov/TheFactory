using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class Assembler1 : Entity, ICrafter
    {
        private readonly Crafter Crafter;

        internal Assembler1(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Assembler_1, 1) }, EntityType.Assembler1)
        {
            Crafter = new(new() { RecipeType.Make_copper_wire, RecipeType.Make_simple_circuit });
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
