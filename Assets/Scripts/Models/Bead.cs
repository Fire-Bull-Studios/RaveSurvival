using UnityEngine;

public class Bead : Interactable
{
    public enum BeadType
    {
        common = 0,
        rare,
        mythic
    }

    public BeadType beadType = BeadType.common;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        return;
    }

    public override void Interact(Player player)
    {
        Destroy(this.gameObject);
        return;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
