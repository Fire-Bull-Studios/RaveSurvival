using System;
using Mirror.BouncyCastle.Bcpg.Sig;
using PlasticGui;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public float range = 5.0f;
    private SphereCollider col = null;
    private Rigidbody rb = null;
    private bool isInteractable = true;
    private bool isInstantInteract = false;
    public string interactName = "item";
    public string action = "interact with";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = gameObject.AddComponent<SphereCollider>();
        col.radius = range;
        col.isTrigger = true;
    }

    public abstract void Interact(Player player);
    public bool IsInteractable()
    {
        return isInteractable;
    }
    public bool IsInstantInteract()
    {
        return isInstantInteract;
    }
    public string GetAction()
    {
        return action;
    }
    public string GetName()
    {
        return interactName;
    }
    public void Enable(bool x = true)
    {
        isInteractable = x;
    }
}
