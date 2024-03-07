using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemSOData", menuName = "Custom/New ItemSOData", order = 2)]
public class ItemSOData : ScriptableObject
{
    public string ItemName;
    public string ItemDescription;
    public string FlavorText;
    public Sprite ItemImage;

    public bool CanBeUsed;
    public bool Stackable;

    [Header("Growth Rates")]

    [Range(0, 200)] public float GoldWorthPerUnit;
    [Range(0, 200)] public float MaxStackSize;

    [Header("Cosmetics")]

    public Color ItemColor = Color.white;

    public Color TextColor = Color.white;

    public Color StableColor = Color.white;
    public Color UnstableColor = Color.blue;
    public Color VolatileColor = Color.yellow;
    public Color DangerousColor = Color.green;

    public enum ItemType
    {
        Flashbang,
        Ammo,
        Reagent
    }

    public ItemType TypeOfItem;

    public enum Rarity
    {
        Stable,
        Unstable,
        Volatile,
        Dangerous
    }

    public Rarity ItemRarity;
}
