using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public class Crafter
    {
        private const int itemRequestMultiplier = 3;
        private float timeStarted;
        private float time;
        private RecipeType currentRecipe = RecipeType.None;

        internal readonly List<InvSlot> WantedItems;

        public readonly List<RecipeType> AvailableRecipes;
        public readonly Storage InputStorage = new(6, 1);
        public readonly Storage OutputStorage = new(6, 1);

        internal Crafter(List<RecipeType> availableRecipes)
        {
            AvailableRecipes = availableRecipes;
            WantedItems = new();
        }

        public float GetPercentComplete()
        {
            if (currentRecipe == RecipeType.None) return 0f;
            return (time - timeStarted) / CraftingRecipes.Get(currentRecipe).time * 100;
        }

        public InvSlot GetExpectedInputItem(Vector2Int pos)
        {
            if (currentRecipe == RecipeType.None) return new(ItemType.None, 0);

            var recipe = CraftingRecipes.Get(currentRecipe);
            if (pos.x >= recipe.inputs.Length) return new(ItemType.None, 0);

            return recipe.inputs[pos.x];
        }

        public InvSlot GetExpectedOutputItem(Vector2Int pos)
        {
            if (currentRecipe == RecipeType.None) return new(ItemType.None, 0);

            var recipe = CraftingRecipes.Get(currentRecipe);
            if (pos.x >= recipe.outputs.Length) return new(ItemType.None, 0);

            return recipe.outputs[pos.x];
        }

        public void ChangeRecipe(RecipeType recipe)
        {
            if (!AvailableRecipes.Any(a => a == recipe)) throw new Exception("Tried changing to an unsupported recipe");

            currentRecipe = recipe;
            timeStarted = time;

            UpdateWantedItems();
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

        private void UpdateWantedItems()
        {
            WantedItems.Clear();
            InvSlot[] inputs = CraftingRecipes.Get(currentRecipe).inputs;
            for (int i = 0; i < inputs.Length; i++)
            {
                InvSlot storageSlot = InputStorage.GetItem(new(i, 0));
                if (storageSlot.Type != ItemType.None && storageSlot.Type != ItemType.Empty && storageSlot.Type != inputs[i].Type) continue;
                if (storageSlot.Amount >= inputs[i].Amount * itemRequestMultiplier) continue;

                WantedItems.Add(new(inputs[i].Type, inputs[i].Amount * itemRequestMultiplier - storageSlot.Amount));
            }
        }

        private void CheckCraft()
        {
            if (currentRecipe == RecipeType.None) return;
            if (!CheckInputs())
            {
                timeStarted = time;
                return;
            }
            (InvSlot[] inputs, InvSlot[] outputs, float requiredTime) = CraftingRecipes.Get(currentRecipe);
            if (time - timeStarted < requiredTime) return;
            if (!CheckOutputs()) return;
            timeStarted = time;

            for (int i = 0; i < inputs.Length; i++)
            {
                Vector2Int pos = new(i, 0);
                InputStorage.SetItem(new(inputs[i].Type, InputStorage.GetItem(pos).Amount - inputs[i].Amount), pos);

                UpdateWantedItems();
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                Vector2Int pos = new(i, 0);
                OutputStorage.SetItem(new(outputs[i].Type, OutputStorage.GetItem(pos).Amount + outputs[i].Amount), pos);
            }
        }

        private bool CheckOutputs()
        {
            (_, InvSlot[] outputs, _) = CraftingRecipes.Get(currentRecipe);
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
            (InvSlot[] inputs, _, _) = CraftingRecipes.Get(currentRecipe);
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
