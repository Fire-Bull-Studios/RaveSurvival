using UnityEngine;

public class Beadery : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Camera beaderyCam;
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Interact(Player player)
    {
        player.SwapToCamera(beaderyCam);
        player.lookHandler.SetCursorActive(true);
        player.SetCanShoot(false);
        player.moveHandler.SetCanMove(false);
    }
}
