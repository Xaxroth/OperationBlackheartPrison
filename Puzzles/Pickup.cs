using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public MutationData MutationPickupData;
    public enum PickupObject
    {
        Flashbang,
        Ammo,
        Mutation
    }

    public PickupObject ObjectType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MutationPickupData = GetComponent<Mutation>().mutationData;

            InventoryManager.Instance.AddPickupToInventory(gameObject);

            AudioManager.Instance.PlaySound(AudioManager.Instance.Pickup, 1.0f);
            //Destroy(gameObject);
        }

    }
}
