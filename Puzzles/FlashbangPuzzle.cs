using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashbangPuzzle : MonoBehaviour
{
    public GameObject Flame;
    public static bool FlashbangPuzzlePassed;

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Flashbang"))
        {
            if (other.GetComponent<ProjectileScript>().Detonation)
            {
                Flame.SetActive(true);
                FlashbangPuzzlePassed = true;
            }
        }
    }
}
