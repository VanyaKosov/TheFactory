using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class StoneFurnace : Entity, ICrafter, ITakeable, IPuttable
    {
        private readonly Crafter Crafter;

        internal StoneFurnace(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Stone_furnace, 1) }, EntityType.Stone_furnace)
        {
            Crafter = new(new()
            {
                RecipeType.Smelt_iron_ore,
                RecipeType.Smelt_copper_ore,
                RecipeType.Smelt_stone_ore,
                RecipeType.Smelt_iron_plate
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

        internal override List<InvSlot> GetComponents()
        {
            List<InvSlot> items = base.GetComponents();
            items.AddRange(Crafter.GetComponents());

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
