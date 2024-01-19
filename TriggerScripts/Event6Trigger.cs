using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event6Trigger : MonoBehaviour
{
    public bool EventTriggered = false;
    public Button TriggerButton;
    public BoxCollider Trigger;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!EventTriggered && !DialogueManagerScript.Instance.InProgress && MainRoomPuzzle.AltarUnlocked)
            {
                DialogueManagerScript.Instance.Event6();
                EventTriggered = true;
            }
        }
    }


}
