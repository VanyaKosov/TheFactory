using Dev.Kosov.Factory.Core;
using UnityEngine.U2D;
using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class PlayerController : MonoBehaviour
    {
        private const float idleAnim = 1f;
        private const float runningAnim = 2f;
        private const float miningAnim = 3f;
        private Vector2 prevMoveVector = new(0, 1);
        private World world;
        private Inventory inventory;

        public UserInput UserInput;
        public Camera Camera;
        public EntityPlacer EntityPlacer;
        public WorldController WorldController;
        public Animator MovementAnimator;
        public Animator ShadowAnimator;
        public PixelPerfectCamera PixelPerfectCamera;
        public Rigidbody2D RigidBody;
        public float MoveSpeed = 10;
        public int MinZoom = 5;
        public int MaxZoom = 40;
        public float ZoomSpeed = 5;

        void OnEnable()
        {
            world = WorldController.World;
            inventory = world.Inventory;
            UserInput.PrimaryInput += OnPrimaryInput;
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

            ActionType action;
            if (EntityPlacer.ActionType == ActionType.None)
            {
                RigidBody.MovePosition(RigidBody.position + Time.deltaTime * MoveSpeed * moveVector);
                action = ChooseAnimation(moveVector);
                if (action == ActionType.Running) prevMoveVector = moveVector;
            }
            else
            {
                action = EntityPlacer.ActionType;
            }

            UpdateAnimations(action);
        }

        private ActionType ChooseAnimation(Vector2 moveVector)
        {
            if (Mathf.Approximately(moveVector.magnitude, 0.0f)) return ActionType.Idle;
            if (Mathf.Approximately(moveVector.magnitude, 1.0f)) return ActionType.Running;
            throw new Exception("Unnormalized vector???");
        }

        private void UpdateAnimations(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.RemoveBuilding:
                    SetAnimations(prevMoveVector * idleAnim);
                    return;
                case ActionType.ChopTree:
                    SetAnimations(prevMoveVector * miningAnim);
                    return;
                case ActionType.MineOre:
                    SetAnimations(prevMoveVector * miningAnim);
                    return;
                case ActionType.Idle:
                    SetAnimations(prevMoveVector * idleAnim);
                    return;
                case ActionType.Running:
                    SetAnimations(prevMoveVector * runningAnim);
                    return;
                default:
                    throw new Exception("No action");
            }
        }

        private void SetAnimations(Vector2 animPos)
        {
            MovementAnimator.SetFloat("xOffset", animPos.x);
            MovementAnimator.SetFloat("yOffset", animPos.y);

            ShadowAnimator.SetFloat("xOffset", animPos.x);
            ShadowAnimator.SetFloat("yOffset", animPos.y);
        }

        private void Zoom()
        {
            PixelPerfectCamera.assetsPPU =
                Math.Min(
                    Math.Max(Mathf.RoundToInt(PixelPerfectCamera.assetsPPU + Input.mouseScrollDelta.y * ZoomSpeed), MinZoom),
                MaxZoom);
        }

        private void OnPrimaryInput(object sender, EventArgs args)
        {
            Vector2 mousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
            world.OpenEntity(WorldController.WorldToMapPos(mousePos));
        }
    }
}