using UnityEngine;

namespace Dev.Kosov.Factory.Core.Entities
{
    internal class Assembler1 : Entity
    {
        public Assembler1(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { ItemType.Assembler1 }, new() { 1 }, EntityType.Assembler1)
        {

        }
    }
}
