using UnityEngine;
using RaveSurvival;
using Mirror;
using System;
using System.Collections;

public class Entity : NetworkBehaviour
{
    public float maxHealth = 50.0f;
    protected float health = 0f;

    public virtual void Start()
    {
        health = maxHealth;
    }

    public virtual void TakeDamage(float dmg, Transform bulletDirection, Vector3 pos, Entity shotBy)
    {
        DebugManager.Instance.Print($"Health: {health}\tDamage: {dmg}", DebugManager.DebugLevel.Paul);
        health -= dmg;
        if (health <= 0f)
        {
            health = 0;
            Die(shotBy.name);
        }
    }

    protected virtual void Die(String shotBy)
    {
        DebugManager.Instance.Print($"{name} was just killed by {shotBy}", DebugManager.DebugLevel.Verbose);
    }

    public virtual void AddKandi(Kandi kandi)
    {
        return;
    }
}