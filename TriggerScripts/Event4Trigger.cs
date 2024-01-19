using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event4Trigger : MonoBehaviour
{
    public bool EventTriggered = false;
    public Button TriggerButton;
    public BoxCollider Trigger;

    private void OnTriggerEnter(Collider other)
    {
        if (!EventTriggered)
        {
            DialogueManagerScript.Instance.Event4();
            EventTriggered = true;
        }
    }

    public void Update()
    {
        if (!gameObject.GetComponent<Chest>().locked)
        {
            ChestOpened();
        }
    }

    public void ChestOpened()
    {
        if (!EventTriggered && Natia.Instance.CurrentEnemyState != Natia.EnemyState.Waiting)
        {
            DialogueManagerScript.Instance.Event4();
            PlayerControllerScript.Instance.GetComponent<Flashbang>().enabled = true;
            EventTriggered = true;
        }
    }


}
