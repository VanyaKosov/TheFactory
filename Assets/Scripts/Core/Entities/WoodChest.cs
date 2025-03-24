using UnityEngine;

namespace Dev.Kosov.Factory.Core.Entities
{
    internal class WoodChest : Entity
    {
        internal const int InvWidth = 10;
        internal const int InvHeight = 2;
        internal readonly InvSlot[,] inventory;

        public WoodChest(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { ItemType.WoodChest }, new() { 1 })
        {
            inventory = new InvSlot[InvWidth, InvHeight];
        }
    }
}
