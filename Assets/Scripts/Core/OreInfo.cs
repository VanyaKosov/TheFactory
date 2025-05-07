using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core.Assets.Scripts.Core
{
    internal static class OreInfo
    {
        private static readonly Dictionary<OreType, Info> info = new()
        {
            { OreType.Coal, new(new(8f, 12f), 10_000, 0.000_01f, ItemType.Coal) },
            { OreType.Stone, new(new(8f, 12f), 10_000, 0.000_01f, ItemType.Stone) },
            { OreType.Copper, new(new(8f, 12f), 10_000, 0.000_01f, ItemType.Copper_ore) },
            { OreType.Iron, new(new(8f, 12f), 10_000, 0.000_01f, ItemType.Iron_ore) }
        };

        internal static Info Get(OreType type)
        {
            return info[type];
        }

        internal static IEnumerable<(OreType Type, Info Info)> GetAll()
        {
            foreach (var item in info) yield return (item.Key, item.Value);
        }

        internal readonly struct Info
        {
            internal readonly Vector2 RadiusVariation;
            internal readonly int MaxRichness;
            internal readonly float SpawnChance;
            internal readonly ItemType ItemType;

            internal Info(Vector2 radiusVariation, int maxRichness, float spawnChance, ItemType itemType)
            {
                RadiusVariation = radiusVariation;
                MaxRichness = maxRichness;
                SpawnChance = spawnChance;
                ItemType = itemType;
            }
        }
    }
}
