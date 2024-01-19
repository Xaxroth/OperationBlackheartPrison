using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public AudioClip[] SneakerFootstepSounds;
    public AudioClip[] BackpackSounds;
    public AudioClip[] PantingSounds;

    public void RunningEvent()
    {
        PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(SneakerFootstepSounds[Random.Range(0, SneakerFootstepSounds.Length)], 0.7f);
        PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(BackpackSounds[Random.Range(0, BackpackSounds.Length)], 0.7f);
    }

    public void StopAudio()
    {
        PlayerControllerScript.Instance.PlayerAudioSource.Stop();
    }

    public void PlayFootstep()
    {
        PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(SneakerFootstepSounds[Random.Range(0, SneakerFootstepSounds.Length)], 0.2f);
        PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(BackpackSounds[Random.Range(0, BackpackSounds.Length)], 0.4f);
    }

    public void Panting()
    {
        //PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(PantingSounds[Random.Range(0, BackpackSounds.Length)], 0.7f);
    }
}
