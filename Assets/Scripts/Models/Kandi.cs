using System;
using RaveSurvival;
using UnityEngine;

public class Kandi : Interactable
{
    public struct Effect
    {
        public Player.Scalar scalar { get; }
        public float value { get; }

        public Effect(Player.Scalar scalar, float value)
        {
            this.scalar = scalar;
            this.value = value;
        }
    }
    public GameObject kandiModel;
    public Effect effect;
    public Player.Scalar scalar;
    public float value;

    protected override void Start()
    {
        base.Start();
        effect = new Effect(scalar, value);
    }

    public override void Interact(Player player)
    {
        DebugManager.Instance.Print($"Interacted with {name}", DebugManager.DebugLevel.Minimal);
        Debug.Log($"KANDI MODEL: {kandiModel}");
        player.AddKandi(this);
        Destroy(this.gameObject);
        return;
    }

    public Effect GetEffect()
    {
        return effect;
    }
}
