using Mirror;
using UnityEngine;
using RaveSurvival;

public class Weapon : NetworkBehaviour
{
  public float damage = 10f;
  public float range = 100f;
  public float soundRange = 100f;
  public Entity owner = null;

  protected bool isEnabled = true;
  protected AudioSource audioSource;
  protected bool isReady = false;

  public virtual void Awake()
  {
    audioSource = GetComponent<AudioSource>();
    Entity entity = GetComponentInParent<Entity>();
    if (entity != null)
    {
      owner = entity;
    }
  }

  public override void OnStartLocalPlayer()
  {
    base.OnStartLocalPlayer();
    isReady = true;
  }

  public virtual void Start()
  {
    isReady = true;
  }
}