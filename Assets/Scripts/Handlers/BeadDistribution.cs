using System;
using UnityEngine;

public class BeadDistribution : MonoBehaviour
{
    public GameObject commonBeadPrefab;
    public GameObject rareBeadPrefab;
    public GameObject mythicBeadPrefab;

    private float commonProb = 0.9f;
    private float rareProb = 0.09f;
    private float mythicProb = 0.01f;

    public static BeadDistribution instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = new BeadDistribution(commonBeadPrefab, rareBeadPrefab, mythicBeadPrefab);
    }

    public BeadDistribution(GameObject c, GameObject r, GameObject m)
    {
        commonBeadPrefab = c;
        rareBeadPrefab = r;
        mythicBeadPrefab = m;
    }

    public GameObject[] DistributeBeads()
    {
        int beadQuantity = UnityEngine.Random.Range(1, 5);
        GameObject[] beads = new GameObject[beadQuantity];
        //Debug.Log($"ADD BEAD: {commonBeadPrefab}");
        for (int i = 0; i < beadQuantity; i++)
        {
            Debug.Log($"ADD BEAD: {commonBeadPrefab.name}");
            beads[i] = commonBeadPrefab;
        }
        return beads;
    }
}
