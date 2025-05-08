using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class Assembler1 : Entity, ICrafter, ITakeable, IPuttable
    {
        private readonly Crafter Crafter;

        internal Assembler1(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Assembler_1, 1) }, EntityType.Assembler1)
        {
            Crafter = new(new() 
            { 
                RecipeType.Make_copper_wire, 
                RecipeType.Make_simple_circuit, 
                RecipeType.Make_iron_gear,
                RecipeType.Make_inserter
            });
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
            items.AddRange(Crafter.GetComponents()); // Also adds ItemType.None

            return items;
        }

        InvSlot ITakeable.Take()
        {
            return Crafter.OutputStorage.AutoTake();
        }

        InvSlot ITakeable.Take(ItemType type)
        {
            return Crafter.OutputStorage.AutoTake(type);
        }

        int IPuttable.Put(InvSlot item)
        {
            return Crafter.InputStorage.AutoPut(item.Type, item.Amount);
        }

        List<InvSlot> IPuttable.GetWantedItems()
        {
            return Crafter.WantedItems;
        }
    }
}
