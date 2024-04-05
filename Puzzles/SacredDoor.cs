using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacredDoor : MonoBehaviour
{
    public RandomDoor[] Doors;

    public string SceneToLoadInstead;
    void Start()
    {
        int SacredDoor = Random.Range(0, Doors.Length);

        Doors[SacredDoor].OverrideScene = SceneToLoadInstead;
        Doors[SacredDoor].nextSceneOverride = true;
    }
}
