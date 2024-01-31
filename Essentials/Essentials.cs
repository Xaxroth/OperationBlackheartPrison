using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essentials : MonoBehaviour
{
    public static bool Initialized;
    public static Essentials Instance;

    void Awake()
    {
        if (Initialized)
        {
            Destroy(gameObject);
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Initialized = true;
        }
    }
}
