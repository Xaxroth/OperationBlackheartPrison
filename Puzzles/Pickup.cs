using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public MutationData MutationPickupData;
    public ItemSOData ItemData;
    public enum PickupObject
    {
        Flashbang,
        Ammo,
        Mutation,
        Item
    }

    public PickupObject ObjectType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DontDestroyOnLoad(gameObject);
            switch (ObjectType)
            {
                case PickupObject.Item:
                    ItemData = GetComponent<ItemDataHolder>().ItemData;

                    InventoryManager.Instance.AddPickupToInventory(gameObject);

                    AudioManager.Instance.PlaySound(AudioManager.Instance.Pickup, 0.5f);
                    break;
                case PickupObject.Ammo:
                    ItemData = GetComponent<ItemDataHolder>().ItemData;

                    InventoryManager.Instance.AddPickupToInventory(gameObject);

                    AudioManager.Instance.PlaySound(AudioManager.Instance.Pickup, 0.5f);
                    break;
                case PickupObject.Mutation:
                    MutationPickupData = GetComponent<Mutation>().mutationData;

                    InventoryManager.Instance.AddPickupToInventory(gameObject);

                    AudioManager.Instance.PlaySound(AudioManager.Instance.Pickup, 0.5f);
                    break;
            }

            gameObject.SetActive(false);
        }

    }
}
