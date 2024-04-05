using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levers : MonoBehaviour
{
    public bool SwitchedOn;

    public GameObject ObjectToOpen;
    public GameObject[] EnemyToActivate;
    public Animator LeverAnimator;
    public bool LeverState = false;
    public AudioSource LeverAudioSource;
    Animator ObjectAnimator;

    public void Start()
    {
        LeverAudioSource = GetComponent<AudioSource>();
    }

    public void PullLever()
    {
        if (!LeverState)
        {
            ObjectToOpen.SetActive(false);
            for (int i = 0; i < EnemyToActivate.Length; i++)
            {
                EnemyToActivate[i].SetActive(true);
            }
            LeverAudioSource.PlayOneShot(AudioManager.Instance.LeverSound, 1.0f);
            LeverState = true;
        }
    }
}
