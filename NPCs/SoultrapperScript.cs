using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoultrapperScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Enemy enemy = GetComponent<Enemy>();

        foreach (GameObject item in enemy.ItemsDropped)
        {
            item.GetComponent<Pickup>().ItemData = InventoryManager.Instance.FlashbangItem;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
