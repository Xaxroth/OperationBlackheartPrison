using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event5Trigger : MonoBehaviour
{
    public bool EventTriggered = false;
    public Button TriggerButton;
    public BoxCollider Trigger;

    public void OnTriggerEnter(Collider other)
    {
        if (!EventTriggered && !DialogueManagerScript.Instance.InProgress)
        {
            DialogueManagerScript.Instance.Event5();
            EventTriggered = true;
        }
    }


}
