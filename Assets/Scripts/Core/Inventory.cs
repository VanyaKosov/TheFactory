using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    class Inventory
    {
        public int Width { get; private set; } = 10;
        public int Height { get; private set; } = 16;
        public int HotbarWidth { get; private set; } = 10;
        private readonly static Dictionary<ItemType, int> stackSizes = new()
        {
            { ItemType.Empty, 0 },
            { ItemType.Wood, 100 },
            { ItemType.Coal, 100 },
            { ItemType.Iron_ore, 100 },
            { ItemType.Copper_ore, 100 }
        };
        private readonly Slot[,] inventory;
        private readonly Slot[,] hotbar;

        public Inventory()
        {
            inventory = new Slot[Width, Height]; ;
        }

        //public bool AddItem(Vector2Int slotPos, ItemType type, int amount)
        //{
        //    Slot slot = inventory[slotPos.x, slotPos.y];
        //    if (slot.Type != type) return false;
        //    if (slot.Amount )
        //}

        private class Slot
        {
            public ItemType Type { get; set; }
            public int Amount { get; set; }

            public Slot(ItemType type, int amount)
            {
                Type = type;
                Amount = amount;
            }
        }
    }
}
