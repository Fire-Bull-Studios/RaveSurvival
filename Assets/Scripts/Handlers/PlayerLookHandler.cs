using System;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

namespace RaveSurvival
{
    public class PlayerLookHandler : NetworkBehaviour
    {
        public float overallSensitivity = 100f;
        public Transform playerBody;
        float xRotation = 0f;
        private bool canLook = true;

        private InputSystem_Actions inputActions;
        private Vector2 lookInput;   // x = yaw, y = pitch

        [SerializeField]
        private float mouseSensitivityMultiplier = 0.25f;
        [SerializeField]
        private float gamepadSensitiviyMultiplier = 1.25f;

        void Awake()
        {
            inputActions = new InputSystem_Actions();
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
            // Enable only the Player action map
            inputActions.Player.Enable();

            // Subscribe to Look action
            inputActions.Player.Look.performed += OnLook;
            inputActions.Player.Look.canceled += OnLook; // gives (0,0) when released
        }

        private void DisableInput()
        {
            if (inputActions == null) return;

            inputActions.Player.Look.performed -= OnLook;
            inputActions.Player.Look.canceled -= OnLook;
            inputActions.Player.Disable();
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            lookInput = ctx.ReadValue<Vector2>();

            // Detect the device that triggered the input
            if (ctx.control.device is Mouse)
            {
                // Mouse delta is huge — scale it DOWN
                lookInput *= mouseSensitivityMultiplier;
            }
            else if (ctx.control.device is Gamepad)
            {
                // Gamepad values are small — maybe leave default
                lookInput *= gamepadSensitiviyMultiplier;
            }
        }

        /// <summary>
        /// Locks the cursor for the local player.
        /// </summary>
        void Start()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("Error... tried to initialize player look handler without game manager instance");
                return;
            }

            // Realistically this should only be done on the local player, but this matches your old logic.
            if (isLocalPlayer && GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            Cursor.lockState = CursorLockMode.Locked;
        }

        void Look()
        {
            // Convert input into rotation deltas
            float mouseX = lookInput.x * overallSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * overallSensitivity * Time.deltaTime;

            // Adjust the vertical rotation based on Y input (pitch)
            xRotation -= mouseY;
            xRotation = Math.Clamp(xRotation, -90f, 90f);

            // Apply vertical rotation to the camera
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Rotate the player's body horizontally based on X input (yaw)
            playerBody.Rotate(Vector3.up * mouseX);
        }

        /// <summary>
        /// Unity's Update method, called once per frame.
        /// Handles the player's look mechanics for the local player.
        /// </summary>
        void Update()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("Error... tried to update player look handler without game manager instance");
                return;
            }

            if (!canLook)
                return;

            if (GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
            {
                if (isLocalPlayer)
                    Look();
            }
            else
            {
                Look();
            }
        }

        public void SetCanLook(bool x)
        {
            canLook = x;
        }
    }
}
