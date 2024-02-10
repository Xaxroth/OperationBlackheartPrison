using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRoomPuzzle : MonoBehaviour
{
    public GameObject FirstFlame;
    public GameObject SecondFlame;
    public GameObject ThirdFlame;
    public GameObject FourthFlame;

    public GameObject Blockade;
    public GameObject Gargoyle;

    public static bool AltarUnlocked;

    void Update()
    {
        if (JumpTrialPuzzle.JumpTrialPassed)
        {
            FirstFlame.SetActive(true);
        }

        if (FlashbangPuzzle.FlashbangPuzzlePassed)
        {
            SecondFlame.SetActive(true);
        }

        if (SpikeTrapPuzzle.SpikeTrialPassed)
        {
            ThirdFlame.SetActive(true);
        }

        if (FirstFlame.activeInHierarchy &&  SecondFlame.activeInHierarchy && ThirdFlame.activeInHierarchy && FourthFlame.activeInHierarchy)
        {
            AltarUnlocked = true;
        }

        if (AltarUnlocked)
        {
            Blockade.SetActive(false);
            Gargoyle.SetActive(false);
        }

    }
}
