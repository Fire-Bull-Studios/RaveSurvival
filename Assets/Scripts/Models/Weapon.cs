using Mirror;
using UnityEngine;
using RaveSurvival;

public class Weapon : Interactable
{
    public float damage = 10f;
    public float fireRange = 100f;
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

    // public override void OnStartLocalPlayer()
    // {
    //     base.OnStartLocalPlayer();
    //     isReady = true;
    // }

    public override void Interact(Player player)
    {
        DebugManager.Instance.Print("Im interacting with you from the weapon class mother fuh", DebugManager.DebugLevel.Verbose);

    }

    protected override void Start()
    {
        base.Start();
        isReady = true;
    }

}