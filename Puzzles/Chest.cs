using System.Collections;
using System.Collections.Generic;
using System.Media;
using System.Net.Sockets;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public bool locked;
    public bool payloadDelivered;

    public int numberOfItemsInChest;
    public int maxNumberOfItems = 4;

    public GameObject PickupItem;

    public List<GameObject> Payload = new List<GameObject>();

    public void Start()
    {
        int Roll = Random.Range(0, 100);

        if (Roll <= 20)
        {
            numberOfItemsInChest = 0;
        }
        else if (Roll > 20 && Roll < 65)
        {
            numberOfItemsInChest = 1;
        }
        else if (Roll >= 65 && Roll < 80)
        {
            numberOfItemsInChest = 2;
        }
        else if (Roll >= 80 && Roll <= 95)
        {
            numberOfItemsInChest = 3;
        }
        else if (Roll > 95 && Roll <= 100)
        {
            numberOfItemsInChest = 4;
        }

        for (int i = 0; i < numberOfItemsInChest; i++)
        {
            int qualityRoll = Random.Range(0, 100);

            if (qualityRoll <= 90)
            {
                PickupItem.GetComponent<ItemDataHolder>().ItemData = LootTable.Instance.CommonItems[Random.Range(0, LootTable.Instance.CommonItems.Length)];
            }
            else if (qualityRoll > 90 && qualityRoll < 95)
            {
                PickupItem.GetComponent<ItemDataHolder>().ItemData = LootTable.Instance.CommonItems[Random.Range(0, LootTable.Instance.UncommonItems.Length)];
            }
            else if (qualityRoll >= 95 && qualityRoll < 99)
            {
                PickupItem.GetComponent<ItemDataHolder>().ItemData = LootTable.Instance.CommonItems[Random.Range(0, LootTable.Instance.RareItems.Length)];
            }
            else if (qualityRoll == 100)
            {
                PickupItem.GetComponent<ItemDataHolder>().ItemData = LootTable.Instance.CommonItems[Random.Range(0, LootTable.Instance.UniqueItems.Length)];
            }

            GameObject NewItem = Instantiate(PickupItem, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            NewItem.SetActive(false);

            Payload.Add(NewItem);
        }
    }

    public void OpenChest()
    {
        if (!locked)
        {
            //AudioManager.Instance.PlaySound(AudioManager.Instance.OpenDoor, 1.0f);
        }
        else
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.DoorLocked, 1.0f);

            if (Natia.Instance != null)
            {
                if (!Natia.Instance.Dead && !DialogueManagerScript.Instance.InProgress && Natia.Instance.CurrentEnemyState != Natia.NatiaState.Waiting)
                {
                    DialogueManagerScript.Instance.NatiaLockpicking();
                    Natia.Instance.CanMove = true;
                    Natia.Instance.OpenChest(gameObject);
                }
            }
        }
    }

    private void Update()
    {
        if (!payloadDelivered && !locked)
        {
            for (int i = 0; i < Payload.Count; i++)
            {
                Payload[i].SetActive(true);
                Payload[i].GetComponent<Rigidbody>().AddForce(transform.forward * 1, ForceMode.Impulse);
            }

            payloadDelivered = true;
        }
    }
}
