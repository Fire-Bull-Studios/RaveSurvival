using RaveSurvival;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BeadUIHandler : MonoBehaviour
{
    public TextMeshProUGUI commonBeadTxt;
    public TextMeshProUGUI rareBeadTxt;
    public TextMeshProUGUI mythicBeadTxt;

    public void UpdateBeadCount(int count, Bead.BeadType type)
    {
        switch (type)
        {
            case Bead.BeadType.common:
                commonBeadTxt.text = count.ToString();
                break;
            case Bead.BeadType.rare:
                rareBeadTxt.text = count.ToString();
                break;
            case Bead.BeadType.mythic:
                mythicBeadTxt.text = count.ToString();
                break;
            default:
                DebugManager.Instance.Print("If you are getting this message than you messed up bro", DebugManager.DebugLevel.Production);
                break;
        }
    }

    public void Reset()
    {
        Debug.Log("HERE!!!");
        commonBeadTxt.text = "0";
        rareBeadTxt.text = "0";
        mythicBeadTxt.text = "0";
    }
}
