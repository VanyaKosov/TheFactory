using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class Entity
    {
        internal readonly Rotation rotation;
        internal readonly Vector2Int topLeftPos;
        internal readonly Vector2Int size;
        internal readonly List<ItemType> items;
        internal readonly List<int> itemCounts;

        internal Entity(Rotation rotation, Vector2Int topLeftPos, Vector2Int size, List<ItemType> items, List<int> itemCounts)
        {
            this.rotation = rotation;
            this.topLeftPos = topLeftPos;
            this.size = size;
            this.items = items;
            this.itemCounts = itemCounts;
        }
    }
}
