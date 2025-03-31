using Dev.Kosov.Factory.Core.Assets.Scripts.Core;
using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class PlayerController : MonoBehaviour
    {
        private Vector2 prevMoveVector = new(0, 1);

        public WorldController WorldController;
        public Animator MovementAnimator;
        public Animator ShadowAnimator;
        public Camera Camera;
        public Rigidbody2D RigidBody;
        public float MoveSpeed;
        public float MinZoom;
        public float MaxZoom;
        public float ZoomSpeed;

        void Start()
        {

        }

        void Update()
        {

        }

        void FixedUpdate()
        {
            Move();
            Zoom();
        }

        private void Move()
        {
            float xVel = Input.GetAxis("Horizontal");
            float yVel = Input.GetAxis("Vertical");

            Vector2 moveVector = new Vector2(xVel, yVel).normalized;
            RigidBody.MovePosition(RigidBody.position + Time.deltaTime * MoveSpeed * moveVector);

            ActionType movementAction = ChooseAnimation(moveVector);
            if (movementAction == ActionType.Running) prevMoveVector = moveVector;
            UpdateAnimations(movementAction);
        }

        private ActionType ChooseAnimation(Vector2 moveVector)
        {
            // Might have problems with precision?
            if (Mathf.Approximately(moveVector.magnitude, 0.0f)) return ActionType.Idle;
            if (Mathf.Approximately(moveVector.magnitude, 1.0f)) return ActionType.Running;
            throw new Exception("Unnormalized vector???");
        }

        private void UpdateAnimations(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.RemoveBuilding:
                    return;
                case ActionType.ChopTree:
                    return;
                case ActionType.MineOre:
                    return;
                case ActionType.Idle:
                    MovementAnimator.SetFloat("xOffset", prevMoveVector.x);
                    MovementAnimator.SetFloat("yOffset", prevMoveVector.y);

                    ShadowAnimator.SetFloat("xOffset", prevMoveVector.x);
                    ShadowAnimator.SetFloat("yOffset", prevMoveVector.y);
                    return;
                case ActionType.Running:
                    MovementAnimator.SetFloat("xOffset", prevMoveVector.x * 2);
                    MovementAnimator.SetFloat("yOffset", prevMoveVector.y * 2);

                    ShadowAnimator.SetFloat("xOffset", prevMoveVector.x * 2);
                    ShadowAnimator.SetFloat("yOffset", prevMoveVector.y * 2);
                    return;
                default:
                    throw new Exception("No action");
            }
        }

        private void Zoom()
        {
            Camera.orthographicSize =
                Math.Min(
                    Math.Max(
                        Camera.orthographicSize + Input.mouseScrollDelta.y * ZoomSpeed * -1,
                    MinZoom),
                MaxZoom);
        }
    }
}