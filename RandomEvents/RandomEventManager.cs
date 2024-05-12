using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    public static RandomEventManager Instance;

    public int FriendlyDemonsKilled = 0;

    public bool SoultrapperKilled;

    public GameObject FlashbangMonster;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int ChanceToSpawn = 0;

        ChanceToSpawn = FriendlyDemonsKilled;

        int Roll = Random.Range(0, 100);

        if (Roll < ChanceToSpawn && !SoultrapperKilled)
        {
            // SpawnEvent (FirendlyDemons)
            Instantiate(FlashbangMonster, Vector3.zero, Quaternion.identity);
            SoultrapperKilled = true;
            Debug.Log("ACK");
        }
    }
}
