using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    class Entity
    {
        public readonly Rotation rotation;
        public readonly Vector2Int topLeftPos;
        public readonly Vector2Int size;
        public readonly List<ItemType> items;
        public readonly List<int> itemCounts;

        public Entity(Rotation rotation, Vector2Int topLeftPos, Vector2Int size, List<ItemType> items, List<int> itemCounts)
        {
            this.rotation = rotation;
            this.topLeftPos = topLeftPos;
            this.size = size;
            this.items = items;
            this.itemCounts = itemCounts;
        }
    }
}
