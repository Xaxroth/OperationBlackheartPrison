using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public int Health = 50;
    public GameObject DestructionEffect;
    public AudioSource DoorAudioSource;
    public Animator DoorAnimator;
    public Collider GateCollider;
    public AudioClip Creak;
    public bool Opened = false;
    public void TakeDamage(int Damage)
    {
        Health -= Damage;

        if (Health <= 0 && !Opened)
        {
            DoorAudioSource.PlayOneShot(Creak);
            DoorAnimator.SetBool("Open", true);
            GateCollider.enabled = false;

            Opened = true;
        }
    }

    public void OpenDoor()
    {
        if (!Opened)
        {
            DoorAudioSource.PlayOneShot(Creak);
            DoorAnimator.SetBool("Open", true);
            GateCollider.enabled = false;

            Opened = true;
        }
    }
}
