using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonChestPuzzle : MonoBehaviour
{
    public Chest[] Chests;
    public GameObject PrisonKey;

    public Chest correctChest;

    void Start()
    {
        int ChestToContainKey = Random.Range(0, Chests.Length);

        correctChest = Chests[ChestToContainKey];


        GameObject PrisonKeyGO = Instantiate(PrisonKey, Chests[ChestToContainKey].gameObject.transform.position + new Vector3(0, 2, 0), Quaternion.identity);

        PrisonKeyGO.SetActive(false);

        Chests[ChestToContainKey].Payload.Add(PrisonKeyGO);
    }
}
