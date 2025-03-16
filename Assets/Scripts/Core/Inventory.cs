using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
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
            { ItemType.Copper_ore, 100 }
        };
        private readonly Slot[,] inventory;
        private readonly Slot[,] hotbar;
        private Slot cursorSlot;

        public event EventHandler<SetItemEventArgs> SetInvItem;
        public event EventHandler<SetItemEventArgs> SetHotbarItem;
        public event EventHandler<SetCursorEventArgs> SetCursorItem;

        public Inventory()
        {
            inventory = new Slot[Width, Height];
            hotbar = new Slot[HotbarWidth, HotbarHeight];
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

        //public void PutToInventory(Vector2Int pos)
        //{
        //    (cursorSlot, inventory[pos.x, pos.y]) = (inventory[pos.x, pos.y], cursorSlot);
        //}

        //public void PutToHotbar(Vector2Int pos)
        //{
        //    (cursorSlot, hotbar[pos.x, pos.y]) = (hotbar[pos.x, pos.y], cursorSlot);
        //}

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
        }

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

        public class SetCursorEventArgs : EventArgs
        {
            public readonly ItemType Type;
            public readonly int Amount;

            public SetCursorEventArgs(ItemType type, int amount)
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

            public SetItemEventArgs(Vector2Int pos, ItemType type, int amount)
            {
                Pos = pos;
                Type = type;
                Amount = amount;
            }
        }
    }
}
