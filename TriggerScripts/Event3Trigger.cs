using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event3Trigger : MonoBehaviour
{
    public bool EventTriggered = false;
    public Button TriggerButton;
    public BoxCollider Trigger;

    private void OnTriggerEnter(Collider other)
    {
        if (!EventTriggered && Natia.Instance.CurrentEnemyState != Natia.EnemyState.Waiting)
        {
            DialogueManagerScript.Instance.Event3();
            EventTriggered = true;
        }
    }


}
