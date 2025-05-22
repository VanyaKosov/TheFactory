using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class ElectricDrill : Entity, ICrafter, ITakeable, IPuttable
    {
        private readonly Crafter crafter;
        private readonly Func<Vector2Int, OreType> mineOre;
        private readonly Func<Vector2Int, OreType> getOreAtPos;
        private Vector2Int miningPos;

        internal ElectricDrill(Rotation rotation, Vector2Int bottomLeftPos, Func<Vector2Int, OreType> mineOre, Func<Vector2Int, OreType> getOreAtPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Electric_drill, 1) }, EntityType.Electric_drill)
        {
            crafter = new(new()
            {
                RecipeType.Mine_iron_ore,
                RecipeType.Mine_copper_ore,
                RecipeType.Mine_stone_ore,
                RecipeType.Mine_coal_ore
            });
            crafter.CompletedCraft += MineOreIfCompleted;

            this.mineOre = mineOre;
            this.getOreAtPos = getOreAtPos;
        }

        public Crafter GetCrafter()
        {
            return crafter;
        }

        internal override void UpdateState()
        {
            base.UpdateState();
            UpdateCrafterState();
        }

        internal override List<InvSlot> GetComponents()
        {
            List<InvSlot> items = base.GetComponents();
            items.AddRange(crafter.GetComponents());

            return items;
        }

        InvSlot ITakeable.Take()
        {
            return crafter.OutputStorage.AutoTake();
        }

        InvSlot ITakeable.Take(ItemType type)
        {
            return crafter.OutputStorage.AutoTake(type);
        }

        int IPuttable.Put(InvSlot item)
        {
            return crafter.InputStorage.AutoPut(item.Type, item.Amount);
        }

        List<InvSlot> IPuttable.GetWantedItems()
        {
            return crafter.WantedItems;
        }

        private void UpdateCrafterState()
        {
            if (crafter.CurrentRecipe == RecipeType.None) return;

            InvSlot[] outputs = CraftingRecipes.Get(crafter.CurrentRecipe).outputs;
            OreType oreType = ItemInfo.Get(outputs[0].Type).OreType;

            bool found = false;
            for (int x = BottomLeftPos.x; x < BottomLeftPos.x + Size.x; x++)
            {
                if (found) break;
                for (int y = BottomLeftPos.y; y < BottomLeftPos.y + Size.y; y++)    
                {
                    if (getOreAtPos(new(x, y)) == oreType) {
                        found = true;
                        miningPos = new(x, y);
                        break;
                    }
                }
            }

            if (!found) return;

            crafter.UpdateState();
        }

        private void MineOreIfCompleted(object sender, EventArgs args)
        {
            mineOre(miningPos);
        }
    }
}
