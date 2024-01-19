using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrialPuzzle : MonoBehaviour
{
    public BoxCollider PlayerPlate;
    public BoxCollider NatiaPlate;

    public GameObject PlayerGO;
    public GameObject NatiaGO;
    public GameObject ActivatedGO;
    public bool PlayerInsidePlate;
    public bool NatiaInsidePlate;
    public static bool JumpTrialPassed;

    void Update()
    {
        if (PlayerPlate.bounds.Contains(PlayerControllerScript.Instance.gameObject.transform.position))
        {
            PlayerInsidePlate = true;
        }
        else
        {
            PlayerInsidePlate = false;
        }

        if (NatiaPlate.bounds.Contains(Natia.Instance.gameObject.transform.position))
        {
            NatiaInsidePlate = true;
        }
        else
        {
            NatiaInsidePlate = false;
        }

        if (NatiaInsidePlate && PlayerInsidePlate)
        {
            JumpTrialPassed = true;
            ActivatedGO.SetActive(true);
        }
    }
}
