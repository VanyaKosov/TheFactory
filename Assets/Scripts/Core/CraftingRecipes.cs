using System.Collections.Generic;

namespace Dev.Kosov.Factory.Core
{
    static class CraftingRecipes
    {
        private static readonly Dictionary<RecipeType, (InvSlot[] inputs, InvSlot[] outputs, float time)> recipes = new()
        {
            {
                RecipeType.Smelt_iron_ore,
                (
                    new InvSlot[] { new(ItemType.Iron_ore, 1) },
                    new InvSlot[] { new(ItemType.Iron_plate, 1) },
                    2f
                )
            },
            {
                RecipeType.Smelt_copper_ore,
                (
                    new InvSlot[] { new(ItemType.Copper_ore, 1) },
                    new InvSlot[] { new(ItemType.Copper_plate, 1) },
                    2f // 2f
                )
            },
            {
                RecipeType.Mine_iron_ore,
                (
                    new InvSlot[] { new() },
                    new InvSlot[] { new(ItemType.Iron_ore, 1) },
                    1f
                )
            },
            {
                RecipeType.Mine_copper_ore,
                (
                    new InvSlot[] { new() },
                    new InvSlot[] { new(ItemType.Copper_ore, 1) },
                    1f
                )
            }
        };

        internal static (InvSlot[] inputs, InvSlot[] outputs, float time) Get(RecipeType type)
        {
            return recipes[type];
        }
    }
}
