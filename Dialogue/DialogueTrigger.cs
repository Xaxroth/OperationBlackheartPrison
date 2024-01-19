using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public BoxCollider Trigger;
    public bool Finished;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !Finished)
        {
            DialogueManagerScript.Instance.Event2();
            Finished = true;
            // Call on method
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
