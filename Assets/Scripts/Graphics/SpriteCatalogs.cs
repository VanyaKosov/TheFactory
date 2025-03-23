using Dev.Kosov.Factory.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class SpriteCatalogs : MonoBehaviour
    {
        [Header("Item Icons Sprites")]
        public Sprite Wood;
        public Sprite Coal;
        public Sprite Iron_Ore;
        public Sprite Copper_Ore;
        public Sprite Assembler1;
        public Sprite WoodChest;

        [Header("Entity Sprites")]
        public Sprite Assembler1Entity;
        public Sprite WoodChestEntity;

        private readonly Dictionary<ItemType, Sprite> itemTypeToSprite = new();
        private readonly Dictionary<EntityType, Sprite> entityTypeToSprite = new();

        void Awake()
        {
            // Icons
            itemTypeToSprite.Add(ItemType.Wood, Wood);
            itemTypeToSprite.Add(ItemType.Coal, Coal);
            itemTypeToSprite.Add(ItemType.Iron_ore, Iron_Ore);
            itemTypeToSprite.Add(ItemType.Copper_ore, Copper_Ore);
            itemTypeToSprite.Add(ItemType.Assembler1, Assembler1);
            itemTypeToSprite.Add(ItemType.WoodChest, WoodChest);

            // Entities
            entityTypeToSprite.Add(EntityType.Assembler1, Assembler1Entity);
            entityTypeToSprite.Add(EntityType.WoodChest, WoodChestEntity);
        }

        public Sprite GetIconSprite(ItemType type)
        {
            return itemTypeToSprite[type];
        }

        public Sprite GetEntitySprite(EntityType type)
        {
            return entityTypeToSprite[type];
        }
    }
}
