using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public Transform ItemContent;
    public GameObject InventoryPrefab;
    public int InventorySlots = 5;
    public List<GameObject> Inventory = new List<GameObject>();
    public List<MutationData> Mutations = new List<MutationData>();

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Start()
    {
        InitializeInventory();
    }

    public void AddPickupToInventory(GameObject ItemGameObject)
    {
        for (int i = 0; i < Inventory.Count; i++)
        {
            ItemData newItemData = Inventory[i].GetComponent<ItemData>();
            ItemDataHolder ItemDataContainer = ItemGameObject.GetComponent<ItemDataHolder>();

            if (Inventory[i].CompareTag("EmptySlot"))
            {
                if (ItemDataContainer != null)
                {
                    newItemData.PickupItem = ItemGameObject;
                    newItemData.ItemDataSO = ItemGameObject.GetComponent<ItemDataHolder>().ItemData;
                }
                else
                {
                    newItemData.ItemDataSO = null;
                    newItemData.PickupItem = ItemGameObject;
                    newItemData.MutationDataSO = ItemGameObject.GetComponent<Mutation>().mutationData;
                }

                Inventory[i].tag = "FilledSlot";

                break;
            }
            else
            {
                if (newItemData.ItemDataSO != null && newItemData.ItemDataSO.Stackable == true)
                {
                    if (ItemDataContainer != null)
                    {
                        if (newItemData.ItemDataSO == ItemGameObject.GetComponent<ItemDataHolder>().ItemData)
                        {
                            newItemData.Quantity++;
                            break;
                        }
                    }
                }
            }
        }
    }

    public void RemoveMutationItem(MutationData mutationItem)
    {
        Mutations.Remove(mutationItem);
    }

    public void InitializeInventory()
    {
        float yOffsetIncrement = 85f;
        float xOffsetIncrement = 85f;

        for (int x = 0; x < InventorySlots; x++)
        {
            for (int y = 0; y < InventorySlots; y++)
            {
                GameObject InventorySlot = Instantiate(InventoryPrefab, ItemContent);
                Inventory.Add(InventorySlot);
                Vector3 newPosition = InventorySlot.transform.position + new Vector3(x * xOffsetIncrement, -y * yOffsetIncrement, 0f);
                InventorySlot.transform.position = newPosition;
            }
        }
    }
}
