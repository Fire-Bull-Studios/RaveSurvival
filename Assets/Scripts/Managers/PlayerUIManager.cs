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
      case PlayerAttribute.health:
        changeHealth(value);
        break;
      default:
        break;
    }
  }

  private void changeHealth(int value)
  {
    float health = ((float)value) / 100f;
    Debug.Log($"health: {health}");
    healthBar.HandleHealthChange(health);
    // TODO
  }

  private void switchToDeathScene()
  {
    // TODO
  }

  private void changeAmmo()
  {

  }
}
