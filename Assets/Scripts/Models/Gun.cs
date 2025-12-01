using System;
using Mirror;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using RaveSurvival;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Gun : Weapon
{
    public float velocity = 15f;
    public float fireRate = 15f;

    private float nextTimeToFire = 0f;
    private bool canShoot = true;

    private bool hasAmmo;

    public BulletType bulletType = BulletType.RAYCAST;

    [SerializeField]
    public Transform bulletStart;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    public GameObject projectile;

    public AudioClip fireSound;
    public enum BulletType
    {
        RAYCAST = 0,
        PROJECTILE
    }

    public uint startingAmmo = 144;
    public uint magazineSize = 12;
    public uint magazineAmmo;
    public uint totalAmmo;
    //private const int PlayerLayer = 9;

    public override void Awake()
    {
        base.Awake();
        if (this.bulletType == BulletType.PROJECTILE && bulletStart == null)
        {
            bulletStart = transform.Find("bulletSpawn");
            if (bulletStart == null)
            {
                Debug.LogError("Gun: bulletSpawn Transform not found on this weapon!");
            }
        }
        //only the player should 'have ammo'
        hasAmmo = gameObject.layer == Player.PlayerLayer;
        totalAmmo = hasAmmo ? startingAmmo : uint.MaxValue;
        magazineAmmo = hasAmmo ? magazineSize : uint.MaxValue;
    }


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        isReady = true;
    }

    public override void Start()
    {
        isReady = true;
    }

    public void SetBulletStart(Transform start)
    {
        bulletStart = start;
    }

    // ...existing code...
    public bool Reload()
    {
        if (!hasAmmo)
        {
            return false;
        }

        if (totalAmmo == 0)
        {
            DebugManager.Instance.Print("Out of ammo bro", DebugManager.DebugLevel.Verbose);
            return false;
        }

        uint reloadAmount = magazineSize - magazineAmmo;
        if (reloadAmount == 0)
        {
            // Magazine already full
            return true;
        }

        // transfer is the number of rounds we can actually move into the magazine
        uint transfer = totalAmmo <= reloadAmount ? totalAmmo : reloadAmount;

        magazineAmmo += transfer;
        totalAmmo -= transfer;

        DebugManager.Instance.Print($"Ammo = {magazineAmmo} / {totalAmmo}", DebugManager.DebugLevel.Paul);
        return true;
    }

    public bool AddAmmo()
    {
        DebugManager.Instance.Print("Adding ammo...", DebugManager.DebugLevel.Paul);
        if (!hasAmmo)
        {
            return false;
        }

        magazineAmmo = magazineSize;
        totalAmmo = startingAmmo;
        return true;
    }

    public void OnlineFire(float currentTime)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("Error... Game manager instance is null when trying to shoot!");
            return;
        }

        if (bulletStart == null)
        {
            Debug.LogWarning("bulletStart is null, cannot shoot");
            return;
        }

        if (currentTime > nextTimeToFire)
        {
            // Get the origin and direction of the shot from the camera
            Vector3 originPosition = bulletStart.position;
            Vector3 direction = bulletStart.forward;

            if (!isServer)
            {
                Debug.LogWarning("Enemy tried to shoot but not on the server");
                CmdShoot(originPosition, direction);
                return;
            }
            else
            {
                ServerShoot(originPosition, direction);
            }
            nextTimeToFire = Time.time + (1f / fireRate);
            ServerShoot(originPosition, direction);

            if (isLocalPlayer)
            {
                muzzleFlash.Play();
                audioSource.Play();
            }

            nextTimeToFire = currentTime + fireRate;
        }
    }

    public void Fire(float currentTime, float damageMult = 1.0f)
    {
        float finalDamage = damage * damageMult;
        if (GameManager.Instance == null)
        {
            Debug.LogError("Error... Game manager instance is null when trying to shoot!");
            return;
        }

        if (bulletStart == null)
        {
            Debug.LogWarning("bulletStart is null, cannot shoot");
            return;
        }

        if (currentTime > nextTimeToFire)
        {
            if (hasAmmo)
            {
                if (magazineAmmo <= 0)
                {
                    return;
                }
                magazineAmmo--;
                DebugManager.Instance.Print($"Current ammo in magazine = {magazineAmmo}", DebugManager.DebugLevel.Verbose);
            }

            // Get the origin and direction of the shot from the camera
            Vector3 originPosition = bulletStart.position;
            Vector3 direction = bulletStart.forward;

            PlayMuzzleFlash();
            if (bulletType == BulletType.RAYCAST)
            {

                RaycastHit hit;
                if (Physics.Raycast(originPosition, direction, out hit, range))
                {
                    Entity entity = hit.transform.GetComponent<Entity>();
                    if (entity != null && entity.name != owner.name)
                    {
                        entity.TakeDamage(finalDamage, bulletStart, originPosition, entity);
                    }

                    GameObject impactFx = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactFx, 2f);
                }
            }
            else if (bulletType == BulletType.PROJECTILE)
            {
                GameObject projectile = Instantiate(this.projectile, originPosition, Quaternion.LookRotation(direction));
                projectile.GetComponent<Projectile>().FireBullet(velocity, owner);
            }

            nextTimeToFire = currentTime + (1f / fireRate);
        }

        return;
    }

    [Server]
    void ServerShoot(Vector3 originPosition, Vector3 direction)
    {

        RpcPlayMuzzleFlash();

        if (bulletType == BulletType.RAYCAST)
        {

            RaycastHit hit;
            if (Physics.Raycast(originPosition, direction, out hit, range))
            {
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, bulletStart, originPosition, enemy);
                }

                GameObject impactFx = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactFx, 2f);
            }
        }
        else if (bulletType == BulletType.PROJECTILE)
        {
            this.projectile.layer = LayerMask.NameToLayer("Default");
            GameObject projectile = Instantiate(this.projectile, originPosition, Quaternion.LookRotation(direction));
            NetworkServer.Spawn(projectile);
            projectile.GetComponent<Projectile>().FireBullet(velocity, owner);
        }
    }

    [Command]
    void CmdShoot(Vector3 originPosition, Vector3 direction)
    {
        RpcPlayMuzzleFlash();
        if (bulletType == BulletType.RAYCAST)
        {

            RaycastHit hit;
            if (Physics.Raycast(originPosition, direction, out hit, range))
            {
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, bulletStart, originPosition, enemy);
                }

                GameObject impactFx = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactFx, 2f);
            }
        }
        else if (bulletType == BulletType.PROJECTILE)
        {
            GameObject projectile = Instantiate(this.projectile, originPosition, Quaternion.LookRotation(direction));
            NetworkServer.Spawn(projectile);
            projectile.GetComponent<Projectile>().FireBullet(velocity, owner);
        }
    }

    /// <summary>
    /// ClientRpc method executed on all clients to play the muzzle flash effect.
    /// Ensures the effect is played for non-local players.
    /// </summary>
    [ClientRpc]
    void RpcPlayMuzzleFlash()
    {
        if (audioSource.clip == null || audioSource.clip != fireSound)
        {
            audioSource.clip = fireSound;
        }
        audioSource.Play();
        muzzleFlash.Play();
    }

    void PlayMuzzleFlash()
    {
        if (audioSource.clip == null || audioSource.clip != fireSound)
        {
            audioSource.clip = fireSound;
        }
        audioSource.Play();
        muzzleFlash.Play();
    }

    public void SetCanShoot(bool x)
    {
        canShoot = x;
    }
}
