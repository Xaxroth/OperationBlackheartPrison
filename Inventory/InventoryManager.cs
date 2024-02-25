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
            if (Inventory[i].CompareTag("EmptySlot"))
            {
                ItemData newItemData = Inventory[i].GetComponent<ItemData>();
                newItemData.ItemDataSO = ItemGameObject.GetComponent<Mutation>().mutationData;
                // Update the image and string of the existing inventory slot (Inventory[i])
                // Assuming you have components such as Image and Text in the inventory slot GameObject
                //Image itemImage = Inventory[i].GetComponentInChildren<Image>();
                //Text itemNameText = Inventory[i].GetComponentInChildren<Text>();

                //// Assuming ItemGameObject contains components with the image and string you want to update
                //Sprite newItemImage = ItemGameObject.GetComponent<Mutation>().mutationData.MutationImage;
                //string newItemName = ItemGameObject.GetComponent<Mutation>().mutationData.MutationName;

                //// Assign the image and string from ItemGameObject to the inventory slot
                //itemImage.sprite = newItemImage;
                //itemNameText.text = newItemName;

                // Change the tag of the inventory slot to indicate it's filled
                Inventory[i].tag = "FilledSlot";

                // Exit the loop since an empty slot was found and filled
                break;
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

    //public void ListItems()
    //{

    //    //foreach (var item in Mutations)
    //    //{
    //    //    GameObject InventorySlot = Instantiate(InventoryPrefab, ItemContent);

    //    //    // Adjust the position with yOffset
    //    //    Vector3 newPosition = InventorySlot.transform.position + new Vector3(0f, yOffset, 0f);
    //    //    InventorySlot.transform.position = newPosition;

    //    //    // Update yOffset for next iteration
    //    //    yOffset -= yOffsetIncrement; // Adjust this value as needed

    //    //    // Uncomment the lines below if you want to assign values to instantiated objects
    //    //    //var ItemName = InventorySlot.transform.Find("Item/ItemName").GetComponentInChildren<Text>();
    //    //    //var ItemDescription = InventorySlot.transform.Find("Item/ItemName").GetComponent<Text>();
    //    //    //var ItemIcon = InventorySlot.transform.Find("Item/ItemName").GetComponent<Image>();

    //    //    //ItemName.text = item.MutationName;
    //    //    //ItemDescription.text = item.MutantDescription;
    //    //    //ItemIcon.sprite = item.MutationSprite;
    //    //}
    //}

    void Update()
    {
        
    }
}
