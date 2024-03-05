using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashbang : MonoBehaviour
{
    [SerializeField] private GameObject flashbang;
    [SerializeField] public Transform ShootPosition;
    [SerializeField] private float flashbangForce;
    [SerializeField] private float flashbangChargeRate = 0.001f;
    public int AmountOfFlashbangs = 5;
    public int MaximumAmountOfFlashbangs = 10;
    private bool chargingFlashbang;

    private void Start()
    {
        SetAmmo();
    }
    void Update()
    {
        ShootPosition.transform.rotation = PlayerControllerScript.Instance.Orientation.transform.rotation;

        if (SetAmmo() > 0)
        {

            if (Input.GetKeyDown(KeyCode.T))
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.PullGrenadePin, 0.1f);
            }

            if (Input.GetKey(KeyCode.T))
            {
                chargingFlashbang = true;
                flashbangForce += flashbangChargeRate;
            }

            if (Input.GetKeyUp(KeyCode.T) && chargingFlashbang)
            {
                ThrowFlashbang();
                AudioManager.Instance.PlaySound(AudioManager.Instance.FlashbangSounds[Random.Range(0, AudioManager.Instance.FlashbangSounds.Length)], 1.0f);
            }
        }

    }

    public int SetAmmo()
    {
        int AmountOfFlashbangs = 0;

        foreach (GameObject item in InventoryManager.Instance.Inventory)
        {
            if (item.CompareTag("FilledSlot"))
            {
                ItemData itemData = item.GetComponent<ItemData>();

                if (itemData.ItemName.Equals("Flashbang"))
                {
                    AmountOfFlashbangs++;
                }
            }
        }

        return AmountOfFlashbangs;
    }

    public void RemoveItem()
    {
        for (int i = 0; i < InventoryManager.Instance.Inventory.Count; i++)
        {
            if (InventoryManager.Instance.Inventory[i].CompareTag("FilledSlot") && InventoryManager.Instance.Inventory[i].gameObject.GetComponent<ItemData>().ItemName.Equals("Flashbang"))
            {
                InventoryManager.Instance.Inventory[i].gameObject.GetComponent<ItemData>().ClearItemSlot();
                break;
            }
        }
    }

    public void ThrowFlashbang()
    {
        GameObject FlashBangGO = Instantiate(flashbang, ShootPosition.position + new Vector3(0, 3, 0), ShootPosition.rotation);

        FlashBangGO.GetComponent<ProjectileScript>().force = flashbangForce;
        RemoveItem();

        SetAmmo();
        chargingFlashbang = false;
        flashbangForce = 0;

    }
}
