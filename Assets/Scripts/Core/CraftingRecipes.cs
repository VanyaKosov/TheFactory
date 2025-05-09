using System.Collections.Generic;

namespace Dev.Kosov.Factory.Core
{
    public static class CraftingRecipes
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
                    2f
                )
            },
            {
                RecipeType.Smelt_stone_ore,
                (
                    new InvSlot[] { new(ItemType.Stone, 1) },
                    new InvSlot[] { new(ItemType.Stone_brick, 1) },
                    2f
                )
            },
            {
                RecipeType.Mine_iron_ore,
                (
                    new InvSlot[] { },
                    new InvSlot[] { new(ItemType.Iron_ore, 1) },
                    1f
                )
            },
            {
                RecipeType.Mine_copper_ore,
                (
                    new InvSlot[] { },
                    new InvSlot[] { new(ItemType.Copper_ore, 1) },
                    1f
                )
            },
            {
                RecipeType.Mine_stone_ore,
                (
                    new InvSlot[] { },
                    new InvSlot[] { new(ItemType.Stone, 1) },
                    1f
                )
            },
            {
                RecipeType.Make_copper_wire,
                (
                    new InvSlot[] { new(ItemType.Copper_plate, 2 ) },
                    new InvSlot[] { new(ItemType.Copper_wire, 1) },
                    1f
                )
            },
            {
                RecipeType.Make_simple_circuit,
                (
                    new InvSlot[] { new(ItemType.Copper_wire, 2 ), new(ItemType.Iron_plate, 1) },
                    new InvSlot[] { new(ItemType.Simple_circuit, 1) },
                    1f
                )
            },
            {
                RecipeType.Make_iron_gear,
                (
                    new InvSlot[] { new(ItemType.Iron_plate, 4) },
                    new InvSlot[] { new(ItemType.Iron_gear, 1) },
                    1f
                )
            },
            {
                RecipeType.Make_inserter,
                (
                    new InvSlot[] { new(ItemType.Iron_plate, 2), new(ItemType.Iron_gear, 1), new(ItemType.Copper_wire, 1) },
                    new InvSlot[] { new(ItemType.Inserter, 1) },
                    2f
                )
            },
            {
                RecipeType.Make_assembler1,
                (
                    new InvSlot[] { new(ItemType.Iron_plate, 4), new(ItemType.Iron_gear, 1), new(ItemType.Simple_circuit, 1) },
                    new InvSlot[] { new(ItemType.Assembler_1, 1) },
                    2f
                )
            },
            {
                RecipeType.Make_wooden_chest,
                (
                    new InvSlot[] { new(ItemType.Wood, 5) },
                    new InvSlot[] { new(ItemType.Wood_chest, 1) },
                    1f
                )
            },
            {
                RecipeType.Make_furnace,
                (
                    new InvSlot[] { new(ItemType.Stone_brick, 5) },
                    new InvSlot[] { new(ItemType.Stone_furnace, 1) },
                    3f
                )
            }
        };

        public static (InvSlot[] inputs, InvSlot[] outputs, float time) Get(RecipeType type)
        {
            return recipes[type];
        }
    }
}
