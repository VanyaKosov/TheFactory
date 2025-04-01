using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public class Inventory
    {
        private readonly Storage inventory;
        private readonly Storage hotbar;

        internal InvSlot CursorSlot { get; private set; }

        public int Width { get; private set; } = 10;
        public int Height { get; private set; } = 16;
        public int HotbarWidth { get; private set; } = 10;
        public int HotbarHeight { get; private set; } = 1;

        public event EventHandler<SetItemEventArgs> SetInvItem;
        public event EventHandler<SetItemEventArgs> SetHotbarItem;
        public event EventHandler<SetCursorEventArgs> SetCursorItem;

        public Inventory()
        {
            inventory = new(Width, Height);
            hotbar = new(HotbarWidth, HotbarHeight);
            CursorSlot = new(ItemType.None, 0);

            inventory.SlotChanged += OnInventorySlotChange;
            hotbar.SlotChanged += OnHotbarSlotChange;

        }

        public void Run()
        {
            DefaultInvInitialize();
        }

        public void SwitchItemWithInventory(Vector2Int pos)
        {
            InvSlot temp = new(CursorSlot);
            CursorSlot = inventory.GetItem(pos);
            inventory.SetItem(temp, pos);
            SetCursorItem?.Invoke(this, new(CursorSlot.Type, CursorSlot.Amount));
        }

        public void SwitchItemWithHotbar(Vector2Int pos)
        {
            InvSlot temp = new(CursorSlot);
            CursorSlot = hotbar.GetItem(pos);
            hotbar.SetItem(temp, pos);
            SetCursorItem?.Invoke(this, new(CursorSlot.Type, CursorSlot.Amount));
        }

        internal int AddItemToInventory(ItemType type, int amount)
        {
            return inventory.AutoPut(type, amount);
        }

        internal int AddItemToCursor(ItemType type, int amount) // Returns remainder
        {
            if (CursorSlot.Type != type) return amount;

            int maxStackSize = ItemInfo.Get(type).MaxStackSize;
            int overflow = CursorSlot.Amount + amount - maxStackSize;
            if (overflow <= 0)
            {
                CursorSlot.Amount += amount;
                return 0;
            }

            CursorSlot.Amount = maxStackSize;
            return amount;
        }

        private void OnInventorySlotChange(object sender, Storage.SlotCahangedEventArgs args)
        {
            SetInvItem?.Invoke(this, new(args.Pos, args.Type, args.Amount));
        }

        private void OnHotbarSlotChange(object sender, Storage.SlotCahangedEventArgs args)
        {
            SetHotbarItem?.Invoke(this, new(args.Pos, args.Type, args.Amount));
        }

        private void DefaultInvInitialize()
        {
            AddItemToInventory(ItemType.Assembler1, 50);
            AddItemToInventory(ItemType.WoodChest, 200);
            AddItemToInventory(ItemType.StoneFurnace, 100);
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
