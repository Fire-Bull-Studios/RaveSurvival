using System;
using System.Xml.Serialization;
using Codice.Client.BaseCommands.Import;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
  public enum PlayerAttribute
  {
    health = 0,
    ammo,
    aliveState
  }

  public HealthBar healthBar;
  public DamageEffectHandler dmgFxHandler;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void ChangePlayerAttribute(PlayerAttribute attribute, int value)
  {
    switch (attribute)
    {
      default:
        break;
    }
  }

  public void TakeDamage(float value)
  {
    healthBar.HandleHealthChange(value);
    dmgFxHandler.CreateDamageFx();
  }

  private void switchToDeathScene()
  {
    // TODO
  }

  private void changeAmmo()
  {

  }
}
