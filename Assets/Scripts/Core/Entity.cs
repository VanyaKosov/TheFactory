using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public class Entity
    {
        private readonly List<InvSlot> deconstructionComponents;

        public readonly Rotation Rotation;
        public readonly Vector2Int BottomLeftPos;
        public readonly EntityType Type;
        public Vector2Int Size
        {
            get => EntityInfo.Get(Type).Size;
        }

        internal Entity(Rotation rotation, Vector2Int bottomLeftPos, List<InvSlot> deconstructionComponents, EntityType type)
        {
            Rotation = rotation;
            BottomLeftPos = bottomLeftPos;
            Type = type;
            this.deconstructionComponents = deconstructionComponents;
        }

        virtual internal void UpdateState()
        {
            // Nothing to be here
        }

        virtual internal List<InvSlot> GetComponents()
        {
            return deconstructionComponents;
        }
    }
}
