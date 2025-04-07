using System.Collections.Generic;
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
            /*Storage inputStorage = crafter.InputStorage;
            for (int x = 0; x < inputStorage.Width; x++)
            {
                for (int y = 0; y < inputStorage.Height; y++)
                {
                    items.Add(inputStorage.GetItem(new(x, y)));
                }
            }

            Storage outputStorage = crafter.OutputStorage;
            for (int x = 0; x < outputStorage.Width; x++)
            {
                for (int y = 0; y < outputStorage.Height; y++)
                {
                    items.Add(outputStorage.GetItem(new(x, y)));
                }

            }*/

            return items;
        }
    }
}
