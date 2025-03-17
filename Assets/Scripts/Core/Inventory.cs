using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public class Inventory
    {
        public int Width { get; private set; } = 10;
        public int Height { get; private set; } = 16;
        public int HotbarWidth { get; private set; } = 10;
        public int HotbarHeight { get; private set; } = 1;
        private readonly static Dictionary<ItemType, int> stackSizes = new()
        {
            { ItemType.Empty, 0 },
            { ItemType.Wood, 100 },
            { ItemType.Coal, 100 },
            { ItemType.Iron_ore, 100 },
            { ItemType.Copper_ore, 100 },
            {ItemType.Assembler1, 100 }
        };
        private readonly InvSlot[,] inventory;
        private readonly InvSlot[,] hotbar;
        internal readonly static Dictionary<ItemType, bool> isPlacable = new()
        {
            { ItemType.Empty, false },
            { ItemType.Wood, false },
            { ItemType.Coal, false },
            { ItemType.Iron_ore, false },
            { ItemType.Copper_ore, false },
            {ItemType.Assembler1, true }
        };
        internal InvSlot cursorSlot;

        public event EventHandler<SetItemEventArgs> SetInvItem;
        public event EventHandler<SetItemEventArgs> SetHotbarItem;
        public event EventHandler<SetCursorEventArgs> SetCursorItem;

        public Inventory()
        {
            inventory = new InvSlot[Width, Height];
            hotbar = new InvSlot[HotbarWidth, HotbarHeight];
            cursorSlot = new(ItemType.Empty, 0);
        }

        public void Run()
        {
            DefaultInvInitialize();
        }

        public void SwitchItemWithInventory(Vector2Int pos)
        {
            (cursorSlot, inventory[pos.x, pos.y]) = (inventory[pos.x, pos.y], cursorSlot);
            SetInvItem?.Invoke(this, new(pos, inventory[pos.x, pos.y].Type, inventory[pos.x, pos.y].Amount));
            SetCursorItem?.Invoke(this, new(cursorSlot.Type, cursorSlot.Amount));
        }

        public void SwitchItemWithHotbar(Vector2Int pos)
        {
            (cursorSlot, hotbar[pos.x, pos.y]) = (hotbar[pos.x, pos.y], cursorSlot);
            SetHotbarItem?.Invoke(this, new(pos, hotbar[pos.x, pos.y].Type, hotbar[pos.x, pos.y].Amount));
            SetCursorItem?.Invoke(this, new(cursorSlot.Type, cursorSlot.Amount));
        }

        internal int AddItemToCursor(ItemType type, int amount) // Returns remainder
        {
            if (cursorSlot.Type != type) return amount;

            int overflow = cursorSlot.Amount + amount - stackSizes[type];
            if (overflow <= 0)
            {
                cursorSlot.Amount += amount;
                return 0;
            }

            cursorSlot.Amount = stackSizes[type];
            return amount;
        }

        internal int TakeItemFromCursor(ItemType type, int amount) // Returns remainder
        {
            if (cursorSlot.Type != type) return amount;

            if (amount <= cursorSlot.Amount)
            {
                int removedAmount = cursorSlot.Amount;
                cursorSlot.Amount = 0;
                cursorSlot.Type = ItemType.Empty;
                return removedAmount;
            }

            cursorSlot.Amount -= amount;
            return 0;
        }

        private void DefaultInvInitialize()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    inventory[x, y] = new(ItemType.Empty, 0);
                }
            }

            for (int x = 0; x < HotbarWidth; x++)
            {
                for (int y = 0; y < HotbarHeight; y++)
                {
                    hotbar[x, y] = new(ItemType.Empty, 0);
                }
            }

            inventory[0, 0] = new(ItemType.Assembler1, 100);
            SetInvItem?.Invoke(this, new(new(0, 0), ItemType.Assembler1, 100));
        }

        

        public class SetCursorEventArgs : EventArgs
        {
            public readonly ItemType Type;
            public readonly int Amount;

            internal SetCursorEventArgs(ItemType type, int amount)
            {
                Type = type;
                Amount = amount;
            }
        }

        public class SetItemEventArgs : EventArgs
        {
            public readonly Vector2Int Pos;
            public readonly ItemType Type;
            public readonly int Amount;

            internal SetItemEventArgs(Vector2Int pos, ItemType type, int amount)
            {
                Pos = pos;
                Type = type;
                Amount = amount;
            }
        }
    }
}
