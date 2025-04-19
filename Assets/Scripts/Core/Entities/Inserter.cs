using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public class Inserter : Entity
    {
        private const float degreesPerSecond = 720f;
        private const float secondsPerCycle = 0.5f;
        private readonly float takePosDegrees;
        private float armDegrees; // 0 == take position, 180 == put position
        private float timeStarted;
        private float prevTime;
        private InvSlot takenItem;

        internal readonly Vector2Int TakePos;
        internal readonly Vector2Int PutPos;

        internal Inserter(Rotation rotation, Vector2Int bottomLeftPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Inserter, 1) }, EntityType.Inserter)
        {
            timeStarted = Time.time;
            prevTime = Time.time;

            switch (rotation)
            {
                case Rotation.Up:
                    PutPos = bottomLeftPos + new Vector2Int(0, 1);
                    TakePos = bottomLeftPos - new Vector2Int(0, 1);
                    takePosDegrees = 180f;
                    break;
                case Rotation.Right:
                    PutPos = bottomLeftPos + new Vector2Int(1, 0);
                    TakePos = bottomLeftPos - new Vector2Int(1, 0);
                    takePosDegrees = 90f;
                    break;
                case Rotation.Down:
                    PutPos = bottomLeftPos - new Vector2Int(0, 1);
                    TakePos = bottomLeftPos + new Vector2Int(0, 1);
                    takePosDegrees = 0f;
                    break;
                case Rotation.Left:
                    PutPos = bottomLeftPos - new Vector2Int(1, 0);
                    TakePos = bottomLeftPos + new Vector2Int(1, 0);
                    takePosDegrees = 270f;
                    break;
                default:
                    throw new Exception("Unknown rotation");
            }
        }

        override internal void UpdateState()
        {
            base.UpdateState();
            float currTime = Time.time;
            float timeDiff = currTime - prevTime;

            if (prevTime <= secondsPerCycle / 2 && currTime > secondsPerCycle / 2)
            {
                DropItem();
            }

            if (currTime - timeStarted >= secondsPerCycle) // Maybe take in the beginning instead of the end?
            {
                TakeItem();
                timeStarted = currTime;
            }

            if (currTime - timeStarted < secondsPerCycle / 2)
            {
                armDegrees += timeDiff * degreesPerSecond;
            }
            else
            {
                armDegrees -= timeDiff * degreesPerSecond;
            }

            prevTime = currTime;
        }

        public float GetTotalDegrees()
        {
            return takePosDegrees + armDegrees;
        }

        private void DropItem()
        {

        }

        private void TakeItem()
        {

        }
    }
}
