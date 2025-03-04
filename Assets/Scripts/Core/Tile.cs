using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Core
{
    class Tile
    {
        public Back BackType { get; set; }
        public int FeatureID { get; set; }
        public Ore OreType { get; }
        public int OreAmount { get; }

        public Tile(Back backType, Ore oreType, int oreAmount)
        {
            BackType = backType;
            OreType = oreType;
            OreAmount = oreAmount;
        }
    }
}
