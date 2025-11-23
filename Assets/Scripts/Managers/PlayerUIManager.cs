using System;
using System.Xml.Serialization;
using Codice.Client.BaseCommands.Import;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public TextMeshProUGUI ammoText;
    public GameObject deathScreen;

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

    public void SwitchToDeathScene()
    {
        deathScreen.SetActive(true);
    }

    public void SetAmmoText(string text)
    {
        ammoText.text = text;
    }

    private void changeAmmo()
    {

    }
}
