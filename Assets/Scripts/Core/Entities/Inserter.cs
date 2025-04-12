using UnityEngine;

namespace Dev.Kosov.Factory.Core.Entities
{
    internal class Inserter : Entity
    {
        internal Inserter(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Inserter, 1) }, EntityType.Inserter)
        {

        }
    }
}
