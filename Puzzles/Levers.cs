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
    public int SpawnRate = 0;
    public int ThresholdToSpawn = 25;
    Animator ObjectAnimator;

    public void Start()
    {
        LeverAudioSource = GetComponent<AudioSource>();
    }

    public void PullLever()
    {
        if (!LeverState)
        {
            SpawnRate = Random.Range(0, 100);
            ObjectToOpen.SetActive(false);

            if (SpawnRate < ThresholdToSpawn)
            {
                for (int i = 0; i < EnemyToActivate.Length; i++)
                {
                    EnemyToActivate[i].SetActive(true);
                }

            }
            LeverAnimator.SetTrigger("Lever");
            LeverAudioSource.PlayOneShot(AudioManager.Instance.LeverSound, 1.0f);
            LeverState = true;
        }
    }
}
