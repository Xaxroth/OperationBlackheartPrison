using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
    public static LootTable Instance;
    
    public ItemSOData[] CommonItems;
    public ItemSOData[] UncommonItems;
    public ItemSOData[] RareItems;
    public ItemSOData[] LegendaryItems;

    public ItemSOData[] UniqueItems;

    public ItemSOData PrisonKey;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Update()
    {
        
    }
}
