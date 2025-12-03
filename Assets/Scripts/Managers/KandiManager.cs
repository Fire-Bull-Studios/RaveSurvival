using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class KandiManager : MonoBehaviour
{
    public Transform[] kandiSlots = new Transform[4];
    private int curKandi = 0;
    //private List<GameObject> kandiList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform temp = transform.GetChild(i);
            temp.GetComponent<MeshFilter>().mesh = null;
            kandiSlots[i] = temp;
        }
    }

    public void AddKandi(GameObject kandi)
    {
        curKandi++;
        Transform temp = kandiSlots[curKandi - 1];
        GameObject model = Instantiate(kandi);
        model.transform.localScale = new Vector3(0.075f, 0.075f, 0.075f);
        model.transform.position = temp.position;
        model.transform.rotation = temp.rotation;
        model.transform.parent = temp;
    }
}
