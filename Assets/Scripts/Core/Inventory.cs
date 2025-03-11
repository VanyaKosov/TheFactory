using System;
using UnityEngine;

namespace Assets.Scripts.Core
{
    class Inventory
    {
        private const int invWidth = 10;
        private const int invHeight = 16;
        private readonly Slot[,] inventory = new Slot[invWidth, invHeight];

        private class Slot
        {
            public ItemType Type { get; set; }
            public int Amount { get; set; }

            public Slot(ItemType type, int amount)
            {
                Type = type;
                Amount = amount;
            }
        }
    }
}
