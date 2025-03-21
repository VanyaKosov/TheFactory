using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class PlayerController : MonoBehaviour
    {
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