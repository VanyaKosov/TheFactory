using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public class Storage
    {
        private readonly bool canTake;
        private readonly InvSlot[,] items;
        private readonly ItemType[,] reserved;

        public readonly int Width;
        public readonly int Height;

        internal event EventHandler<SlotChangedEventArgs> SlotChanged;

        internal Storage(int width, int height, bool canTake)
        {
            Width = width;
            Height = height;
            items = new InvSlot[Width, Height];
            reserved = new ItemType[Width, Height]; // Hope the default is ItemType.None
            this.canTake = canTake;

            DefaultInitialize();
        }

        public InvSlot GetItem(Vector2Int pos)
        {
            return items[pos.x, pos.y];
        }

        internal void SetReserve(ItemType type, Vector2Int pos)
        {
            reserved[pos.x, pos.y] = type;
        }

        internal void SetItem(InvSlot slot, Vector2Int pos)
        {
            if (slot.Amount == 0)
            {
                slot = new(ItemType.None, 0);
            }

            items[pos.x, pos.y] = slot;
            SlotChanged?.Invoke(this, new(pos, slot.Type, slot.Amount));
        }

        internal int TryStack(InvSlot inputSlot, Vector2Int pos) // Returns remainder
        {
            if (items[pos.x, pos.y].Type == ItemType.None && inputSlot.Amount > 0)
            {
                items[pos.x, pos.y] = new(inputSlot.Type, 0);
            }

            if (items[pos.x, pos.y].Type != inputSlot.Type) return inputSlot.Amount;

            int stackSize = ItemInfo.Get(inputSlot.Type).MaxStackSize;
            int canFit = stackSize - items[pos.x, pos.y].Amount;
            if (inputSlot.Amount <= canFit)
            {
                items[pos.x, pos.y].Amount += inputSlot.Amount;
                SlotChanged?.Invoke(this, new(pos, items[pos.x, pos.y].Type, items[pos.x, pos.y].Amount));
                return 0;
            }

            items[pos.x, pos.y].Amount += canFit;
            SlotChanged?.Invoke(this, new(pos, items[pos.x, pos.y].Type, items[pos.x, pos.y].Amount));
            return inputSlot.Amount - canFit;
        }

        // Takes 1 at a time
        internal InvSlot AutoTake() // Returns taken type and count
        {
            if (!canTake) return new(ItemType.None, 0);

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (items[x, y].Type == ItemType.None) continue;

                    ItemType type = items[x, y].Type;
                    items[x, y].Amount--;
                    if (items[x, y].Amount == 0) items[x, y].Type = ItemType.None;

                    SlotChanged?.Invoke(this, new(new(x, y), items[x, y].Type, items[x, y].Amount));
                    return new(type, 1);
                }
            }

            return new(ItemType.None, 0);
        }

        internal InvSlot AutoTake(ItemType request)
        {
            if (!canTake) return new(ItemType.None, 0);

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (items[x, y].Type != request) continue;

                    items[x, y].Amount--;
                    if (items[x, y].Amount == 0) items[x, y].Type = ItemType.None;

                    SlotChanged?.Invoke(this, new(new(x, y), items[x, y].Type, items[x, y].Amount));
                    return new(request, 1);
                }
            }

            return new(ItemType.None, 0);
        }

        internal int AutoPut(ItemType type, int amount) // Returns remainder
        {
            int stackSize = ItemInfo.Get(type).MaxStackSize;

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (items[x, y].Type == ItemType.None) continue;
                    if (reserved[x, y] != ItemType.None && reserved[x, y] != type) continue;
                    amount = TryStack(new(type, amount), new(x, y));

                    if (amount == 0) return 0;
                }
            }

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (items[x, y].Type != ItemType.None) continue;
                    if (reserved[x, y] != ItemType.None && reserved[x, y] != type) continue;
                    items[x, y].Type = type;
                    items[x, y].Amount = Math.Min(stackSize, amount);

                    SlotChanged?.Invoke(this, new(new(x, y), items[x, y].Type, items[x, y].Amount));

                    amount -= items[x, y].Amount;
                    if (amount <= 0)
                    {
                        return 0;
                    }

                }
            }

            return amount;
        }

        private void DefaultInitialize()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    items[x, y] = new(ItemType.None, 0);
                }
            }
        }

        internal class SlotChangedEventArgs : EventArgs
        {
            internal readonly Vector2Int Pos;
            internal readonly ItemType Type;
            internal readonly int Amount;

            public SlotChangedEventArgs(Vector2Int pos, ItemType type, int amount)
            {
                Pos = pos;
                Type = type;
                Amount = amount;
            }
        }
    }
}
