using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public float fadeDuration = 3f;
    public float popUpSpeed = 2.0f;
    public float popUpHeight = 100.0f;
    private float TargetAlpha = 1.0f;
    private float NotificationDisplayTime = 3;
    private float ZoneDisplayTime = 9;
    private Vector2 targetPosition;

    public Image blackScreen;
    public GameObject PlayerUI;
    public GameObject HintsPopUp;
    public GameObject GameOverScreenObject;
    public Texture2D CustomCursor;

    public GameObject JumpScareScreen;

    public ItemData itemData;
    public MutationData mutationData;
    public TextMeshProUGUI NotificationText;
    public Color NotificationLetterColor;

    public TextMeshProUGUI ZoneText;
    public Color ZoneLetterColor;

    public GameObject HintBox;
    public GameObject InventoryUIScreen;
    public RectTransform PopUpWindow;
    public TextMeshProUGUI HintText;

    private bool soundPlayed = false;
    public bool ShowZoneText = false;
    public bool changingScene = false;
    public bool DisplayHint;
    public bool menuActive = false;
    public bool ForceCall;
    public bool toggleInventory = false;
    public bool ShowNotification = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        Cursor.SetCursor(CustomCursor, Vector3.zero, CursorMode.ForceSoftware);
        Cursor.visible = false;
        DisplayZone("Blackheart Prison");

    }

    private void Update()
    {
        PlayerMenu();

        if (DisplayHint)
        {
            PopUpWindow.anchoredPosition = Vector2.Lerp(PopUpWindow.anchoredPosition, targetPosition, Time.deltaTime * popUpSpeed);
        }
        else
        {
            PopUpWindow.anchoredPosition = Vector2.Lerp(PopUpWindow.anchoredPosition, new Vector2(targetPosition.x, -popUpHeight), Time.deltaTime * popUpSpeed);
        }

        if (ShowNotification)
        {
            float fadeSpeed = 3f;
            float targetAlpha = 1.0f;
            NotificationLetterColor.a = Mathf.Lerp(NotificationLetterColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            NotificationText.color = NotificationLetterColor;
        }
        else
        {
            float fadeSpeed = 4f;
            float targetAlpha = 0.0f;
            NotificationLetterColor.a = Mathf.Lerp(NotificationLetterColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            NotificationText.color = NotificationLetterColor;
        }

        if (ShowZoneText)
        {
            float fadeSpeed = 1f;
            float targetAlpha = 1.0f;
            ZoneLetterColor.a = Mathf.Lerp(ZoneLetterColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            ZoneText.color = ZoneLetterColor;
        }
        else
        {
            float fadeSpeed = 2f;
            float targetAlpha = 0.0f;
            ZoneLetterColor.a = Mathf.Lerp(ZoneLetterColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            ZoneText.color = ZoneLetterColor;
        }

        if (GameOverScreenObject.activeInHierarchy)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void SetTransform(Transform position)
    {
        PlayerControllerScript.Instance.transform.position = position.position;

        if (Natia.Instance != null)
        {
            if (Natia.Instance.CurrentEnemyState != Natia.NatiaState.Waiting)
            {
                Natia.Instance.NatiaNavMeshAgent.enabled = false;
                Natia.Instance.gameObject.transform.position = position.position;
            }
        }
    }

    public void PlayJumpScare()
    {
        if (!PlayerControllerScript.Instance.Dead)
        {
            if (!JumpScareScreen.activeInHierarchy)
            {
                PlayerControllerScript.Instance.TakeDamage(PlayerControllerScript.Instance.playerMaxHealth);
                AudioManager.Instance.PlaySound(AudioManager.Instance.DemonJumpScare, 1.0f);
                JumpScareScreen.SetActive(true);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);

        DialogueManagerScript.Instance.EndOfDialogue();
        DialogueManagerScript.Instance.CloseDialogue();

        if (Natia.Instance != null)
        {
            Natia.Instance.NatiaNavMeshAgent.enabled = true;
        }

        StartCoroutine(FadeOut());

        if (blackScreen.color.a == 1.0f)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.CloseDoor, 1.0f);
            StartCoroutine(FadeOut());
        }

        if (blackScreen.color.a == 0.0)
        {
            StopAllCoroutines();
        }
    }

    public void DisplayHints()
    {
        DisplayHint = !DisplayHint;
        targetPosition = DisplayHint ? new Vector2(0, popUpHeight) : new Vector2(0, -popUpHeight);
    }

    public void DisplayNotification(ItemSOData itemData)
    {
        NotificationText.text = "Picked up: " + itemData.ItemName;
        ShowNotification = true;
        StartCoroutine(NotificationTimer());
    }

    public void DisplayWorldNotification(string text)
    {
        NotificationText.text = text;
        ShowNotification = true;
        StartCoroutine(NotificationTimer());
    }

    public void DisplayZone(string text)
    {
        ZoneText.text = text;
        ShowZoneText = true;
        StartCoroutine(ZoneTimer());
    }

    public IEnumerator ZoneTimer()
    {
        yield return new WaitForSeconds(ZoneDisplayTime);
        ShowZoneText = false;
    }

    public IEnumerator NotificationTimer()
    {
        yield return new WaitForSeconds(NotificationDisplayTime);
        ShowNotification = false;
    }

    public void LoadGame()
    {
        Time.timeScale = 1;
        GameDataHandler.Instance.LoadGame();
        PlayerUI.SetActive(false);
        menuActive = false;
        PlayerControllerScript.Instance.paralyzed = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PlayerMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuActive)
            {
                PlayerUI.SetActive(true);
                PlayerControllerScript.Instance.paralyzed = true;
                menuActive = true;
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 0;
            }
            else
            {
                PlayerUI.SetActive(false);
                menuActive = false;
                PlayerControllerScript.Instance.paralyzed = false;
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            ForceCall = false;
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !menuActive)
        {
            if (!toggleInventory)
            {
                InventoryUIScreen.SetActive(true);
                PlayerControllerScript.Instance.paralyzed = true;
                toggleInventory = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                AudioManager.Instance.PlaySound(AudioManager.Instance.InventoryToggle, 1.0f);
            }
            else
            {
                InventoryUIScreen.SetActive(false);
                PlayerControllerScript.Instance.paralyzed = false;
                toggleInventory = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                AudioManager.Instance.PlaySound(AudioManager.Instance.InventoryToggle, 1.0f);
            }
        }
    }

    public void GameOverSceen(string GameOverText)
    {
        GameOverScreenObject.SetActive(true);
        JumpScareScreen.SetActive(false);
        GameOverScreenObject.GetComponentInChildren<TextMeshProUGUI>().text = GameOverText;
        PlayerControllerScript.Instance.paralyzed = true;
        menuActive = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void FadeOutScreen()
    {
        StartCoroutine(FadeOut());
    }

    public void FadeInScreen()
    {
        StartCoroutine(FadeIn());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowHint(string Text)
    {
        DisplayHints();
        HintText.text = Text;

        StartCoroutine(HideHints());
    }

    IEnumerator HideHints()
    {
        yield return new WaitForSeconds(6f);
        DisplayHints();
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
    }

    IEnumerator FadeOut()
    {
        Color targetColor = blackScreen.color;
        targetColor.a = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            blackScreen.color = Color.Lerp(blackScreen.color, targetColor, elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        blackScreen.color = targetColor;
    }
}
