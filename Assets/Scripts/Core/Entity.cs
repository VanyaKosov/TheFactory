using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    internal class Entity
    {
        private readonly List<InvSlot> deconstructionComponents;

        internal readonly Rotation rotation;
        internal readonly Vector2Int bottomLeftPos;
        internal readonly EntityType type;

        internal Entity(Rotation rotation, Vector2Int bottomLeftPos, List<InvSlot> deconstructionComponents, EntityType type)
        {
            this.rotation = rotation;
            this.bottomLeftPos = bottomLeftPos;
            this.type = type;
            this.deconstructionComponents = deconstructionComponents;
        }

        virtual internal List<InvSlot> GetComponents()
        {
            return deconstructionComponents;
        }
    }
}
