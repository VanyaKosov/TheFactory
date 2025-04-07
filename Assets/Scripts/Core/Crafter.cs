using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public class Crafter
    {
        private float timeStarted;
        private float time;
        private RecipeType CurrentRecipe { get; set; } = RecipeType.None;

        public readonly List<RecipeType> AvailableRecipes;
        public readonly Storage InputStorage = new(6, 1);
        public readonly Storage OutputStorage = new(6, 1);

        internal Crafter(List<RecipeType> availableRecipes)
        {
            AvailableRecipes = availableRecipes;
        }

        public float GetPercentComplete()
        {
            if (CurrentRecipe == RecipeType.None) return 0f;
            return (time - timeStarted) / CraftingRecipes.Get(CurrentRecipe).time * 100;
        }

        public ItemType GetExpectedInputItem(Vector2Int pos)
        {
            if (CurrentRecipe == RecipeType.None) return ItemType.None;

            var recipe = CraftingRecipes.Get(CurrentRecipe);
            if (pos.x >= recipe.inputs.Length) return ItemType.None;

            return recipe.inputs[pos.x].Type;
        }

        public ItemType GetExpectedOutputItem(Vector2Int pos)
        {
            if (CurrentRecipe == RecipeType.None) return ItemType.None;

            var recipe = CraftingRecipes.Get(CurrentRecipe);
            if (pos.x >= recipe.outputs.Length) return ItemType.None;

            return recipe.outputs[pos.x].Type;
        }

        public void ChangeRecipe(RecipeType recipe)
        {
            if (!AvailableRecipes.Any(a => a == recipe)) throw new Exception("Tried changing to an unsupported recipe");

            CurrentRecipe = recipe;
            timeStarted = time;
        }

        internal List<InvSlot> GetComponents()
        {
            List<InvSlot> items = new();
            Storage inputStorage = InputStorage;
            for (int x = 0; x < inputStorage.Width; x++)
            {
                for (int y = 0; y < inputStorage.Height; y++)
                {
                    items.Add(inputStorage.GetItem(new(x, y)));
                }
            }

            Storage outputStorage = OutputStorage;
            for (int x = 0; x < outputStorage.Width; x++)
            {
                for (int y = 0; y < outputStorage.Height; y++)
                {
                    items.Add(outputStorage.GetItem(new(x, y)));
                }

            }

            return items;
        }

        internal void UpdateState()
        {
            time = Time.time;

            CheckCraft();
        }

        private void CheckCraft()
        {
            if (CurrentRecipe == RecipeType.None) return;
            if (!CheckInputs())
            {
                timeStarted = time;
                return;
            }
            (InvSlot[] inputs, InvSlot[] outputs, float requiredTime) = CraftingRecipes.Get(CurrentRecipe);
            if (time - timeStarted < requiredTime) return;
            if (!CheckOutputs()) return;
            timeStarted = time;

            for (int i = 0; i < inputs.Length; i++)
            {
                Vector2Int pos = new(i, 0);
                InputStorage.SetItem(new(inputs[i].Type, InputStorage.GetItem(pos).Amount - inputs[i].Amount), pos);
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                Vector2Int pos = new(i, 0);
                OutputStorage.SetItem(new(outputs[i].Type, OutputStorage.GetItem(pos).Amount + outputs[i].Amount), pos);
            }
        }

        private bool CheckOutputs()
        {
            (_, InvSlot[] outputs, _) = CraftingRecipes.Get(CurrentRecipe);
            for (int i = 0; i < outputs.Length; i++)
            {
                InvSlot storageSlot = OutputStorage.GetItem(new(i, 0));
                if (storageSlot.Type == ItemType.None) continue;
                if (storageSlot.Type != outputs[i].Type) return false;
                if (storageSlot.Amount + outputs[i].Amount > ItemInfo.Get(storageSlot.Type).MaxStackSize) return false;
            }

            return true;
        }

        private bool CheckInputs()
        {
            (InvSlot[] inputs, _, _) = CraftingRecipes.Get(CurrentRecipe);
            for (int i = 0; i < inputs.Length; i++)
            {
                InvSlot storageSlot = InputStorage.GetItem(new(i, 0));
                if (storageSlot.Type != inputs[i].Type) return false;
                if (storageSlot.Amount < inputs[i].Amount) return false;
            }

            return true;
        }
    }
}
