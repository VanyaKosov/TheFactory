using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class ElectricDrill : Entity, ICrafter, ITakeable, IPuttable
    {
        private readonly Crafter Crafter;

        internal ElectricDrill(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Electric_drill, 1) }, EntityType.Electric_drill)
        {
            Crafter = new(new()
            {
                RecipeType.Mine_iron_ore,
                RecipeType.Mine_copper_ore,
                RecipeType.Mine_stone_ore,
                RecipeType.Mine_coal_ore
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
