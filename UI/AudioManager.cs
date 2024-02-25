using System.Collections;
using System.Collections.Generic;
using System.Media;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip[] GameSounds;

    public Image blackScreen;
    public float fadeDuration = 3f; // Set the duration of the fade in seconds

    public Scene SceneToLoad;
    public string SceneToLoadString;

    public bool changingScene = false;
    private bool soundPlayed = false;

    public AudioSource FlashbangAudioSource;

    public Transform SpawnPosition;

    public AudioClip FallDeath;

    public AudioClip TrapActivated;

    public AudioClip InventoryToggle;

    public AudioClip RevealStinger;
    public AudioClip OpenDoor;
    public AudioClip CloseDoor;
    public AudioClip DoorLocked;

    public AudioClip Dialogue1x1;
    public AudioClip Dialogue1x2;
    public AudioClip Dialogue1x3;
    public AudioClip Dialogue1x4;

    public AudioClip Dialogue1x5;
    public AudioClip Dialogue1x6;
    public AudioClip Dialogue1x7;
    public AudioClip Dialogue1x8;

    public AudioClip Dialogue2x1;
    public AudioClip Dialogue2x2;
    public AudioClip Dialogue2x3;

    public AudioClip Dialogue4x1;
    public AudioClip Dialogue4x2;

    public AudioClip Dialogue5x1;
    public AudioClip Dialogue5x2;
    public AudioClip Dialogue5x3;

    public AudioClip Lockpicking1;
    public AudioClip Lockpicking2;
    public AudioClip Lockpicking3;
    public AudioClip Lockpicking4;

    public AudioClip NatiaDeath;
    public AudioClip HaliconDeath;

    public AudioClip MissionFailed;

    public AudioClip SaveGame;

    public AudioClip BigWaterSplash;
    public AudioClip Pickup;
    public AudioClip KeepDistance;
    public AudioClip StayClose;
    public AudioClip HoldPosition;
    public AudioClip FollowMe;

    public AudioClip PullGrenadePin;
    public AudioClip[] FlashbangSounds;
    public AudioClip EarsRinging;
    public AudioClip NoAmmo;

    public AudioClip NatiaSong;

    public AudioClip Question1;

    public AudioClip PickLock;

    public AudioClip ChargeEnergy;
    public AudioClip ReleaseEnergy;

    public AudioClip BGM1;
    public AudioSource BGMAudioSource;

    public AudioSource GlobalAudioSource;
    public AudioSource PersistentAudioSource;

    public AudioClip[] GoreHitSounds;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GlobalAudioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }

        if (PersistentAudioSource != null)
        {
            PersistentAudioSource.Play();
        }

    }

    public void PlayBGM()
    {

    }

    public void PlaySound(AudioClip Sound, float volume)
    {
        Instance.GlobalAudioSource.PlayOneShot(Sound, volume);
    }

    public void PlayFlashbangEffect()
    {
        FlashbangAudioSource.Play();
    }

    private const float FadeDuration = 0.3f;

    public static void GradualMuteAllSounds()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != Instance.FlashbangAudioSource)
            {
                Instance.StartCoroutine(FadeVolumeOverTime(audioSource, 0f));
            }
        }
    }

    public static void GradualUnmuteAllSounds()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != Instance.FlashbangAudioSource && audioSource != Instance.BGMAudioSource)
            {
                Instance.StartCoroutine(FadeVolumeOverTime(audioSource, 1f));
            }

            if (audioSource == Instance.BGMAudioSource)
            {
                Instance.StartCoroutine(FadeVolumeOverTime(audioSource, 1f));
            }
        }
    }

    private static IEnumerator FadeVolumeOverTime(AudioSource audioSource, float targetVolume)
    {
        float initialVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < FadeDuration && audioSource != null)
        {
            audioSource.volume = Mathf.Lerp(initialVolume, targetVolume, elapsedTime / FadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
