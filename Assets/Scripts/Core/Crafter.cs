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

        internal RecipeType CurrentRecipe { private set; get; }
        internal readonly List<InvSlot> WantedItems;

        public readonly List<RecipeType> AvailableRecipes;
        public readonly Storage InputStorage;
        public readonly Storage OutputStorage;

        public event EventHandler<EventArgs> CompletedCraft;

        internal Crafter(List<RecipeType> availableRecipes, bool canTakeFromInput = true, bool canTakeFromOutput = true)
        {
            CurrentRecipe = RecipeType.None;
            AvailableRecipes = availableRecipes;
            WantedItems = new();
            InputStorage = new(6, 1, canTakeFromInput);
            OutputStorage = new(6, 1, canTakeFromOutput);
        }

        public float GetPercentComplete()
        {
            if (CurrentRecipe == RecipeType.None) return 0f;
            return (time - timeStarted) / CraftingRecipes.Get(CurrentRecipe).time * 100;
        }

        public InvSlot GetExpectedInputItem(Vector2Int pos)
        {
            if (CurrentRecipe == RecipeType.None) return new(ItemType.None, 0);

            var recipe = CraftingRecipes.Get(CurrentRecipe);
            if (pos.x >= recipe.inputs.Length) return new(ItemType.None, 0);

            return recipe.inputs[pos.x];
        }

        public InvSlot GetExpectedOutputItem(Vector2Int pos)
        {
            if (CurrentRecipe == RecipeType.None) return new(ItemType.None, 0);

            var recipe = CraftingRecipes.Get(CurrentRecipe);
            if (pos.x >= recipe.outputs.Length) return new(ItemType.None, 0);

            return recipe.outputs[pos.x];
        }

        public void ChangeRecipe(RecipeType recipe)
        {
            if (!AvailableRecipes.Any(a => a == recipe)) throw new Exception("Tried changing to an unsupported recipe");

            CurrentRecipe = recipe;
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
                    if (inputStorage.GetItem(new(x, y)).Type == ItemType.None) continue;
                    items.Add(inputStorage.GetItem(new(x, y)));
                }
            }

            Storage outputStorage = OutputStorage;
            for (int x = 0; x < outputStorage.Width; x++)
            {
                for (int y = 0; y < outputStorage.Height; y++)
                {
                    if (outputStorage.GetItem(new(x, y)).Type == ItemType.None) continue;
                    items.Add(outputStorage.GetItem(new(x, y)));
                }

            }

            return items;
        }

        internal void UpdateState()
        {
            time = Time.time;

            CheckCraft();

            UpdateInputStorageReserve();

            if (CurrentRecipe != RecipeType.None)
            {
                UpdateWantedItems();
            }
        }

        private void UpdateInputStorageReserve()
        {
            if (CurrentRecipe == RecipeType.None) return;
            var recipe = CraftingRecipes.Get(CurrentRecipe);

            if (recipe.inputs.Length == 0) return;

            int idx = 0;
            for (int x = 0; x < InputStorage.Width; x++)
            {
                for (int y = 0; y < InputStorage.Height; y++)
                {
                    InputStorage.SetReserve(recipe.inputs[idx].Type, new(x, y));

                    idx++;
                    if (idx >= recipe.inputs.Length) return;
                }
            }
        }

        private void UpdateWantedItems()
        {
            WantedItems.Clear();
            InvSlot[] inputs = CraftingRecipes.Get(CurrentRecipe).inputs;
            for (int i = 0; i < inputs.Length; i++)
            {
                InvSlot storageSlot = InputStorage.GetItem(new(i, 0));
                if (storageSlot.Type == ItemType.None)
                {
                    WantedItems.Add(new(inputs[i].Type, inputs[i].Amount * itemRequestMultiplier));
                    continue;
                }
                if (storageSlot.Type != inputs[i].Type) continue;
                if (storageSlot.Amount >= inputs[i].Amount * itemRequestMultiplier) continue;

                WantedItems.Add(new(inputs[i].Type, inputs[i].Amount * itemRequestMultiplier - storageSlot.Amount));
            }
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

                UpdateWantedItems();
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                Vector2Int pos = new(i, 0);
                OutputStorage.SetItem(new(outputs[i].Type, OutputStorage.GetItem(pos).Amount + outputs[i].Amount), pos);
            }

            CompletedCraft?.Invoke(this, new());
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
