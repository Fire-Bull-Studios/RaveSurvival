using UnityEngine;
using RaveSurvival;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class Player : Entity
{
    public enum Scalar
    {
        DamageMult = 0,
        DamageAdd,
        SpeedAdd,
        HealthAdd,
    }
    public static readonly int PlayerLayer = 9;
    // Reference to the player's camera
    public Camera cam;

    public Animator animator;
    public GameObject mesh;

    // Reference to the player's gun
    public Gun gun;

    // Player's scalars
    private float damageMult = 1.0f;

    // Transform representing the position of the camera
    public Transform cameraPos;
    public PlayerUIManager uIManager;
    public KandiManager kandiManager;
    public PlayerMoveHandler moveHandler;
    public PlayerLookHandler lookHandler;
    public List<Interactable> curCollided;

    // Player's health value
    private float gunNoiseRange = 0f;

    private bool canShoot = true;
    private bool canInteract = true;

    public String interactBtn = "E";

    Vector3 meshPos = new Vector3(0.1f, -1.675f, -0.1f);
    string ammoStr;

    private InputSystem_Actions inputActions;
    private bool shootHeld = false;
    private bool reloadPressed = false;


    void Awake()
    {
        inputActions = new InputSystem_Actions();

    }

    void OnEnable()
    {
        if (inputActions == null)
            inputActions = new InputSystem_Actions();

        inputActions.Player.Enable();

        inputActions.Player.Shoot.performed += OnShoot;
        inputActions.Player.Shoot.canceled += OnShoot;
        inputActions.Player.Reload.performed += OnReload;
    }

    void OnDisable()
    {
        inputActions.Player.Shoot.performed -= OnShoot;
        inputActions.Player.Shoot.canceled -= OnShoot;
        inputActions.Player.Reload.performed -= OnReload;
        inputActions.Player.Disable();
    }


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
        }
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

            if (shootHeld)
            {
                if (GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
                {
                    gun.OnlineFire(Time.time);
                }
                else
                {
                    gun.Fire(Time.time, damageMult);
                    AlertNearEnemies();
                }

                ammoStr = $"{gun.magazineAmmo} / {gun.totalAmmo}";
                uIManager.SetAmmoText(ammoStr);
            }
            if (reloadPressed)
            {
                reloadPressed = false;

                gun.Reload();
                ammoStr = $"{gun.magazineAmmo} / {gun.totalAmmo}";
                uIManager.SetAmmoText(ammoStr);
            }

        }
        if (canInteract)
        {
            if (Input.GetKeyDown(KeyCode.E) && curCollided.Count > 0)
            {
                Interactable interact = curCollided.Last();
                RemoveInteractItem(interact);
                interact.Interact(this);
            }
        }
    }

    private void OnShoot(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        shootHeld = ctx.ReadValue<float>() > 0.5f;
    }

    private void OnReload(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        reloadPressed = true;
    }


    private void AttachCamera(Camera camera)
    {
        // Attach the camera to the player's camera position
        camera.transform.parent = cameraPos.transform;
        camera.transform.position = cameraPos.position;
        camera.transform.rotation = cameraPos.rotation;
        camera.GetComponent<AudioListener>().enabled = true;

        // Link the camera to the gun
        gun.SetBulletStart(camera.gameObject.transform);
    }

    private void MakeMeshChildOfCamera()
    {
        mesh.transform.parent = cam.gameObject.transform;
        mesh.transform.localPosition = meshPos;
    }

    public float GetDamageMult()
    {
        return damageMult;
    }

    public override void AddKandi(Kandi kandi)
    {
        Kandi.Effect effect = kandi.GetEffect();
        switch (effect.scalar)
        {
            case Player.Scalar.DamageMult:
                damageMult += effect.value;
                break;
            default:
                DebugManager.Instance.Print($"Invalid Scalar Type", DebugManager.DebugLevel.Production);
                break;
        }
        kandiManager.AddKandi(kandi.kandiModel);
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

    private void OnTriggerEnter(Collider collider)
    {
        Interactable interact = collider.gameObject.GetComponent<Interactable>();
        if (interact != null)
        {
            DebugManager.Instance.Print($"COLLISION NAME: {collider.gameObject.name}", DebugManager.DebugLevel.Paul);
            if (!curCollided.Contains(interact))
            {
                if (interact.IsInstantInteract())
                {
                    interact.Interact(this);
                }
                else
                {
                    curCollided.Add(interact);
                    uIManager.SetInteractText($"Press {interactBtn} to {interact.GetAction()} {interact.GetName()}");
                }
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        Interactable interact = collider.gameObject.GetComponent<Interactable>();
        if (interact != null)
        {
            RemoveInteractItem(interact);
        }
    }

    private void RemoveInteractItem(Interactable interact)
    {
        curCollided.Remove(interact);
        if (curCollided.Count() > 0)
        {
            Interactable col = curCollided.Last();
            uIManager.SetInteractText($"Press {interactBtn} to {col.GetAction()} {col.GetName()}");
        }
        else
        {
            uIManager.DisableInteractText();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, gunNoiseRange);
        Gizmos.color = Color.coral;
    }
}