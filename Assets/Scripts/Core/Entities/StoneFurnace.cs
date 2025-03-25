using UnityEngine;

namespace Dev.Kosov.Factory.Core.Assets.Scripts.Core.Entities
{
    internal class StoneFurnace : Entity
    {
        public StoneFurnace(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { ItemType.StoneFurnace }, new() { 1 })
        {

        }
    }
}
