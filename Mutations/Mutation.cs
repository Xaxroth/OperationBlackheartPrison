using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Mutation : MonoBehaviour
{
    private UnitManager _gameManager;
    public MeshRenderer meshRenderer;

    [Header("Unit Properties")]

    public MutationData mutationData;

    void Start()
    {
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
