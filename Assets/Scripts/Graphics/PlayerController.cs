using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class PlayerController : MonoBehaviour
    {
        private Vector2 prevMoveVector = new(0, 1);

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

            UpdateAnimations(moveVector);
        }

        private void UpdateAnimations(Vector2 moveVector)
        {
            if (moveVector.magnitude == 0) // Might have problems with precision
            {
                MovementAnimator.SetFloat("xOffset", prevMoveVector.x);
                MovementAnimator.SetFloat("yOffset", prevMoveVector.y);

                ShadowAnimator.SetFloat("xOffset", prevMoveVector.x);
                ShadowAnimator.SetFloat("yOffset", prevMoveVector.y);

                return;
            }
            prevMoveVector = moveVector;

            MovementAnimator.SetFloat("xOffset", prevMoveVector.x * 2);
            MovementAnimator.SetFloat("yOffset", prevMoveVector.y * 2);

            ShadowAnimator.SetFloat("xOffset", prevMoveVector.x * 2);
            ShadowAnimator.SetFloat("yOffset", prevMoveVector.y * 2);
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