using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZekraelDialogueTrigger : MonoBehaviour
{
    bool EventFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!EventFinished)
            {
                DialogueManagerScript.Instance.ZekraelEntrance();
                EventFinished = true;
            }
        }
    }
}
