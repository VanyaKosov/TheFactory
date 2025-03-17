using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core.Entities
{
    internal class WoodChest : Entity
    {
        internal const int Width = 10;
        internal const int Height = 2;
        internal readonly InvSlot[,] inventory;

        public WoodChest(Rotation rotation, Vector2Int topLeftPos)
            : base(rotation, topLeftPos, new(1, 1), new() { ItemType.WoodChest}, new() { 1 })
        {
            inventory = new InvSlot[Width, Height];
        }
    }
}
