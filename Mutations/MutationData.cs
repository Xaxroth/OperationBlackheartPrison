using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "MutationData", menuName = "Custom/New Mutation Data", order = 2)]
public class MutationData : ScriptableObject
{
    public string MutationName;
    public string MutantDescription;
    public Sprite MutationImage;

    [Header("Growth Rates")]

    [Range(0, 200)] public float healAmount;
    [Range(0, 200)] public float staminaAmount;

    [Range(0, 10)] public float jumpGain;
    [Range(0, 10)] public float movementGain;
    [Range(0, 10)] public float knifeAttackSpeedGain;
    [Range(0, 200)] public float healthGain;
    [Range(0, 200)] public float staminaGain;
    [Range(0, 10)] public float constitutionGain;

    [Range(0, 100)] public float MutationGainAmount;
    [Range(0, 100)] public float NatiaAffectionPenalty;

    [Header("Cosmetics")]

    public Color MutationVialColor;

    public enum Rarity
    {
        Stable,
        Unstable,
        Volatile,
        Dangerous
    }

    public Rarity MutationRarity;
}
