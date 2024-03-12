using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Custom/New Unit", order = 1)]
public class UnitData : ScriptableObject
{
    public string unitName;
    public string unitDescription;

    [Header("Base Stats")]

    public int health;
    public int damage;
    public float attackCooldown;
    public float movementSpeed;

    public enum UnitType
    {
        Undead,
        Demon,
        Mechanical,
        Organic
    }

    public UnitType UnitMobilityType;
}
