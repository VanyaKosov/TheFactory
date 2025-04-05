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
            Storage inputSorage = crafter.InputStorage;
            for (int x = 0; x < inputSorage.Width; x++)
            {
                for (int y = 0; y < inputSorage.Height; y++)
                {
                    items.Add(inputSorage.GetItem(new(x, y)));
                }
            }

            Storage outputSorage = crafter.OutputStorage;
            for (int x = 0; x < outputSorage.Width; x++)
            {
                for (int y = 0; y < outputSorage.Height; y++)
                {
                    items.Add(outputSorage.GetItem(new(x, y)));
                }

            }

            return items;
        }
    }
}
