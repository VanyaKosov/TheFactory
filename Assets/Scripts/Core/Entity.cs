using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class Entity
    {
        internal readonly Rotation rotation;
        internal readonly Vector2Int bottomLeftPos;
        internal readonly List<ItemType> items;
        internal readonly List<int> itemCounts;
        internal readonly EntityType type;

        internal Entity(Rotation rotation, Vector2Int bottomLeftPos, List<ItemType> items, List<int> itemCounts, EntityType type)
        {
            this.rotation = rotation;
            this.bottomLeftPos = bottomLeftPos;
            this.items = items;
            this.itemCounts = itemCounts;
            this.type = type;
        }
    }
}
