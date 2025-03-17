using Dev.Kosov.Factory.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class ItemSpriteCatalog : MonoBehaviour
    {
        public Sprite Empty = null;
        public Sprite Wood;
        public Sprite Coal;
        public Sprite Iron_Ore;
        public Sprite Copper_Ore;
        public Sprite Assembler1;

        private Dictionary<ItemType, Sprite> typeToSprite = new();

        void Awake()
        {
            typeToSprite.Add(ItemType.Empty, Empty);
            typeToSprite.Add(ItemType.Wood, Wood);
            typeToSprite.Add(ItemType.Coal, Coal);
            typeToSprite.Add(ItemType.Iron_ore, Iron_Ore);
            typeToSprite.Add(ItemType.Copper_ore, Copper_Ore);
            typeToSprite.Add(ItemType.Assembler1, Assembler1);
        }

        public Sprite GetSprite(ItemType type)
        {
            return typeToSprite[type];
        }
    }
}
