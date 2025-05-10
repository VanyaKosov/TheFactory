using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class IronChest : Entity, IPuttable, ITakeable
    {
        private readonly Storage storage = new(InvWidth, InvHeight);

        internal const int InvWidth = 6;
        internal const int InvHeight = 4;

        internal IronChest(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Iron_chest, 1) }, EntityType.Iron_chest)
        {

        }

        int IPuttable.Put(InvSlot item)
        {
            return storage.AutoPut(item.Type, item.Amount);
        }

        InvSlot ITakeable.Take()
        {
            return storage.AutoTake();
        }

        InvSlot ITakeable.Take(ItemType type)
        {
            return storage.AutoTake(type);
        }

        List<InvSlot> IPuttable.GetWantedItems()
        {
            return null;
        }
    }
}
