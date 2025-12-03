using UnityEngine;

public class Beadery : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        throw new System.NotImplementedException();
    }
}
