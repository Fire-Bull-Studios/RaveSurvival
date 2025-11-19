using Mirror;
using UnityEngine;
using RaveSurvival;

public class Weapon : NetworkBehaviour
{
  public float damage = 10f;
  public float range = 100f;
  public float fireRate = 15f;
  public float soundsRange = 100f;
  private bool isEnabled = true;

  private AudioSource audioSource;
  public AudioClip attackSounds;

  private bool isReady = false;

  public virtual void Awake()
  {
    audioSource = GetComponent<AudioSource>();
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