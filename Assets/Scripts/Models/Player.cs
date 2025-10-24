using UnityEngine;
using UnityEngine.Animations;
using Mirror;
using UnityEngine.UIElements;
using RaveSurvival;

public class Player : NetworkBehaviour
{
  // Reference to the player's camera
  public Camera cam;

  // Reference to the player's gun
  public Gun gun;

  // Transform representing the position of the camera
  public Transform cameraPos;
  public PlayerUIManager uIManager;

  // Player's health value
  public float maxHealth = 50.0f;
  private float health = 0f;
  private float gunNoiseRange = 0f;

  /// <summary>
  /// Unity's Start method, called before the first frame update.
  /// Sets up the camera for the local player and links it to the gun.
  /// </summary>
  public void Start()
  {
    health = maxHealth;
    // Find the first camera in the scene
    Camera camera = FindFirstObjectByType<Camera>();

    if (GameManager.Instance == null)
    {
      Debug.LogError("Error... GameManager is null on player start");
      return;
    }

    switch (GameManager.Instance.gameType)
    {
      case GameManager.GameType.OnlineMultiplayer:
        // Check if this is the local player
        if (isLocalPlayer)
        {
          AttachCamera(camera);
        }
        break;
      case GameManager.GameType.SinglePlayer:
        AttachCamera(camera);
        break;
      case GameManager.GameType.LocalMultiplayer:
        //TODO implement this later
        break;
      default:
        Debug.LogError("Invalid game type enum...");
        break;
    }

    if (gun != null)
    {
      gunNoiseRange = gun.soundRange;
      Debug.Log($"{gunNoiseRange}: {gun.soundRange}");
    }
  }

  private void AttachCamera(Camera camera)
  {
    // Attach the camera to the player's camera position
    camera.transform.parent = cameraPos.transform;
    camera.transform.position = cameraPos.position;
    camera.transform.rotation = cameraPos.rotation;

    // Link the camera to the gun
    gun.SetBulletStart(camera.gameObject.transform);
  }

  /// <summary>
  /// Unity's Update method, called once per frame.
  /// Currently empty but can be used for player-specific updates.
  /// </summary>
  void Update()
  {
    // Placeholder for future update logic
  }

  /// <summary>
  /// Reduces the player's health when taking damage.
  /// If health reaches zero, logs a message indicating the player was killed.
  /// </summary>
  /// <param name="dmg">Amount of damage to apply</param>
  /// <param name="killedBy">GameObject that caused the player's death</param>
  public void TakeDamage(float dmg, GameObject killedBy)
  {
    // Subtract damage from health
    health -= dmg;
    int healthPercent = (int)(health / maxHealth * 100);
    //Debug.Log($"Health percentage: {health / maxHealth}, {health / maxHealth * 100}, {healthPercent}");
    uIManager.ChangePlayerAttribute(PlayerUIManager.PlayerAttribute.health, healthPercent);

    // Check if health has dropped to zero or below
    if (health <= 0)
    {
      health = 0; // Ensure health doesn't go negative
      Debug.Log("You were just killed by: " + killedBy);
    }
  }

  public void AlertNearEnemies()
  {
    Collider[] colliders = Physics.OverlapSphere(transform.position, gunNoiseRange, LayerMask.NameToLayer("Enemy"), QueryTriggerInteraction.Collide);
    foreach (Collider col in colliders)
    {
      Enemy enemy = col.gameObject.GetComponent<Enemy>();
      if (enemy != null)
      {
        enemy.PlayerSpotted(transform);
      }
    }
  }

  void OnDrawGizmos()
  {
    Gizmos.DrawWireSphere(transform.position, gunNoiseRange);
    Gizmos.color = Color.coral;
  }
}