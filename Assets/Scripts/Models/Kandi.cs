using RaveSurvival;
using UnityEngine;

public class Kandi : Interactable
{

    public override void Interact(Player player)
    {
        DebugManager.Instance.Print($"Interacted with {name}", DebugManager.DebugLevel.Minimal);
        return;
    }
}
