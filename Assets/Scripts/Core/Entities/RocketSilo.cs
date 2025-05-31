using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public class RocketSilo : Entity, ICrafter, IPuttable
    {
        private const int partsNeededForLaunch = 50;
        private readonly Crafter crafter;
        private bool rocketReady = true;

        public event EventHandler<EventArgs> RocketLaunch;

        internal RocketSilo(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Rocket_silo, 1) }, EntityType.Rocket_silo)
        {
            crafter = new(new()
            {
                RecipeType.Make_rocket_part
            },
            true, false);
        }

        public Crafter GetCrafter()
        {
            return crafter;
        }

        internal override void UpdateState()
        {
            base.UpdateState();
            if (rocketReady)
            {
                crafter.UpdateState();
                CheckRocketCompletion();
            }
        }

        internal override List<InvSlot> GetComponents()
        {
            List<InvSlot> items = base.GetComponents();
            items.AddRange(crafter.GetComponents());

            return items;
        }

        int IPuttable.Put(InvSlot item)
        {
            return crafter.InputStorage.AutoPut(item.Type, item.Amount);
        }

        List<InvSlot> IPuttable.GetWantedItems()
        {
            return crafter.WantedItems;
        }

        private void CheckRocketCompletion()
        {
            InvSlot slot = crafter.OutputStorage.GetItem(new(0, 0));
            if (slot.Amount < partsNeededForLaunch) return;

            crafter.OutputStorage.SetItem(new(slot.Type, slot.Amount - partsNeededForLaunch), new(0, 0));
            RocketLaunch?.Invoke(this, new());
            rocketReady = false;
        }
    }
}
