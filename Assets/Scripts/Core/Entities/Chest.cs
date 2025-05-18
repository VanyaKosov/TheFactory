using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public class Chest : Entity, IPuttable, ITakeable
    {
        public readonly Storage Storage;
        public readonly int InvWidth;
        public readonly int InvHeight;

        internal Chest(Rotation rotation, Vector2Int bottomLeftPos, int width, int height, ItemType type)
            : base(rotation, bottomLeftPos, new() { new(type, 1) }, ItemInfo.Get(type).EntityType)
        {
            InvWidth = width;
            InvHeight = height;
            Storage = new(width, height, true);
        }

        internal override List<InvSlot> GetComponents()
        {
            List<InvSlot> items = base.GetComponents();
            for (int x = 0; x < Storage.Width; x++)
            {
                for (int y = 0; y < Storage.Height; y++)
                {
                    if (Storage.GetItem(new(x, y)).Type == ItemType.None) continue;
                    items.Add(Storage.GetItem(new(x, y)));
                }
            }

            return items;
        }

        int IPuttable.Put(InvSlot item)
        {
            return Storage.AutoPut(item.Type, item.Amount);
        }

        InvSlot ITakeable.Take()
        {
            return Storage.AutoTake();
        }

        InvSlot ITakeable.Take(ItemType type)
        {
            return Storage.AutoTake(type);
        }

        List<InvSlot> IPuttable.GetWantedItems()
        {
            return null;
        }
    }
}
