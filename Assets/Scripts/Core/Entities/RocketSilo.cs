using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class RocketSilo : Entity, ICrafter, IPuttable
    {
        private readonly Crafter Crafter;

        internal RocketSilo(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Rocket_silo, 1) }, EntityType.Rocket_silo)
        {
            Crafter = new(new()
            {

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
