using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class RocketSilo : Entity
    {
        internal RocketSilo(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Rocket_silo, 1) }, EntityType.Rocket_silo)
        {

        }
    }
}
