using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTrigger : MonoBehaviour
{
    public bool EventTriggered = false;
    public Button TriggerButton;
    public BoxCollider Trigger;

    private void OnTriggerEnter(Collider other)
    {
        if (!EventTriggered)
        {
            DialogueManagerScript.Instance.Event2();
            EventTriggered = true;
        }
    }


}
