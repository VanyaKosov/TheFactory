﻿using System;
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
            inventory = new(Width, Height, true);
            hotbar = new(HotbarWidth, HotbarHeight, true);
            CursorSlot = new(ItemType.None, 0);

            inventory.SlotChanged += OnInventorySlotChange;
            hotbar.SlotChanged += OnHotbarSlotChange;

        }

        public void Run()
        {
            DefaultInvInitialize();
        }

        public void TryTakeHalfFromInventory(Vector2Int pos)
        {
            TryTakeHalfFromStorage(pos, inventory);
        }

        public void TryTakeHalfFromHotbar(Vector2Int pos)
        {
            TryTakeHalfFromStorage(pos, hotbar);
        }

        public void TryPutToInventory(Vector2Int pos)
        {
            TryPutToStorage(pos, inventory);
        }

        public void TryPutToHotbar(Vector2Int pos)
        {
            TryPutToStorage(pos, hotbar);
        }

        public void TryTakeHalfFromStorage(Vector2Int pos, Storage storage)
        {
            if (!storage.CanTake) return;
            if (CursorSlot.Type != ItemType.None) return;
            InvSlot item = storage.GetItem(pos);
            if (item.Type == ItemType.None) return;
            int toTake = item.Amount / 2;
            if (toTake == 0) return;

            SetCursorSlot(item.Type, toTake);
            storage.SetItem(new(item.Type, item.Amount - toTake), pos);
        }

        public void TryPutToStorage(Vector2Int pos, Storage storage)
        {
            if (storage.GetItem(pos).Type == CursorSlot.Type)
            {
                int remainder = storage.TryStack(CursorSlot, pos);
                if (remainder == 0)
                {
                    SetCursorSlot(ItemType.None, 0);
                    return;
                }

                SetCursorSlot(CursorSlot.Type, remainder);
                return;
            }

            SwitchItemWithStorage(pos, storage);
        }

        public void SwitchItemWithStorage(Vector2Int pos, Storage storage)
        {
            if (!storage.CanTake) return;
            InvSlot temp = CursorSlot;
            CursorSlot = storage.GetItem(pos);
            storage.SetItem(temp, pos);
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
                CursorSlot = new(CursorSlot.Type, CursorSlot.Amount + amount);
                return 0;
            }

            CursorSlot = new(CursorSlot.Type, maxStackSize);
            return amount;
        }

        internal void SetCursorSlot(ItemType type, int amount)
        {
            if (amount == 0)
            {
                type = ItemType.None;
            }

            CursorSlot = new(type, amount);
            SetCursorItem?.Invoke(this, new(type, amount));
        }

        private void DefaultInvInitialize()
        {
            //AddItemToInventory(ItemType.Assembler_1, 50);
            AddItemToInventory(ItemType.Wood_chest, 20);
            AddItemToInventory(ItemType.Stone_furnace, 20);
            //AddItemToInventory(ItemType.Copper_ore, 405);
            //AddItemToInventory(ItemType.Iron_ore, 205);
            AddItemToInventory(ItemType.Electric_drill, 10);
            AddItemToInventory(ItemType.Inserter, 10);
            AddItemToInventory(ItemType.Iron_plate, 100);
            //AddItemToInventory(ItemType.Iron_gear, 50);
            //AddItemToInventory(ItemType.Copper_wire, 100);
            //AddItemToInventory(ItemType.Iron_chest, 60);
            //AddItemToInventory(ItemType.Rocket_silo, 1);
            //AddItemToInventory(ItemType.Simple_circuit, 6);
            //AddItemToInventory(ItemType.Rocket_fuel, 6);
            //AddItemToInventory(ItemType.Steel_plate, 6);
        }

        private void OnInventorySlotChange(object sender, Storage.SlotChangedEventArgs args)
        {
            SetInvItem?.Invoke(this, new(args.Pos, args.Type, args.Amount));
        }

        private void OnHotbarSlotChange(object sender, Storage.SlotChangedEventArgs args)
        {
            SetHotbarItem?.Invoke(this, new(args.Pos, args.Type, args.Amount));
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
