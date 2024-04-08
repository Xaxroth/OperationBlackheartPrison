using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class PrisonChestPuzzle : MonoBehaviour
{
    public SpawnChests spawnChests;
    public List<Chest> Chests = new List<Chest>();
    public GameObject PrisonKey;
    public RandomDoor DoorToLock;
    public GameObject AdditionalDanger;
    public Chest correctChest;

    public int ChanceToAppear = 50;

    void Start()
    {
        int DiceRoll = Random.Range(0, 100);

        if (DiceRoll < ChanceToAppear)
        {
            AdditionalDanger.SetActive(true);
            // Now this is a locked room, with monsters of course
            DoorToLock.locked = true;

            int ChestToContainKey = Random.Range(0, spawnChests.ActiveChests.Count);

            correctChest = spawnChests.ActiveChests[ChestToContainKey];

            GameObject PrisonKeyGO = Instantiate(PrisonKey, spawnChests.ActiveChests[ChestToContainKey].gameObject.transform.position + new Vector3(0, 2, 0), Quaternion.identity);

            PrisonKeyGO.SetActive(false);

            spawnChests.ActiveChests[ChestToContainKey].Payload.Add(PrisonKeyGO);
        }

    }
}
