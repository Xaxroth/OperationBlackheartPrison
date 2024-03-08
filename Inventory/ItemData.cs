using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public MutationData MutationDataSO;
    public ItemSOData ItemDataSO;

    public Image InventoryImage;
    public Image Border;
    public string ItemName;
    public string ItemDescription;
    public GameObject Tooltip;
    public GameObject PickupItem;
    public Text ItemNameText = null;

    public int Quantity = 0;

    private Transform originalParent;

    public GameObject UseMenu;

    public Color TooltipNameTextColor;
    public TextMeshProUGUI FlavorText;
    public TextMeshProUGUI TooltipNameText;
    public TextMeshProUGUI TooltipDescriptionText;

    private bool dragging = false;
    private bool isHovering = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && UseMenu.activeInHierarchy)
        {
            UseMenu.SetActive(false);
        }

        HandleMutations();

        HandleItem();

        if (isHovering && MutationDataSO != null || isHovering && ItemDataSO != null)
        {
            DisplayTooltip();
        }
        else
        {
            HideTooltip();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ItemDataSO != null || MutationDataSO != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.DropItem, 1.0f);
            //originalParent = InventoryImage.transform.parent;
            //InventoryImage.transform.SetParent(transform.parent.parent);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ItemDataSO != null || MutationDataSO != null)
        {
            dragging = true;
            //InventoryImage.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ItemDataSO != null || MutationDataSO != null)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            ItemData clickedItemData = clickedObject.GetComponent<ItemData>();

            dragging = false;

            if (clickedItemData != null)
            {
                if (ItemDataSO != null)
                {
                    //InventoryImage.transform.position = clickedItemData.transform.position;
                    AudioManager.Instance.PlaySound(AudioManager.Instance.MoveItem, 1.0f);
                    clickedItemData.ItemDataSO = ItemDataSO;
                    ClearItemSlot();
                }

                if (MutationDataSO != null)
                {
                    //InventoryImage.transform.position = clickedItemData.transform.position;
                    AudioManager.Instance.PlaySound(AudioManager.Instance.MoveItem, 1.0f);
                    clickedItemData.MutationDataSO = MutationDataSO;
                    ClearItemSlot();
                }
            }
            else
            {
                //InventoryImage.transform.SetParent(originalParent);
                //InventoryImage.transform.position = originalParent.position;
            }
        }
    }

    public void OnEnable()
    {
        UseMenu.SetActive(false);
        Tooltip.SetActive(false);
        isHovering = false;
    }

    public void HandleItem()
    {
        if (ItemDataSO == null && MutationDataSO == null)
        {
            InventoryImage.enabled = false;
            UseMenu.SetActive(false);
            Tooltip.SetActive(false);
        }
        else
        {
            if (ItemDataSO != null)
            {
                switch (ItemDataSO.ItemRarity)
                {
                    case ItemSOData.Rarity.Stable:
                        ItemDataSO.TextColor = ItemDataSO.StableColor;
                        Border.color = ItemDataSO.StableColor;
                        TooltipNameTextColor = ItemDataSO.TextColor;
                        break;
                    case ItemSOData.Rarity.Unstable:
                        ItemDataSO.TextColor = ItemDataSO.UnstableColor;
                        Border.color = ItemDataSO.UnstableColor;
                        TooltipNameTextColor = ItemDataSO.TextColor;
                        break;
                    case ItemSOData.Rarity.Volatile:
                        ItemDataSO.TextColor = ItemDataSO.VolatileColor;
                        Border.color = ItemDataSO.VolatileColor;
                        TooltipNameTextColor = ItemDataSO.TextColor;
                        break;
                    case ItemSOData.Rarity.Dangerous:
                        ItemDataSO.TextColor = ItemDataSO.DangerousColor;
                        Border.color = ItemDataSO.DangerousColor;
                        TooltipNameTextColor = ItemDataSO.TextColor;
                        break;
                }

                FlavorText.text = ItemDataSO.FlavorText;
                InventoryImage.sprite = ItemDataSO.ItemImage;
                InventoryImage.color = ItemDataSO.ItemColor;
                ItemName = ItemDataSO.ItemName;
                ItemNameText.text = ItemName;
                TooltipNameText.text = ItemName;
                TooltipNameText.color = TooltipNameTextColor;
                TooltipDescriptionText.text = ItemDataSO.ItemDescription;
                TooltipDescriptionText.color = Color.white;
                InventoryImage.enabled = true;
            }
        }
    }

    public void HandleMutations()
    {
        if (ItemDataSO == null && MutationDataSO == null)
        {
            InventoryImage.enabled = false;
            UseMenu.SetActive(false);
            Tooltip.SetActive(false);
        }
        else
        {
            if (TooltipNameTextColor != null && MutationDataSO != null)
            {
                switch (MutationDataSO.MutationRarity)
                {
                    case MutationData.Rarity.Stable:
                        MutationDataSO.TextColor = MutationDataSO.StableColor;
                        Border.color = MutationDataSO.StableColor;
                        break;
                    case MutationData.Rarity.Unstable:
                        MutationDataSO.TextColor = MutationDataSO.UnstableColor;
                        Border.color = MutationDataSO.UnstableColor;
                        break;
                    case MutationData.Rarity.Volatile:
                        MutationDataSO.TextColor = MutationDataSO.VolatileColor;
                        Border.color = MutationDataSO.VolatileColor;
                        break;
                    case MutationData.Rarity.Dangerous:
                        MutationDataSO.TextColor = MutationDataSO.DangerousColor;
                        Border.color = MutationDataSO.DangerousColor;
                        break;
                }

                TooltipNameTextColor = MutationDataSO.TextColor;
                FlavorText.text = MutationDataSO.FlavorText;
                InventoryImage.sprite = MutationDataSO.MutationImage;
                InventoryImage.color = MutationDataSO.MutationVialColor;
                ItemName = MutationDataSO.MutationName;
                ItemNameText.text = ItemName;
                TooltipNameText.text = ItemName;
                TooltipNameText.color = TooltipNameTextColor;
                TooltipDescriptionText.text = MutationDataSO.MutantDescription;
                TooltipDescriptionText.color = Color.white;
                InventoryImage.enabled = true;
            }

        }
    }

    public void UseItem()
    {
        if (!dragging && ItemDataSO != null && ItemDataSO.CanBeUsed)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.UseItem, 1.0f);

            switch (MutationDataSO.TypeOfItem)
            {
                case MutationData.ItemType.Mutation:
                    ApplyMutation();
                    break;
            }
        }
    }

    public void DisplayUseMenu()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.SelectItem, 1.0f);
        UseMenu.SetActive(true);
    }

    public void ApplyMutation()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.MutationApplied, 1.0f);
        if (MutationDataSO != null)
        {
            PlayerControllerScript player = PlayerControllerScript.Instance;
            Natia natia = Natia.Instance;

            player.playerHealth += (int)MutationDataSO.healAmount;
            player.playerStamina += (int)MutationDataSO.staminaAmount;

            player.playerMaxHealth += (int)MutationDataSO.healthGain;
            player.playerMaxStamina += (int)MutationDataSO.staminaGain;
            player.runSpeed += MutationDataSO.movementGain;
            player.playerJumpHeight += MutationDataSO.jumpGain;
            player.playerConstitution += (int)MutationDataSO.constitutionGain;

            player.MutationLevel += (int)MutationDataSO.MutationGainAmount;
            natia.Affection -= (int)MutationDataSO.NatiaAffectionPenalty;

            ClearItemSlot();
        }
    }

    public void ClearItemSlot()
    {
        gameObject.tag = "EmptySlot";
        ItemDataSO = null;
        MutationDataSO = null;

    }

    public void DropItem()
    {
        if (ItemDataSO != null && !ItemDataSO.CanBeDropped) return;

        gameObject.tag = "EmptySlot";
        ItemDataSO = null;
        MutationDataSO = null;
        PickupItem.SetActive(true);
        PickupItem.transform.position = PlayerControllerScript.Instance.transform.position + new Vector3(0, 0, 5);
    }

    public void DisplayTooltip()
    {
        gameObject.transform.SetAsLastSibling();
        Tooltip.SetActive(true);
    }

    public void HideTooltip()
    {
        Tooltip.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
