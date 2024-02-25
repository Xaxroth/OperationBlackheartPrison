using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public AudioClip[] SneakerFootstepSounds;
    public AudioClip[] BackpackSounds;
    public AudioClip[] PantingSounds;
    public AudioClip[] WaterSplashSounds;

    public bool inWater;

    public void Update()
    {
        inWater = PlayerControllerScript.Instance.InWater;
    }

    public void RunningEvent()
    {
        if (!inWater)
        {
            PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(SneakerFootstepSounds[Random.Range(0, SneakerFootstepSounds.Length)], 0.7f);
            PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(BackpackSounds[Random.Range(0, BackpackSounds.Length)], 0.7f);
        }
        else
        {
            PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(SneakerFootstepSounds[Random.Range(0, SneakerFootstepSounds.Length)], 0.7f);
            PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(WaterSplashSounds[Random.Range(0, WaterSplashSounds.Length)], 0.4f);
            PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(BackpackSounds[Random.Range(0, BackpackSounds.Length)], 0.7f);
        }
    }

    public void StopAudio()
    {
        PlayerControllerScript.Instance.PlayerAudioSource.Stop();
    }

    public void PlayFootstep()
    {
        if (!inWater)
        {
            PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(SneakerFootstepSounds[Random.Range(0, SneakerFootstepSounds.Length)], 0.2f);
            PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(BackpackSounds[Random.Range(0, BackpackSounds.Length)], 0.4f);
        }
        else
        {
            PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(SneakerFootstepSounds[Random.Range(0, SneakerFootstepSounds.Length)], 0.2f);
            PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(WaterSplashSounds[Random.Range(0, WaterSplashSounds.Length)], 0.2f);
            PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(BackpackSounds[Random.Range(0, BackpackSounds.Length)], 0.4f);
        }
    }

    public void NatiaFootsteps()
    {
        Natia.Instance.EnemyAudioSource.PlayOneShot(SneakerFootstepSounds[Random.Range(0, SneakerFootstepSounds.Length)], 0.55f);
    }

    public void Panting()
    {
        //PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(PantingSounds[Random.Range(0, BackpackSounds.Length)], 0.7f);
    }
}
