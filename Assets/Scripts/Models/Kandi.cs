using RaveSurvival;
using UnityEngine;

public class Kandi : Interactable
{

    public override void Interact(Player player)
    {
        DebugManager.Instance.Print($"Interacted with {name}", DebugManager.DebugLevel.Minimal);
        player.AddDamageMult(4);
        Destroy(this.gameObject);
        return;
    }
}
