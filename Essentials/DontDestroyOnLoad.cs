using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public static bool Initialized;
    public DontDestroyOnLoad Instance;

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
