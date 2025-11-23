using UnityEngine;
using RaveSurvival;
using Mirror;
using System;

public class Entity : NetworkBehaviour
{
  //public Weapon weapon;
  public float maxHealth = 50.0f;
  protected float health = 0f;

  public virtual void Start()
  {
    health = maxHealth;
  }

  public virtual void TakeDamage(float dmg, Transform bulletDirection, Vector3 pos, Entity shotBy)
  {
    Debug.Log($"Health: {health}\tDamage: {dmg}");
    health -= dmg;
    if (health <= 0f)
      {
        health = 0;
        Die(shotBy.name);
      }
  }

  protected virtual void Die(String shotBy)
  {
    Debug.Log($"{name} was just killed by {shotBy}");
  }
}