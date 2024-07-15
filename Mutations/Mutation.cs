using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mutation : MonoBehaviour
{
    private UnitManager _gameManager;
    public MeshRenderer meshRenderer;

    public bool OverrideMutation;

    [Header("Unit Properties")]

    public MutationData mutationData;

    void Start()
    {
        if (!OverrideMutation)
        {
            mutationData = LootTable.Instance.Mutations[Random.Range(0, LootTable.Instance.Mutations.Length)];
        }

        Material material = meshRenderer.material;

        if (material != null && material.HasProperty("_EmissionColor"))
        {
            material.SetColor("_EmissionColor", mutationData.MutationVialColor);

            material.EnableKeyword("_EMISSION");
        }
    }

    public void ApplyMutation()
    {
        if (mutationData != null)
        {
            PlayerControllerScript player = PlayerControllerScript.Instance;
            Natia natia = Natia.Instance;

            player.playerHealth += (int)mutationData.healAmount;
            player.playerStamina += (int)mutationData.staminaAmount;

            player.playerMaxHealth += (int)mutationData.healthGain;
            player.playerMaxStamina += (int)mutationData.staminaGain;
            player.runSpeed += (int)mutationData.movementGain;
            player.playerJumpHeight += (int)mutationData.jumpGain;
            player.playerConstitution += (int)mutationData.constitutionGain;

            player.MutationLevel += (int)mutationData.MutationGainAmount;
            natia.Affection -= (int)mutationData.NatiaAffectionPenalty;
        }
    }
}
