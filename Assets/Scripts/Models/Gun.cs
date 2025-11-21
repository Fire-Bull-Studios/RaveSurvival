using System;
using Mirror;
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

  public void Fire(float currentTime)
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

      PlayMuzzleFlash();
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
        projectile.GetComponent<Projectile>().FireBullet(velocity, owner);
      }

        nextTimeToFire = currentTime + (1f / fireRate);
    }
    else
    {
      //Do nothing
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
