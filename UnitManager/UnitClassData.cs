using UnityEngine;

[CreateAssetMenu(fileName = "UnitClassData", menuName = "Custom/New Unit Class", order = 1)]
public class UnitClassData : ScriptableObject
{
    public string className;
    public string classDescription;

    public bool promotedClass;

    [Header("Base Stats")]

    public int defaultMovementRange;
    public int defaultHealth;
    public int defaultPower;
    public int defaultSkill;
    public int defaultSpeed;
    public int defaultConstitution;
    public int defaultDefense;
    public int defaultResistance;
    public int defaultLuck;

    [Header("Growth Rates")]

    [Range(0, 1)] public float healthGrowth;
    [Range(0, 1)] public float powerGrowth;
    [Range(0, 1)] public float skillGrowth;
    [Range(0, 1)] public float speedGrowth;
    [Range(0, 1)] public float constitutionGrowth;
    [Range(0, 1)] public float defenseGrowth;
    [Range(0, 1)] public float resistanceGrowth;
    [Range(0, 1)] public float luckGrowth;

    [Header("Weapon Skills")]

    public bool canUseSwords;
    public bool canUseLances;
    public bool canUseAxes;
    public bool canUseBows;

    public bool canUseElemental;
    public bool canUseDark;
    public bool canUseLight;
    public bool canUseStaves;

    [Header("Promotion")]

    public int bonusMovementRange;
    public int bonusHealth;
    public int bonusPower;
    public int bonusSkill;
    public int bonusSpeed;
    public int bonusConstitution;
    public int bonusDefense;
    public int bonusResistance;
    public int bonusLuck;

    public enum UnitType
    {
        Grounded,
        Mounted,
        Flying
    }

    public UnitType UnitMobilityType;
}
