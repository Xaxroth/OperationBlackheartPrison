using System.Collections;
using System.Collections.Generic;
using System.Media;
using System.Net.Sockets;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public bool locked;

    void Start()
    {
        
    }

    void Update()
    {
    }

    public void OpenChest()
    {
        if (!locked)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.OpenDoor, 1.0f);
        }
        else
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.DoorLocked, 1.0f);

            if (Natia.Instance != null)
            {
                if (!Natia.Instance.Dead && !DialogueManagerScript.Instance.InProgress && Natia.Instance.CurrentEnemyState != Natia.NatiaState.Waiting)
                {
                    DialogueManagerScript.Instance.NatiaLockpicking();
                    Natia.Instance.CanMove = true;
                    Natia.Instance.OpenChest(gameObject);
                }
            }
        }
    }
}
