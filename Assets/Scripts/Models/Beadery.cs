using System;
using System.Collections.Generic;
using PlasticGui.WorkspaceWindow.BrowseRepository;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Beadery : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Camera beaderyCam;
    public bool isInteracting = false;
    public LayerMask obstructionMask;
    public Material disableMaterial;
    public InteractOption[] interactOptions;
    public CrankHandler crank;

    private InputSystem_Actions inputActions;
    private int commonScore = 0;
    private int rareScore = 0;
    private int mythicScore = 0;
    private InteractOption selectedOption = null;
    private bool hovering = false;

    protected override void Start()
    {
        base.Start();
    }

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnExit(InputAction.CallbackContext ctx)
    {
        isInteracting = false;
        foreach (InteractOption option in interactOptions)
        {
            option.Reset();
        }
        inputActions.Player.Exit.performed -= OnExit;
        inputActions.UI.Click.performed -= OnClick;
        inputActions.Player.Disable();
        inputActions.UI.Disable();
        crank.DisableCrank();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (hovering)
        {
            foreach (InteractOption option in interactOptions)
            {
                if (option.IsHovered() && option.isEnabled)
                {
                    if (selectedOption != null)
                    {
                        selectedOption.Reset();
                    }
                    option.Select();
                    selectedOption = option;
                    crank.EnableCrank(beaderyCam);
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isInteracting)
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = beaderyCam.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100, obstructionMask))
            {
                InteractOption option = hit.transform.GetComponent<InteractOption>();
                if (option != null)
                {
                    hovering = true;
                    foreach (InteractOption interactOption in interactOptions)
                    {
                        if (option.isEnabled && option.name == interactOption.name && !interactOption.selected)
                        {
                            interactOption.Hover();
                        }
                        else if (option.isEnabled && option.name != interactOption.name && !interactOption.selected)
                        {
                            interactOption.Reset();
                        }
                        else
                        {
                            //DO NOTHING
                        }
                    }
                }
            }
            else
            {
                hovering = false;
                foreach (InteractOption interactOption in interactOptions)
                {
                    if (!interactOption.selected)
                        interactOption.Reset();
                }
            }
            Debug.DrawRay(ray.origin, ray.direction, Color.green);
        }
    }

    public override void Interact(Player player)
    {
        player.SwapToCamera(beaderyCam);
        player.lookHandler.SetCursorActive(true);
        player.lookHandler.SetCanLook(false);
        player.SetCanShoot(false);
        player.moveHandler.SetCanMove(false);
        isInteracting = true;

        foreach (InteractOption option in interactOptions)
        {
            option.CheckDisabled(commonScore, rareScore, mythicScore);
        }
        crank.DisableCrank();

        if (inputActions == null)
            inputActions = new InputSystem_Actions();

        inputActions.Player.Enable();
        inputActions.UI.Enable();
        inputActions.Player.Exit.performed += OnExit;
        inputActions.UI.Click.performed += OnClick;
    }
}
