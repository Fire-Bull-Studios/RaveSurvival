using UnityEngine;
using UnityEngine.Animations;
using Mirror;
using UnityEngine.UIElements;
using RaveSurvival;
using System;

public class Player : Entity
{
    // Reference to the player's camera
    public Camera cam;

    public Animator animator;
    public GameObject mesh;

    // Reference to the player's gun
    public Gun gun;

    // Transform representing the position of the camera
    public Transform cameraPos;
    public PlayerUIManager uIManager;
    public PlayerMoveHandler moveHandler;
    public PlayerLookHandler lookHandler;

    // Player's health value
    private float gunNoiseRange = 0f;

    private bool canShoot = true;

    public String interactBtn = "E";

    Vector3 meshPos = new Vector3(0.1f, -1.675f, -0.1f);
    string ammoStr;

    /// <summary>
    /// Unity's Start method, called before the first frame update.
    /// Sets up the camera for the local player and links it to the gun.
    /// </summary>
    public override void Start()
    {
        base.Start();
        health = maxHealth;
        ammoStr = $"{gun.magazineAmmo} / {gun.totalAmmo}";
        uIManager.SetAmmoText(ammoStr);
        // Find the first camera in the scene
        cam = FindFirstObjectByType<Camera>();

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
                    AttachCamera(cam);
                }
                break;
            case GameManager.GameType.SinglePlayer:
            case GameManager.GameType.Endless:
                AttachCamera(cam);
                MakeMeshChildOfCamera();
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

    private void MakeMeshChildOfCamera()
    {
        mesh.transform.parent = cam.gameObject.transform;
        mesh.transform.localPosition = meshPos;
    }

    /// <summary>
    /// Unity's Update method, called once per frame.
    /// Currently empty but can be used for player-specific updates.
    /// </summary>
    void Update()
    {
        if (canShoot)
        {
            if (GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer && !isLocalPlayer)
            {
                return;
            }

            if (Input.GetButton("Fire1"))
            {
                if (GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
                {
                    gun.OnlineFire(Time.time);
                }
                else
                {
                    gun.Fire(Time.time);
                    AlertNearEnemies();
                }
                ammoStr = $"{gun.magazineAmmo} / {gun.totalAmmo}";
                uIManager.SetAmmoText(ammoStr);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                gun.Reload();
                ammoStr = $"{gun.magazineAmmo} / {gun.totalAmmo}";
                uIManager.SetAmmoText(ammoStr);
            }
        }
    }

    /// <summary>
    /// Reduces the player's health when taking damage.
    /// If health reaches zero, logs a message indicating the player was killed.
    /// </summary>
    /// <param name="dmg">Amount of damage to apply</param>
    /// <param name="killedBy">GameObject that caused the player's death</param>
    public override void TakeDamage(float dmg, Transform bulletDirection, Vector3 pos, Entity shotBy)
    {
        base.TakeDamage(dmg, bulletDirection, pos, shotBy);
        // Subtract damage from health
        float healthPercent = health / maxHealth;
        //Debug.Log($"Health percentage: {health / maxHealth}, {health / maxHealth * 100}, {healthPercent}");
        uIManager.TakeDamage(healthPercent);
    }

    protected override void Die(String shotBy)
    {
        base.Die(shotBy);
        uIManager.SwitchToDeathScene();
        moveHandler.SetCanMove(false);
        lookHandler.SetCanLook(false);
        gun.SetCanShoot(false);
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

    void OnCollisionEnter(Collision collision)
    {
        DebugManager.Instance.Print($"COLLISION NAME: {collision.gameObject.name}", DebugManager.DebugLevel.Verbose);
        Debug.Log("INTERACTTTTTTT!!!!");
        Interactable interact = collision.gameObject.GetComponent<Interactable>();
        if (interact != null)
        {
            if (interact.IsInstantInteract())
            {
                interact.Interact(this);
            }
            else
            {
                uIManager.setInteractText($"Press {interactBtn} to {interact.GetAction()} {interact.GetName()}");
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, gunNoiseRange);
        Gizmos.color = Color.coral;
    }
}