using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrankHandler : MonoBehaviour
{
    public Renderer knob;
    public LayerMask obstructionMask;

    private bool isEnabled;
    private Camera beaderyCam = null;
    private Material mat;
    private Color initColor;
    private InputSystem_Actions inputActions;
    private bool holding = false;
    private bool grabbed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (knob != null)
        {
            mat = knob.material;
            initColor = mat.color;
        }
    }

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    // Update is called once per frame
    void Update()
    {
        if ((isEnabled && holding && beaderyCam != null) || grabbed)
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = beaderyCam.ScreenPointToRay(mousePos);
            float _xRot = Mathf.Clamp(-ray.direction.y + 0.3f, 0.6f, 0.9f);
            RaycastHit hit;
            if (!grabbed)
            {
                if (Physics.Raycast(ray.origin, ray.direction, out hit, 100, obstructionMask))
                {
                    if (hit.transform.gameObject.tag == "Crank")
                    {
                        grabbed = true;
                    }
                }
            }
            else
            {
                transform.rotation = new Quaternion(_xRot, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            }
        }
    }
    public void EnableCrank(Camera cam)
    {
        beaderyCam = cam;
        isEnabled = true;
        mat.color = initColor;
        inputActions.UI.Enable();
        inputActions.UI.Click.performed += OnPress;
    }

    public void DisableCrank()
    {
        isEnabled = false;
        mat.color = Color.black;
        inputActions.UI.Click.performed -= OnPress;
        inputActions.UI.Disable();
    }

    private void OnPress(InputAction.CallbackContext ctx)
    {
        holding = ctx.ReadValue<float>() > 0.7;
        if (ctx.ReadValue<float>() < 0.7)
        {
            grabbed = false;
        }
    }
}
