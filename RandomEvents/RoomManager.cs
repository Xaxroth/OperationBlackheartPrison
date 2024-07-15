using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    public int RoomsToSpawn = 25;
    public int RoomsSpawned = 0;

    public List<GameObject> Rooms = new List<GameObject>();
    public List<GameObject> RoomsActive = new List<GameObject>();

    public int Index;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(HandleRooms), 1.0f, 1.0f);
    }

    public void HandleRooms()
    {
        if (RoomsActive.Count > Index + 2)
        {
            Rooms[Index].SetActive(false);
            Index++;
        }
    }
}
