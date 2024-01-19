using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public BoxCollider KillZoneCollider;

    public void Awake()
    {
        KillZoneCollider = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerControllerScript>().TakeDamage(5000);
            AudioManager.Instance.PlaySound(AudioManager.Instance.FallDeath, 1.0f);
        }
    }

}
