using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Kosov.Factory.Core
{
    public class Inserter : Entity
    {
        private const int capacity = 1;
        private const float degreesPerSecond = 180f;
        private readonly Func<Vector2Int, Entity> getEntityAtPos;
        private readonly float takePosDegrees;
        private Action<float> state;
        private float armDegrees; // 0 == take position, 180 == put position
        private float timeStarted;
        private float prevTime;
        private InvSlot item;
        public InvSlot Item { get => item; }

        internal readonly Vector2Int TakePos;
        internal readonly Vector2Int PutPos;

        internal Inserter(Rotation rotation, Vector2Int bottomLeftPos, Func<Vector2Int, Entity> getEntityAtPos)
            : base(rotation, bottomLeftPos, new() { new(ItemType.Inserter, 1) }, EntityType.Inserter)
        {
            this.getEntityAtPos = getEntityAtPos;
            state = StateTake;
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
            float deltaTime = currTime - prevTime;

            state(deltaTime);

            prevTime = currTime;
        }

        override internal List<InvSlot> GetComponents()
        {
            List<InvSlot> items = base.GetComponents();
            items.Add(item);

            return items;
        }

        private void StateTake(float deltaTime)
        {
            Entity takeEntity = getEntityAtPos(TakePos);
            if (takeEntity == null) return;
            if (takeEntity is not ITakeable source) return;

            Entity putEntity = getEntityAtPos(PutPos);
            if (putEntity == null) return;
            if (putEntity is not IPuttable target) return;

            List<InvSlot> wantedItems = target.GetWantedItems();
            InvSlot taken = new(ItemType.None, 0);
            foreach (InvSlot item in wantedItems)
            {
                taken = source.Take(item.Type);
                if (taken.Type != ItemType.None) break;
            }

            if (taken.Type == ItemType.None) return;
            item = taken;

            state = StateRotateToTarget;
        }

        private void StateRotateToTarget(float deltaTime)
        {
            armDegrees += degreesPerSecond * deltaTime;
            if (armDegrees < 180) return;
            armDegrees = 180;
            state = StatePut;
        }

        private void StatePut(float deltaTime)
        {
            Entity entity = getEntityAtPos(PutPos);
            if (entity == null) return;
            if (entity is not IPuttable target) return;

            int remainder = target.Put(item);
            item.Amount = remainder;
            if (remainder != 0) return;
            item.Type = ItemType.None;

            state = StateRotateToSource;
        }

        private void StateRotateToSource(float deltaTime)
        {
            armDegrees -= degreesPerSecond * deltaTime;
            if (armDegrees > 0) return;
            armDegrees = 0;
            state = StateTake;
        }
    }
}
