using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverEnemy : MonoBehaviour
{
    GameObject Beam;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }
}
