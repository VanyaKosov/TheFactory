using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Core
{
    abstract class Building
    {
        public Rotation Rotation { get; }
        public Vector2Int TopLeftPos { get; }
        public Vector2Int Size { get; }

        public Building(Rotation rotation, Vector2Int topLeftPos, Vector2Int size)
        {
            Rotation = rotation;
            TopLeftPos = topLeftPos;
            Size = size;
        }
    }
}
