using System;
using UnityEngine;
using Mirror;
using Mirror.BouncyCastle.Bcpg.Sig;

namespace RaveSurvival
{
  public class PlayerLookHandler : NetworkBehaviour
  {
    // Sensitivity of the mouse for looking around
    public float mouseSensitivity = 100f;

    // Reference to the player's body transform for rotating the player
    public Transform playerBody;

    // Tracks the vertical rotation of the camera
    float xRotation = 0f;
    private bool canLook = true;

    /// <summary>
    /// Unity's Start method, called before the first frame update.
    /// Locks the cursor to the center of the screen for the local player.
    /// </summary>
    void Start()
    {
      if (GameManager.Instance == null)
      {
        Debug.LogError("Error... tried to initalize player look handler without game manager instance");
        return;
      }

      if (isLocalPlayer && GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
      {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
      }
      Cursor.lockState = CursorLockMode.Locked;
    }

    void Look()
    {
      // Get mouse input for horizontal and vertical movement
      float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
      float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

      // Adjust the vertical rotation based on mouse Y input
      xRotation -= mouseY;

      // Clamp the vertical rotation to prevent over-rotation
      xRotation = Math.Clamp(xRotation, -90f, 90f);

      // Apply the vertical rotation to the camera
      transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

      // Rotate the player's body horizontally based on mouse X input
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

      if (canLook)
      {
        if (isLocalPlayer && GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
        {
          Look();
        }
        else if (GameManager.Instance.gameType == GameManager.GameType.SinglePlayer)
        {
          Look();
        }
      }
    }

    public void SetCanLook(bool x)
    {
      canLook = x;
    }
  }
}
