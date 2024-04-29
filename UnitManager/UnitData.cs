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
    public float Range = 25;

    public int StoppingDistance = 2;
    public float AttackRadius = 2;

    public AudioClip AttackSound;

    public GameObject ThrownProjectile;

    public bool Stealthed;

    public Material EnemyMaterial;
    public Material ShimmerMaterial;

    [Header("Cosmetics")]

    public AudioClip[] MonsterIdleSounds;
    public AudioClip[] MonsterDetectSounds;
    public AudioClip[] MonsterAttackSounds;

    public enum EnemyAttackType
    {
        Melee,
        Ranged,
        InstantKill
    }

    public EnemyAttackType AttackType;
    public enum UnitType
    {
        Undead,
        Demon,
        Mechanical,
        Organic,
        Neutral
    }

    public UnitType UnitMobilityType;
}
