using UnityEngine;
using RaveSurvival;
using Mirror;

public class Entity : NetworkBehaviour
{
  public Weapon weapon;
  public float maxHealth = 50.0f;
  private float health = 0f;

  public virtual void Start()
  {
    health = maxHealth;
  }

  public void TakeDamage(float dmg, Entity shotBy)
  {
    health -= dmg;
  }
}