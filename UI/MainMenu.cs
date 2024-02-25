using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public Image blackScreen;
    public float fadeDuration = 3f;
    public AudioSource MainAudioSource;
    public AudioClip StartGame;
    public Texture2D CustomCursor;
    void Start()
    {
        Cursor.SetCursor(CustomCursor, Vector3.zero, CursorMode.ForceSoftware);
    }

    public void StartNewGame()
    {
        StartCoroutine(NewGame());
        MainAudioSource.Stop();
        MainAudioSource.PlayOneShot(StartGame);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator NewGame()
    {
        blackScreen.gameObject.SetActive(true);
        Cursor.visible = false;
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
        SceneManager.LoadScene("Scene_01_Cells");
    }
}
