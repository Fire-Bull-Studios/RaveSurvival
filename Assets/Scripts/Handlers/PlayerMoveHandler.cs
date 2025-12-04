using System;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

namespace RaveSurvival
{
    public class PlayerMoveHandler : NetworkBehaviour
    {
        public CharacterController controller;

        public float speed = 12f;

        public float gravity = -9.81f;

        public float jumpHeight = 3f;

        public Transform groundCheck;
        public float groundDistance = 0.4f;

        public LayerMask groundMask;

        private Vector3 velocity;

        private bool isGrounded;
        private bool canMove = true;

        private InputSystem_Actions inputActions;
        private Vector2 moveInput;
        private bool jumpQueued;

        void Awake()
        {
            if (controller == null)
                controller = GetComponent<CharacterController>();

            inputActions = new InputSystem_Actions();
        }

        // Mirror: called only for the local player instance
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            EnableInput();
        }

        void OnEnable()
        {
            if (inputActions == null)
                inputActions = new InputSystem_Actions();

            EnableInput();
        }


        void OnDisable()
        {
            DisableInput();
        }

        private void EnableInput()
        {
            if (inputActions == null)
                inputActions = new InputSystem_Actions();

            // Enable only the Player action map
            inputActions.Player.Enable();

            // Subscribe to actions
            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += OnMove; // will give (0,0) when released

            inputActions.Player.Jump.performed += OnJump;
        }

        private void DisableInput()
        {
            if (inputActions == null) return;

            inputActions.Player.Move.performed -= OnMove;
            inputActions.Player.Move.canceled -= OnMove;
            inputActions.Player.Jump.performed -= OnJump;

            inputActions.Player.Disable();
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            // we just queue the jump, handled in PlayerMove so timing stays consistent
            if (ctx.performed)
                jumpQueued = true;
        }

        /// <summary>
        /// Unity's Update method, called once per frame.
        /// Handles player movement and jumping for the local player.
        /// </summary>
        void Update()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("Error... GameManager is null when trying to move player...");
                return;
            }

            if (!canMove)
                return;

            if (GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
            {
                // Ensure the movement logic only applies to the local player
                if (isLocalPlayer)
                {
                    PlayerMove();
                }
            }
            else
            {
                PlayerMove();
            }
        }

        void PlayerMove()
        {
            // Check if the player is grounded using a sphere at the groundCheck position
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            // Reset vertical velocity if the player is grounded
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Small negative value to keep the player grounded
            }

            float x = moveInput.x; // left / right
            float z = moveInput.y; // forward / back

            // Calculate the movement direction based on input and player orientation
            Vector3 move = transform.right * x + transform.forward * z;

            // Move the player using the CharacterController
            controller.Move(move * speed * Time.deltaTime);

            // Check if jump was pressed and the player is grounded
            if (jumpQueued && isGrounded)
            {
                // Calculate the vertical velocity needed to achieve the desired jump height
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            jumpQueued = false; // consume the jump input

            // Apply gravity to the vertical velocity
            velocity.y += gravity * Time.deltaTime;

            // Apply the vertical velocity to the player
            controller.Move(velocity * Time.deltaTime);
        }

        public void SetCanMove(bool x)
        {
            canMove = x;
        }
    }
}
