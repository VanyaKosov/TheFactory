using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public class Inserter : Entity
    {
        private const float degreesPerSecond = 720f;
        private const float secondsPerCycle = 360f / degreesPerSecond;
        private readonly Func<Vector2Int, Entity> getEntityAtPos;
        private readonly float takePosDegrees;
        private float armDegrees; // 0 == take position, 180 == put position
        private float timeStarted;
        private float prevTime;
        private ItemType item;

        internal readonly Vector2Int TakePos;
        internal readonly Vector2Int PutPos;

        internal Inserter(Rotation rotation, Vector2Int bottomLeftPos, Func<Vector2Int, Entity> getEntityAtPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Inserter, 1) }, EntityType.Inserter)
        {
            this.getEntityAtPos = getEntityAtPos;
            timeStarted = Time.time;
            prevTime = timeStarted;

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

        public float GetTotalDegrees()
        {
            return takePosDegrees + armDegrees;
        }

        override internal void UpdateState()
        {
            base.UpdateState();
            float currTime = Time.time;
            float timeDiff = currTime - prevTime;

            if (prevTime - timeStarted <= secondsPerCycle / 2 && currTime - timeStarted > secondsPerCycle / 2)
            {
                DropItem();
                armDegrees = 180f;
            }

            if (currTime - timeStarted >= secondsPerCycle) // Maybe take in the beginning instead of the end?
            {
                TakeItem();
                timeStarted = currTime;
                armDegrees = 0;
                return;
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

        override internal List<InvSlot> GetComponents()
        {
            List<InvSlot> items = base.GetComponents();
            if (item != ItemType.None)
            {
                items.Add(new(item, 1));
            }

            return items;
        }

        private void DropItem()
        {
            Entity entity = getEntityAtPos.Invoke(PutPos);
            if (entity == null) return;
        }

        private void TakeItem()
        {
            Entity entity = getEntityAtPos.Invoke(TakePos);
            if (entity == null) return;
        }
    }
}
