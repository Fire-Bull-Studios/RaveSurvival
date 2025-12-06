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
    public TextMeshProUGUI interactText;
    public GameObject deathScreen;
    public BeadUIHandler beadUI;

    void Start()
    {
        interactText.enabled = false;
        beadUI.Reset();
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

    public void SwitchToDeathScene()
    {
        deathScreen.SetActive(true);
    }

    public void SetAmmoText(string text)
    {
        ammoText.text = text;
    }

    public void SetInteractText(string text)
    {
        interactText.enabled = true;
        interactText.text = text;
    }

    public void DisableInteractText()
    {
        interactText.enabled = false;
    }

    public void UpdateBeadUI(int count, Bead.BeadType type)
    {
        beadUI.UpdateBeadCount(count, type);
    }

    private void changeAmmo()
    {

    }
}
