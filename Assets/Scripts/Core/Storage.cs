using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class Storage
    {
        private readonly InvSlot[,] items;

        internal readonly int Width;
        internal readonly int Height;

        internal event EventHandler<SlotCahangedEventArgs> SlotChanged;

        internal Storage(int width, int height)
        {
            Width = width;
            Height = height;
            items = new InvSlot[Width, Height];

            DefaultInitialize();
        }

        internal InvSlot GetItem(Vector2Int pos)
        {
            return new(items[pos.x, pos.y]);
        }

        internal void SetItem(InvSlot slot, Vector2Int pos)
        {
            items[pos.x, pos.y] = new(slot.Type, slot.Amount);
            SlotChanged?.Invoke(this, new(pos, slot.Type, slot.Amount));
        }

        internal InvSlot AutoTake() // Returns taken type and count
        {
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

        internal int AutoPut(ItemType type, int amount) // Returns remainder
        {
            int stackSize = ItemInfo.Get(type).MaxStackSize;

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (items[x, y].Type != type) continue;
                    int canFit = stackSize - items[x, y].Amount;
                    canFit = Math.Min(canFit, amount);

                    items[x, y].Amount += canFit;
                    amount -= canFit;

                    SlotChanged?.Invoke(this, new(new(x, y), items[x, y].Type, items[x, y].Amount));

                    if (amount == 0) return 0;
                }
            }

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (items[x, y].Type != ItemType.None) continue;
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

        internal class SlotCahangedEventArgs : EventArgs
        {
            internal readonly Vector2Int Pos;
            internal readonly ItemType Type;
            internal readonly int Amount;

            public SlotCahangedEventArgs(Vector2Int pos, ItemType type, int amount)
            {
                Pos = pos;
                Type = type;
                Amount = amount;
            }
        }
    }
}
