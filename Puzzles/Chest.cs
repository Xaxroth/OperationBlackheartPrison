using System.Collections;
using System.Collections.Generic;
using System.Media;
using System.Net.Sockets;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public bool locked;
    public bool payloadDelivered;

    public List<GameObject> Payload = new List<GameObject>();

    public void OpenChest()
    {
        if (!locked)
        {
            //AudioManager.Instance.PlaySound(AudioManager.Instance.OpenDoor, 1.0f);
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

    private void Update()
    {
        if (!payloadDelivered && !locked)
        {
            for (int i = 0; i < Payload.Count; i++)
            {
                Payload[i].SetActive(true);
                Payload[i].GetComponent<Rigidbody>().AddForce(transform.forward * 1, ForceMode.Impulse);
            }

            payloadDelivered = true;
        }
    }
}
