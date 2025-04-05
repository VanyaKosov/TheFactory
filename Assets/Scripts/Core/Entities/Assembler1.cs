using UnityEngine;

namespace Dev.Kosov.Factory.Core.Entities
{
    internal class Assembler1 : Entity
    {
        //private readonly Crafter crafter = new();

        internal Assembler1(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Assembler_1, 1) }, EntityType.Assembler1)
        {

        }
    }
}
