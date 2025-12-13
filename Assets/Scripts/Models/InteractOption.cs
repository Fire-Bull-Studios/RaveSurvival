using UnityEngine;

public class InteractOption : MonoBehaviour
{
    public int commonCost = 0;
    public int rareCost = 0;
    public int mythicCost = 0;
    public bool isEnabled = true;
    public bool selected = false;
    public bool hovered = false;

    private Color initColor;

    Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        if (mat)
        {
            initColor = mat.color;
        }
    }

    public void CheckDisabled(int common, int rare, int mythic)
    {
        if (common < commonCost)
        {
            isEnabled = false;
            if (mat)
            {
                mat.color = Color.black;
            }
        }
    }

    public void Reset()
    {
        if (mat)
        {
            mat.DisableKeyword("_EMISSION");
        }
        hovered = false;
        selected = false;
    }

    public void Hover()
    {
        if (mat)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.yellow * 0.3f);
        }
        hovered = true;
    }

    public bool IsHovered()
    {
        return hovered;
    }

    public void Select()
    {
        selected = true;
        if (mat)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.white * 0.3f);
        }
    }
}
