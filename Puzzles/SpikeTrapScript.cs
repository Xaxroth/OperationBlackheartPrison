using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapScript : MonoBehaviour
{
    public Animator SpikeAnimator;

    public bool WallSpike;

    public void Start()
    {
        SpikeAnimator = GetComponent<Animator>();
    }

    public void OnTriggerEnter(Collider other)
    {
        SpikeAnimator.SetTrigger("SpikeTrigger");
        AudioManager.Instance.PlaySound(AudioManager.Instance.TrapActivated, 1.0f);

        if (!WallSpike)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerControllerScript>().TakeDamage(100);
            }

            if (other.gameObject.CompareTag("Natia"))
            {
                other.gameObject.GetComponent<Natia>().TakeDamage(100);
            }
        }
    }

    public void Activate()
    {
        SpikeAnimator.SetTrigger("SpikeTrigger");
    }
}
