﻿using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class WoodChest : Entity
    {
        private readonly Storage storage = new(InvWidth, InvHeight);

        internal const int InvWidth = 6;
        internal const int InvHeight = 2;

        internal WoodChest(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Wood_chest, 1) }, EntityType.WoodChest)
        {

        }
    }
}
