using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonsterRandomly : MonoBehaviour
{
    public int SpawnChance = 5;
    public int SpawnInterval = 1;

    public int EnemiesSpawned = 0;
    public int MaxNumberOfEnemiesToSpawn = 6;

    public GameObject EnemyToSpawn;
    public GameObject[] SpawnPositions;

    void Start()
    {
        InvokeRepeating("GenerateRandomNumber", SpawnInterval, SpawnInterval);
    }

    public void GenerateRandomNumber()
    {
        if (EnemiesSpawned < MaxNumberOfEnemiesToSpawn)
        {
            int RandomNumber = Random.Range(0, 100);

            if (RandomNumber < SpawnChance)
            {
                EnemiesSpawned++;
                GameObject NewEnemy = Instantiate(EnemyToSpawn, SpawnPositions[Random.Range(0, SpawnPositions.Length)].transform.position, Quaternion.identity);
            }
        }
    }
}
