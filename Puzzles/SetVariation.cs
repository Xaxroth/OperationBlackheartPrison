using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVariation : MonoBehaviour
{
    public GameObject[] Variations;
    void Start()
    {
        Variations[Random.Range(0, Variations.Length)].SetActive(true);
    }
}
