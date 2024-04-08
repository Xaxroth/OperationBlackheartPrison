using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChests : MonoBehaviour
{
    public GameObject[] PossibleChests;

    public List<Chest> ActiveChests = new List<Chest>();

    int ChestSpawnChance = 20;
    void Awake()
    {
        Shuffle(PossibleChests);

        for (int i = 0; i < PossibleChests.Length; i++) 
        {
            int DiceRoll = Random.Range(0, 100);

            if (DiceRoll < ChestSpawnChance) 
            {
                PossibleChests[i].SetActive(true);
                ActiveChests.Add(PossibleChests[i].GetComponent<Chest>());
            }
        
        }
    }

    void Shuffle(GameObject[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    void Update()
    {
        
    }
}
