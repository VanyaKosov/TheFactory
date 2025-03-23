using UnityEngine;

namespace Dev.Kosov.Factory.Core.Entities
{
    internal class Assembler1 : Entity
    {
        public Assembler1(Rotation rotation, Vector2Int topLeftPos)
            : base(rotation, topLeftPos, new() { ItemType.Assembler1 }, new() { 1 })
        {

        }
    }
}
