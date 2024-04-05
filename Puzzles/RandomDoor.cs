using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public class RandomDoor : MonoBehaviour
{
    public Image blackScreen;
    public float fadeDuration = 3f;

    public Scene SceneToLoad;
    public string[] PossibleScenes;
    public string OverrideScene;

    public Transform SpawnPosition;

    public bool PermanentlyLocked;

    public string SuccessMessage;
    public string FailureMessage;
    public bool nextSceneOverride;
    public bool locked;
    public bool needsKey;
    public string RequiredKeyName;
    private bool changingScene = false;
    private bool soundPlayed = false;

    void Start()
    {
        changingScene = false;

        blackScreen = UIManager.Instance.blackScreen;
    }

    void Update()
    {
        if (changingScene)
        {
            StartCoroutine(FadeIn());
            changingScene = false;
        }

    }

    public void ChangeScene()
    {
        if (!locked)
        {
            changingScene = true;
            soundPlayed = false;
            AudioManager.Instance.PlaySound(AudioManager.Instance.OpenDoor, 1.0f);
        }
        else
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.DoorLocked, 1.0f);

            if (Natia.Instance != null)
            {
                if (!Natia.Instance.Dead && !DialogueManagerScript.Instance.InProgress)
                {
                    DialogueManagerScript.Instance.NatiaLockpicking();
                    Natia.Instance.OpenDoor(gameObject);
                }
            }
        }
    }

    IEnumerator FadeIn()
    {
        Color targetColor = blackScreen.color;
        targetColor.a = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            blackScreen.color = Color.Lerp(blackScreen.color, targetColor, elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        blackScreen.color = targetColor;
        UIManager.Instance.SetTransform(SpawnPosition);

        if (!nextSceneOverride)
        {
            SceneManager.LoadScene(PossibleScenes[Random.Range(0, PossibleScenes.Length)]);
        }
        else
        {
            SceneManager.LoadScene(OverrideScene);
        }
    }
}
