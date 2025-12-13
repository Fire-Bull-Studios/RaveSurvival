using System;
using System.Collections.Generic;
using RaveSurvival;
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

    [Serializable]
    public struct WeaponIconEntry
    {
        public Gun.GunType gunType;
        public Sprite icon;
    }
    [SerializeField]
    private WeaponIconEntry[] weaponIcons;
    private Dictionary<Gun.GunType, Sprite> weaponMap;

    public Image weaponIcon;

    void Awake()
    {
        weaponMap = new Dictionary<Gun.GunType, Sprite>();
        foreach (var entry in weaponIcons)
        {
            weaponMap[entry.gunType] = entry.icon;
        }
    }

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

    public void SetWeaponIcon(Gun.GunType gunType)
    {
        if (weaponMap.TryGetValue(gunType, out Sprite icon))
        {
            weaponIcon.sprite = icon;
            weaponIcon.enabled = true;
        }
        else
        {
            DebugManager.Instance.Print($"No weapon icon mapped for {gunType}", DebugManager.DebugLevel.Minimal);
            weaponIcon.enabled = false;
        }
    }
}
